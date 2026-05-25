-- ============================================================
-- AJUSTE: Saldo pendiente por solicitar en sugerencias logísticas
-- ============================================================

-- 1. ALTER TABLE QUOTATION_LOGISTICS_SUGGESTION
-- Agregar columnas de trazabilidad de cantidades

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.QUOTATION_LOGISTICS_SUGGESTION')
      AND name = 'PRODUCTS_ID'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE dbo.QUOTATION_LOGISTICS_SUGGESTION
        ALTER COLUMN PRODUCTS_ID BIGINT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.QUOTATION_LOGISTICS_SUGGESTION')
      AND name = 'WORK_ORDER_ID'
)
BEGIN
    ALTER TABLE dbo.QUOTATION_LOGISTICS_SUGGESTION
        ADD WORK_ORDER_ID BIGINT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.LOGISTICS_REQUEST')
      AND name = 'WORK_ORDER_ID'
)
BEGIN
    ALTER TABLE dbo.LOGISTICS_REQUEST
        ADD WORK_ORDER_ID BIGINT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.LOGISTICS_REQUEST_DETAIL')
      AND name = 'WORK_ORDER_ID'
)
BEGIN
    ALTER TABLE dbo.LOGISTICS_REQUEST_DETAIL
        ADD WORK_ORDER_ID BIGINT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.QUOTATION_LOGISTICS_SUGGESTION')
      AND name = 'SUGGESTED_QUANTITY_BASE'
)
BEGIN
    ALTER TABLE dbo.QUOTATION_LOGISTICS_SUGGESTION
        ADD SUGGESTED_QUANTITY_BASE     DECIMAL(18,4) NULL,
            ALREADY_REQUESTED_QUANTITY  DECIMAL(18,4) NOT NULL CONSTRAINT DF_QLS_ALREADY_REQUESTED  DEFAULT(0),
            PENDING_TO_REQUEST_QUANTITY DECIMAL(18,4) NOT NULL CONSTRAINT DF_QLS_PENDING_TO_REQUEST DEFAULT(0),
            EXCESS_REQUESTED_QUANTITY   DECIMAL(18,4) NOT NULL CONSTRAINT DF_QLS_EXCESS_REQUESTED   DEFAULT(0),
            IS_FULLY_REQUESTED          BIT           NOT NULL CONSTRAINT DF_QLS_IS_FULLY_REQUESTED  DEFAULT(0);
END;
GO

-- NOTA: SOURCE_SUGGESTION_ID, SOURCE_QUOTATION_VER_LIN_ID y CANCELLED_QUANTITY
-- ya existen en LOGISTICS_REQUEST_DETAIL segun logistic-request-tables.sql.
-- No se requiere ALTER TABLE adicional.

-- ============================================================
-- 3. SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS
-- Usa tablas temporales indexadas para evitar re-evaluacion de
-- conjuntos de datos en cada DML y garantizar seeks en los joins.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS
    @BusinessId     BIGINT,
    @QuotationId    BIGINT,
    @QuotationVerId BIGINT = NULL,
    @UserId         BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Limpieza defensiva: si una ejecucion anterior fallo a mitad,
    -- las tablas temporales de sesion pueden haber quedado abiertas.
    DROP TABLE IF EXISTS #ExistingSuggestions;
    DROP TABLE IF EXISTS #AlreadyRequestedBySuggestion;
    DROP TABLE IF EXISTS #AlreadyRequestedByLin;
    DROP TABLE IF EXISTS #Candidates;

    DECLARE @ResolvedQuotationVerId BIGINT;
    DECLARE @Created      TABLE (ID BIGINT);
    DECLARE @Updated      TABLE (ID BIGINT);
    DECLARE @FullyCovered TABLE (ID BIGINT);

    -- --------------------------------------------------------
    -- Validaciones de entrada
    -- --------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1 FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId
          AND BUSINESS_ID  = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SELECT
            @QuotationId      AS QuotationId,
            CAST(0 AS BIGINT) AS QuotationVerId,
            0 AS CreatedCount, 0 AS ExistingCount,
            0 AS FullyRequestedCount, 0 AS PendingTotalCount, 0 AS TotalActiveCount,
            CAST('La cotizacion no existe o no pertenece a la empresa.' AS NVARCHAR(500)) AS Message;
        RETURN;
    END;

    SET @ResolvedQuotationVerId = @QuotationVerId;

    IF @ResolvedQuotationVerId IS NULL
        SELECT @ResolvedQuotationVerId = SELECTED_QUOTATION_VER_ID
        FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId AND BUSINESS_ID = @BusinessId AND STATUS = 1;

    IF @ResolvedQuotationVerId IS NULL
        SELECT TOP (1) @ResolvedQuotationVerId = QUOTATION_VER_ID
        FROM dbo.SALES_QUOTATION_VER_HDR
        WHERE QUOTATION_ID = @QuotationId
        ORDER BY QUOTATION_VER_ID DESC;

    IF @ResolvedQuotationVerId IS NULL
       OR NOT EXISTS (
            SELECT 1 FROM dbo.SALES_QUOTATION_VER_HDR
            WHERE QUOTATION_VER_ID = @ResolvedQuotationVerId AND QUOTATION_ID = @QuotationId
       )
    BEGIN
        SELECT
            @QuotationId AS QuotationId,
            ISNULL(@ResolvedQuotationVerId, 0) AS QuotationVerId,
            0 AS CreatedCount, 0 AS ExistingCount,
            0 AS FullyRequestedCount, 0 AS PendingTotalCount, 0 AS TotalActiveCount,
            CAST('No existe una version seleccionada o activa para la cotizacion.' AS NVARCHAR(500)) AS Message;
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.SALES_QUOTATION_VER_LIN
        WHERE QUOTATION_VER_ID = @ResolvedQuotationVerId
    )
    BEGIN
        SELECT
            @QuotationId            AS QuotationId,
            @ResolvedQuotationVerId AS QuotationVerId,
            0 AS CreatedCount, 0 AS ExistingCount,
            0 AS FullyRequestedCount, 0 AS PendingTotalCount, 0 AS TotalActiveCount,
            CAST('La version de cotizacion no tiene lineas registradas.' AS NVARCHAR(500)) AS Message;
        RETURN;
    END;

    -- Paso 1: Sugerencias existentes activas (scan unico, indexado para joins)
    SELECT
        QUOTATION_LOGISTICS_SUGGESTION_ID,
        QUOTATION_VER_LIN_ID,
        LOGISTICS_SUGGESTION_RULE_ID,
        PRODUCTS_ID,
        DESCRIPTION
    INTO #ExistingSuggestions
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
    WHERE BUSINESS_ID      = @BusinessId
      AND QUOTATION_ID     = @QuotationId
      AND QUOTATION_VER_ID = @ResolvedQuotationVerId
      AND STATUS           = 1;

    CREATE INDEX IX_ES_LIN_RULE ON #ExistingSuggestions (QUOTATION_VER_LIN_ID, LOGISTICS_SUGGESTION_RULE_ID);

    -- Paso 2a: Cantidad solicitada por suggestion (vinculo exacto, preferencia)
    -- Usa IX_LOGISTICS_REQUEST_DETAIL_SOURCE_SUGGESTION (BUSINESS_ID, SOURCE_SUGGESTION_ID, STATUS)
    -- APPROVED_QUANTITY - CANCELLED_QUANTITY = cantidad aun activa en esa linea de detalle
    SELECT
        d.SOURCE_SUGGESTION_ID,
        SUM(d.APPROVED_QUANTITY - d.CANCELLED_QUANTITY) AS ALREADY_REQUESTED_QUANTITY
    INTO #AlreadyRequestedBySuggestion
    FROM dbo.LOGISTICS_REQUEST_DETAIL d
    INNER JOIN dbo.LOGISTICS_REQUEST h
        ON  h.LOGISTICS_REQUEST_ID = d.LOGISTICS_REQUEST_ID
        AND h.STATUS = 1
    INNER JOIN dbo.LOGISTICS_REQUEST_STATUS hrs
        ON  hrs.LOGISTICS_REQUEST_STATUS_ID = h.LOGISTICS_REQUEST_STATUS_ID
        AND hrs.CODE NOT IN ('REJECTED', 'CANCELLED')
    INNER JOIN dbo.LOGISTICS_REQUEST_DETAIL_STATUS drs
        ON  drs.LOGISTICS_REQUEST_DETAIL_STATUS_ID = d.LOGISTICS_REQUEST_DETAIL_STATUS_ID
        AND drs.CODE NOT IN ('REJECTED', 'CANCELLED')
    WHERE d.BUSINESS_ID          = @BusinessId
      AND d.SOURCE_SUGGESTION_ID IS NOT NULL
      AND d.STATUS               = 1
    GROUP BY d.SOURCE_SUGGESTION_ID;

    CREATE UNIQUE INDEX UIX_AR_S ON #AlreadyRequestedBySuggestion (SOURCE_SUGGESTION_ID);

    -- Paso 2b: Cantidad solicitada por linea (fallback, join 1:1 garantizado)
    -- Usa IX_LOGISTICS_REQUEST_DETAIL_QUOTATION_LINE (BUSINESS_ID, SOURCE_QUOTATION_VER_LIN_ID, STATUS)
    SELECT
        d.SOURCE_QUOTATION_VER_LIN_ID,
        SUM(d.APPROVED_QUANTITY - d.CANCELLED_QUANTITY) AS ALREADY_REQUESTED_QUANTITY
    INTO #AlreadyRequestedByLin
    FROM dbo.LOGISTICS_REQUEST_DETAIL d
    INNER JOIN dbo.LOGISTICS_REQUEST h
        ON  h.LOGISTICS_REQUEST_ID = d.LOGISTICS_REQUEST_ID
        AND h.STATUS = 1
    INNER JOIN dbo.LOGISTICS_REQUEST_STATUS hrs
        ON  hrs.LOGISTICS_REQUEST_STATUS_ID = h.LOGISTICS_REQUEST_STATUS_ID
        AND hrs.CODE NOT IN ('REJECTED', 'CANCELLED')
    INNER JOIN dbo.LOGISTICS_REQUEST_DETAIL_STATUS drs
        ON  drs.LOGISTICS_REQUEST_DETAIL_STATUS_ID = d.LOGISTICS_REQUEST_DETAIL_STATUS_ID
        AND drs.CODE NOT IN ('REJECTED', 'CANCELLED')
    WHERE d.BUSINESS_ID                 = @BusinessId
      AND d.SOURCE_QUOTATION_VER_LIN_ID IS NOT NULL
      AND d.STATUS                      = 1
    GROUP BY d.SOURCE_QUOTATION_VER_LIN_ID;

    CREATE UNIQUE INDEX UIX_AR_L ON #AlreadyRequestedByLin (SOURCE_QUOTATION_VER_LIN_ID);

    -- Paso 3: Candidatos con valores derivados (evaluado una vez, DML hacen seeks aqui)
    SELECT
        @QuotationId                                                        AS QUOTATION_ID,
        l.QUOTATION_VER_ID,
        l.QUOTATION_VER_LIN_ID,
        r.LOGISTICS_SUGGESTION_RULE_ID,
        r.LOGISTICS_RESOURCE_TYPE_ID,
        r.SUGGESTED_PRODUCTS_ID                                             AS PRODUCTS_ID,
        COALESCE(NULLIF(LTRIM(RTRIM(r.SUGGESTED_DESCRIPTION)), ''), p.DESCRIPTION, l.DESCRIPTION, 'Sugerencia logistica') AS DESCRIPTION,
        base_qty.v                                                      AS SUGGESTED_QUANTITY_BASE,
        CONCAT(
            'Regla: ', r.RULE_NAME,
            CASE WHEN r.KEYWORD          IS NOT NULL THEN CONCAT(' | Keyword: ', r.KEYWORD) ELSE '' END,
            CASE WHEN r.PRODUCTS_TYPE_ID IS NOT NULL THEN ' | Tipo producto'                ELSE '' END,
            CASE WHEN r.SYSTEM_ID        IS NOT NULL THEN ' | Sistema'                      ELSE '' END,
            CASE WHEN r.LINE_TYPE        IS NOT NULL THEN CONCAT(' | Linea: ', r.LINE_TYPE) ELSE '' END
        )                                                               AS SUGGESTION_REASON,
        bal.ALREADY_REQUESTED_QUANTITY,
        es_match.QUOTATION_LOGISTICS_SUGGESTION_ID                      AS EXISTING_SUGGESTION_ID,
        CAST(CASE WHEN base_qty.v - bal.ALREADY_REQUESTED_QUANTITY < 0
                  THEN 0 ELSE base_qty.v - bal.ALREADY_REQUESTED_QUANTITY
             END AS DECIMAL(18,4))                                      AS PENDING_TO_REQUEST_QUANTITY,
        CAST(CASE WHEN bal.ALREADY_REQUESTED_QUANTITY > base_qty.v
                  THEN bal.ALREADY_REQUESTED_QUANTITY - base_qty.v ELSE 0
             END AS DECIMAL(18,4))                                      AS EXCESS_REQUESTED_QUANTITY,
        CAST(CASE WHEN base_qty.v - bal.ALREADY_REQUESTED_QUANTITY <= 0
                  THEN 1 ELSE 0
             END AS BIT)                                                AS IS_FULLY_REQUESTED
    INTO #Candidates
    FROM dbo.SALES_QUOTATION_VER_LIN l
    INNER JOIN dbo.LOGISTICS_SUGGESTION_RULE r
        ON  r.BUSINESS_ID = @BusinessId
        AND r.STATUS      = 1
        AND (r.KEYWORD         IS NULL OR UPPER(ISNULL(l.DESCRIPTION, '')) LIKE '%' + UPPER(r.KEYWORD) + '%')
        AND (r.PRODUCTS_TYPE_ID IS NULL OR r.PRODUCTS_TYPE_ID = l.PRODUCTS_TYPE_ID)
        AND (r.SYSTEM_ID        IS NULL OR r.SYSTEM_ID        = l.SYSTEM_ID)
        AND (r.LINE_TYPE        IS NULL OR UPPER(r.LINE_TYPE) = UPPER(ISNULL(l.LINE_TYPE, '')))
    LEFT JOIN dbo.PRODUCTS p
        ON  p.PRODUCTS_ID = r.SUGGESTED_PRODUCTS_ID
    -- Cantidad base: CROSS APPLY para calcularla una sola vez y reusarla
    CROSS APPLY (
        SELECT CAST(
            CASE WHEN ISNULL(r.QUANTITY_FACTOR, 0) > 0 AND l.QTY IS NOT NULL
                 THEN l.QTY * r.QUANTITY_FACTOR
                 ELSE r.DEFAULT_QUANTITY
            END AS DECIMAL(18,4)
        ) AS v
    ) base_qty
    -- Sugerencia existente: OUTER APPLY TOP 1 sobre temp indexada (seek garantizado)
    OUTER APPLY (
        SELECT TOP 1 es.QUOTATION_LOGISTICS_SUGGESTION_ID
        FROM #ExistingSuggestions es
        WHERE (es.QUOTATION_VER_LIN_ID = l.QUOTATION_VER_LIN_ID
               OR (es.QUOTATION_VER_LIN_ID IS NULL AND l.QUOTATION_VER_LIN_ID IS NULL))
          AND (es.LOGISTICS_SUGGESTION_RULE_ID = r.LOGISTICS_SUGGESTION_RULE_ID
               OR (es.LOGISTICS_SUGGESTION_RULE_ID IS NULL AND r.LOGISTICS_SUGGESTION_RULE_ID IS NULL))
          AND (es.PRODUCTS_ID = r.SUGGESTED_PRODUCTS_ID
               OR (es.PRODUCTS_ID IS NULL AND r.SUGGESTED_PRODUCTS_ID IS NULL)
               OR UPPER(es.DESCRIPTION) = UPPER(COALESCE(NULLIF(LTRIM(RTRIM(r.SUGGESTED_DESCRIPTION)), ''), p.DESCRIPTION, l.DESCRIPTION, 'Sugerencia logistica')))
    ) es_match
    -- Cantidad solicitada por suggestion (preferencia): LEFT JOIN 1:1 sobre PK
    LEFT JOIN #AlreadyRequestedBySuggestion ar_s
        ON ar_s.SOURCE_SUGGESTION_ID = es_match.QUOTATION_LOGISTICS_SUGGESTION_ID
    -- Cantidad solicitada por linea (fallback): LEFT JOIN 1:1 sobre PK
    LEFT JOIN #AlreadyRequestedByLin ar_l
        ON ar_l.SOURCE_QUOTATION_VER_LIN_ID = l.QUOTATION_VER_LIN_ID
    -- Saldo final: CROSS APPLY para nombrar el valor una vez y reusarlo en los CASE
    CROSS APPLY (
        SELECT ISNULL(ar_s.ALREADY_REQUESTED_QUANTITY,
                      ISNULL(ar_l.ALREADY_REQUESTED_QUANTITY, 0)) AS ALREADY_REQUESTED_QUANTITY
    ) bal
    WHERE l.QUOTATION_VER_ID = @ResolvedQuotationVerId
      AND UPPER(ISNULL(l.LINE_TYPE, '')) = 'ITEM'
      AND l.STATUS = 1
      AND (r.KEYWORD IS NOT NULL OR r.PRODUCTS_TYPE_ID IS NOT NULL
           OR r.SYSTEM_ID IS NOT NULL OR r.LINE_TYPE IS NOT NULL);

    CREATE INDEX IX_CAND_EXISTING ON #Candidates (EXISTING_SUGGESTION_ID);

    -- --------------------------------------------------------
    -- Paso 4 A: Insertar nuevas sugerencias con saldo pendiente
    -- --------------------------------------------------------
    INSERT INTO dbo.QUOTATION_LOGISTICS_SUGGESTION
    (
        BUSINESS_ID, QUOTATION_ID, QUOTATION_VER_ID, QUOTATION_VER_LIN_ID,
        LOGISTICS_SUGGESTION_RULE_ID, LOGISTICS_RESOURCE_TYPE_ID,
        PRODUCTS_ID, DESCRIPTION,
        SUGGESTED_QUANTITY, APPROVED_QUANTITY,
        SUGGESTED_QUANTITY_BASE, ALREADY_REQUESTED_QUANTITY,
        PENDING_TO_REQUEST_QUANTITY, EXCESS_REQUESTED_QUANTITY, IS_FULLY_REQUESTED,
        IS_SELECTED, IS_MANUAL, IS_DUPLICATED,
        SUGGESTION_REASON, CREATE_USER, STATUS
    )
    OUTPUT INSERTED.QUOTATION_LOGISTICS_SUGGESTION_ID INTO @Created(ID)
    SELECT
        @BusinessId,
        QUOTATION_ID, QUOTATION_VER_ID, QUOTATION_VER_LIN_ID,
        LOGISTICS_SUGGESTION_RULE_ID, LOGISTICS_RESOURCE_TYPE_ID,
        PRODUCTS_ID, DESCRIPTION,
        PENDING_TO_REQUEST_QUANTITY,
        PENDING_TO_REQUEST_QUANTITY,
        SUGGESTED_QUANTITY_BASE, ALREADY_REQUESTED_QUANTITY,
        PENDING_TO_REQUEST_QUANTITY, EXCESS_REQUESTED_QUANTITY, IS_FULLY_REQUESTED,
        1, 0, 0,
        SUGGESTION_REASON, @UserId, 1
    FROM #Candidates
    WHERE EXISTING_SUGGESTION_ID IS NULL
      AND PENDING_TO_REQUEST_QUANTITY > 0;

    -- --------------------------------------------------------
    -- Paso 4 B: Actualizar existentes con saldo pendiente
    -- --------------------------------------------------------
    UPDATE s
    SET
        s.SUGGESTED_QUANTITY          = c.PENDING_TO_REQUEST_QUANTITY,
        s.APPROVED_QUANTITY           = c.PENDING_TO_REQUEST_QUANTITY,
        s.SUGGESTED_QUANTITY_BASE     = c.SUGGESTED_QUANTITY_BASE,
        s.ALREADY_REQUESTED_QUANTITY  = c.ALREADY_REQUESTED_QUANTITY,
        s.PENDING_TO_REQUEST_QUANTITY = c.PENDING_TO_REQUEST_QUANTITY,
        s.EXCESS_REQUESTED_QUANTITY   = c.EXCESS_REQUESTED_QUANTITY,
        s.IS_FULLY_REQUESTED          = 0,
        s.IS_SELECTED                 = 1,
        s.UPDATE_USER                 = @UserId,
        s.UPDATE_DATE                 = GETDATE()
    OUTPUT INSERTED.QUOTATION_LOGISTICS_SUGGESTION_ID INTO @Updated(ID)
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
    INNER JOIN #Candidates c ON s.QUOTATION_LOGISTICS_SUGGESTION_ID = c.EXISTING_SUGGESTION_ID
    WHERE c.PENDING_TO_REQUEST_QUANTITY > 0
      AND NOT EXISTS (SELECT 1 FROM @Created cr WHERE cr.ID = s.QUOTATION_LOGISTICS_SUGGESTION_ID);

    -- --------------------------------------------------------
    -- Paso 4 C: Marcar como completamente solicitadas
    -- --------------------------------------------------------
    UPDATE s
    SET
        s.SUGGESTED_QUANTITY_BASE     = c.SUGGESTED_QUANTITY_BASE,
        s.ALREADY_REQUESTED_QUANTITY  = c.ALREADY_REQUESTED_QUANTITY,
        s.PENDING_TO_REQUEST_QUANTITY = 0,
        s.EXCESS_REQUESTED_QUANTITY   = c.EXCESS_REQUESTED_QUANTITY,
        s.IS_FULLY_REQUESTED          = 1,
        s.IS_SELECTED                 = 0,
        s.UPDATE_USER                 = @UserId,
        s.UPDATE_DATE                 = GETDATE()
    OUTPUT INSERTED.QUOTATION_LOGISTICS_SUGGESTION_ID INTO @FullyCovered(ID)
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
    INNER JOIN #Candidates c ON s.QUOTATION_LOGISTICS_SUGGESTION_ID = c.EXISTING_SUGGESTION_ID
    WHERE c.IS_FULLY_REQUESTED = 1;

    -- --------------------------------------------------------
    -- Resultado final
    -- --------------------------------------------------------
    DECLARE @CreatedCount      INT = (SELECT COUNT(1) FROM @Created);
    DECLARE @UpdatedCount      INT = (SELECT COUNT(1) FROM @Updated);
    DECLARE @FullyCoveredCount INT = (SELECT COUNT(1) FROM @FullyCovered);
    DECLARE @PendingTotalCount INT = 0;
    DECLARE @TotalActiveCount  INT = 0;

    SELECT
        @PendingTotalCount = SUM(CASE WHEN ISNULL(IS_FULLY_REQUESTED, 0) = 0 THEN 1 ELSE 0 END),
        @TotalActiveCount  = COUNT(1)
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
    WHERE BUSINESS_ID      = @BusinessId
      AND QUOTATION_ID     = @QuotationId
      AND QUOTATION_VER_ID = @ResolvedQuotationVerId
      AND STATUS           = 1;

    SELECT
        @QuotationId            AS QuotationId,
        @ResolvedQuotationVerId AS QuotationVerId,
        @CreatedCount           AS CreatedCount,
        @UpdatedCount           AS ExistingCount,
        @FullyCoveredCount      AS FullyRequestedCount,
        ISNULL(@PendingTotalCount, 0) AS PendingTotalCount,
        ISNULL(@TotalActiveCount, 0)  AS TotalActiveCount,
        CAST(
            CASE
                WHEN @CreatedCount > 0
                    THEN 'Sugerencias generadas correctamente.'
                WHEN @FullyCoveredCount > 0 AND @CreatedCount = 0
                    THEN 'No se crearon nuevas sugerencias porque las cantidades ya fueron solicitadas.'
                ELSE
                    'No se crearon nuevas sugerencias. Puede que no existan reglas aplicables o que ya hayan sido generadas.'
            END AS NVARCHAR(500)
        ) AS Message;

    DROP TABLE IF EXISTS #ExistingSuggestions;
    DROP TABLE IF EXISTS #AlreadyRequestedBySuggestion;
    DROP TABLE IF EXISTS #AlreadyRequestedByLin;
    DROP TABLE IF EXISTS #Candidates;
END;
GO

-- ============================================================
-- 4. SP_WS_LIST_QUOTATION_LOGISTICS_SUGGESTIONS
-- Incluye los nuevos campos de saldo
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_LIST_QUOTATION_LOGISTICS_SUGGESTIONS
    @BusinessId     BIGINT,
    @QuotationId    BIGINT,
    @QuotationVerId BIGINT     = NULL,
    @ResourceTypeId BIGINT     = NULL,
    @WorkOrderId    BIGINT     = NULL,
    @OnlySelected   BIT        = NULL,
    @Search         NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.QUOTATION_LOGISTICS_SUGGESTION_ID  AS QuotationLogisticsSuggestionId,
        s.BUSINESS_ID                        AS BusinessId,
        s.QUOTATION_ID                       AS QuotationId,
        s.QUOTATION_VER_ID                   AS QuotationVerId,
        s.QUOTATION_VER_LIN_ID               AS QuotationVerLinId,
        s.WORK_ORDER_ID                      AS WorkOrderId,
        wo.WORK_ORDER_CODE                   AS WorkOrderCode,
        wo.WORK_ORDER_NAME                   AS WorkOrderName,
        l.DESCRIPTION                        AS LineDescription,
        l.QTY                                AS LineQty,
        s.LOGISTICS_SUGGESTION_RULE_ID       AS LogisticsSuggestionRuleId,
        s.LOGISTICS_RESOURCE_TYPE_ID         AS LogisticsResourceTypeId,
        rt.CODE                              AS ResourceTypeCode,
        rt.DESCRIPTION                       AS ResourceTypeDescription,
        s.PRODUCTS_ID                        AS ProductsId,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 'WITHOUT_PRODUCT'
            ELSE 'WITH_PRODUCT'
        END                                  AS ProductStatus,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 'NOT_EVALUATED'
            WHEN ISNULL(p.IS_STOCKABLE, 0) = 0 THEN 'NOT_STOCKABLE'
            WHEN ISNULL(stock.AvailableQuantity, 0) >= ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) THEN 'AVAILABLE'
            WHEN ISNULL(stock.AvailableQuantity, 0) > 0 THEN 'PARTIAL'
            ELSE 'NOT_AVAILABLE'
        END                                  AS StockStatus,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 'MAP_OR_CREATE_PRODUCT'
            WHEN ISNULL(p.IS_STOCKABLE, 0) = 0 THEN 'REVIEW_BY_LOGISTICS'
            WHEN ISNULL(stock.AvailableQuantity, 0) >= ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) THEN 'ATTEND_FROM_STOCK'
            ELSE 'PURCHASE_OR_RENTAL'
        END                                  AS SuggestedAction,
        CASE WHEN s.PRODUCTS_ID IS NULL THEN 0 ELSE ISNULL(stock.StockQuantity, 0) END AS StockQuantity,
        CASE WHEN s.PRODUCTS_ID IS NULL THEN 0 ELSE ISNULL(stock.AvailableQuantity, 0) END AS AvailableStockQuantity,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 0
            WHEN ISNULL(p.IS_STOCKABLE, 0) = 0 THEN 0
            WHEN ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) - ISNULL(stock.AvailableQuantity, 0) > 0
                THEN ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) - ISNULL(stock.AvailableQuantity, 0)
            ELSE 0
        END                                  AS MissingQuantity,
        p.SKU                                AS ProductCode,
        p.DESCRIPTION                        AS ProductName,
        s.DESCRIPTION                        AS Description,
        s.SUGGESTED_QUANTITY                 AS SuggestedQuantity,
        s.SUGGESTED_QUANTITY_BASE            AS SuggestedQuantityBase,
        s.ALREADY_REQUESTED_QUANTITY         AS AlreadyRequestedQuantity,
        s.PENDING_TO_REQUEST_QUANTITY        AS PendingToRequestQuantity,
        s.EXCESS_REQUESTED_QUANTITY          AS ExcessRequestedQuantity,
        s.IS_FULLY_REQUESTED                 AS IsFullyRequested,
        s.APPROVED_QUANTITY                  AS ApprovedQuantity,
        s.IS_SELECTED                        AS IsSelected,
        s.IS_MANUAL                          AS IsManual,
        s.IS_DUPLICATED                      AS IsDuplicated,
        s.SUGGESTION_REASON                  AS SuggestionReason,
        s.OFFICE_OBSERVATION                 AS OfficeObservation,
        s.REVIEWED_BY                        AS ReviewedBy,
        s.REVIEWED_DATE                      AS ReviewedDate,
        s.STATUS                             AS Status
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
    INNER JOIN dbo.LOGISTICS_RESOURCE_TYPE rt
        ON  rt.LOGISTICS_RESOURCE_TYPE_ID = s.LOGISTICS_RESOURCE_TYPE_ID
    LEFT JOIN dbo.SALES_QUOTATION_VER_LIN l
        ON  l.QUOTATION_VER_LIN_ID = s.QUOTATION_VER_LIN_ID
    LEFT JOIN dbo.OPERATIONS_WORK_ORDER wo
        ON  wo.WORK_ORDER_ID = s.WORK_ORDER_ID
    LEFT JOIN dbo.PRODUCTS p
        ON  p.PRODUCTS_ID = s.PRODUCTS_ID
    OUTER APPLY (
        SELECT
            SUM(ISNULL(i.STOCK_QUANTITY, 0)) AS StockQuantity,
            SUM(ISNULL(i.AVAILABLE_QUANTITY, i.STOCK_QUANTITY)) AS AvailableQuantity
        FROM dbo.INVENTORY_STOCK i
        WHERE i.BUSINESS_ID = s.BUSINESS_ID
          AND i.PRODUCTS_ID = s.PRODUCTS_ID
          AND i.STATUS = 1
    ) stock
    WHERE s.BUSINESS_ID  = @BusinessId
      AND s.QUOTATION_ID = @QuotationId
      AND (@QuotationVerId IS NULL OR s.QUOTATION_VER_ID         = @QuotationVerId)
      AND (@ResourceTypeId IS NULL OR s.LOGISTICS_RESOURCE_TYPE_ID = @ResourceTypeId)
      AND (@WorkOrderId IS NULL OR s.WORK_ORDER_ID = @WorkOrderId)
      AND (@OnlySelected  IS NULL OR s.IS_SELECTED               = @OnlySelected)
      AND s.STATUS = 1
      AND (
            @Search IS NULL
            OR s.DESCRIPTION         LIKE '%' + @Search + '%'
            OR ISNULL(p.DESCRIPTION, '') LIKE '%' + @Search + '%'
            OR ISNULL(l.DESCRIPTION, '') LIKE '%' + @Search + '%'
          )
    ORDER BY rt.CODE, s.DESCRIPTION;
END;
GO

-- ============================================================
-- 5. SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION
-- Valida que ApprovedQuantity no supere PendingToRequestQuantity
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION
    @SuggestionId        BIGINT,
    @BusinessId          BIGINT,
    @IsSelected          BIT,
    @ApprovedQuantity    DECIMAL(18,4),
    @OfficeObservation   NVARCHAR(500) = NULL,
    @WorkOrderId         BIGINT        = NULL,
    @UserId              BIGINT,
    @COutput             INT           OUTPUT,
    @SOutput             NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RecordStatus     BIT;
    DECLARE @PendingToRequest DECIMAL(18,4);
    DECLARE @IsFullyReq       BIT;

    -- Una sola lectura reemplaza los dos IF NOT EXISTS + SELECT previos
    SELECT
        @RecordStatus     = STATUS,
        @PendingToRequest = PENDING_TO_REQUEST_QUANTITY,
        @IsFullyReq       = IS_FULLY_REQUESTED
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
    WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
      AND BUSINESS_ID = @BusinessId;

    IF @RecordStatus IS NULL
    BEGIN SET @COutput = 0; SET @SOutput = 'La sugerencia no existe.'; RETURN; END;

    IF @RecordStatus = 0
    BEGIN SET @COutput = 0; SET @SOutput = 'No se puede editar una sugerencia desactivada.'; RETURN; END;

    IF @IsSelected = 1 AND ISNULL(@IsFullyReq, 0) = 1
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'No se puede seleccionar una sugerencia que ya fue completamente solicitada.';
        RETURN;
    END;

    IF @ApprovedQuantity < 0 OR (@IsSelected = 1 AND @ApprovedQuantity <= 0)
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La cantidad aprobada no es valida.';
        RETURN;
    END;

    IF @IsSelected = 1
       AND ISNULL(@PendingToRequest, 0) > 0
       AND @ApprovedQuantity > @PendingToRequest
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La cantidad aprobada no puede superar la cantidad pendiente por solicitar.';
        RETURN;
    END;

    IF @WorkOrderId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
        )
        BEGIN
            SET @COutput = 0;
            SET @SOutput = 'La orden de trabajo seleccionada no existe o no pertenece a la empresa.';
            RETURN;
        END;

        IF EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
              AND ISNULL(NEED_LOGISTICS, 0) = 0
        )
        BEGIN
            SET @COutput = 0;
            SET @SOutput = 'La orden de trabajo seleccionada no requiere logistica.';
            RETURN;
        END;
    END;

    UPDATE dbo.QUOTATION_LOGISTICS_SUGGESTION
    SET IS_SELECTED        = @IsSelected,
        APPROVED_QUANTITY  = @ApprovedQuantity,
        OFFICE_OBSERVATION = @OfficeObservation,
        WORK_ORDER_ID      = @WorkOrderId,
        REVIEWED_BY        = @UserId,
        REVIEWED_DATE      = GETDATE(),
        UPDATE_USER        = @UserId,
        UPDATE_DATE        = GETDATE()
    WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
      AND BUSINESS_ID = @BusinessId
      AND STATUS = 1;

    SET @COutput = 1;
    SET @SOutput = 'Sugerencia actualizada correctamente.';
END;
GO

-- ============================================================
-- 6. SP_WS_ADD_MANUAL_QUOTATION_LOGISTICS_SUGGESTION
-- Inicializa los campos de saldo en sugerencias manuales
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_ADD_MANUAL_QUOTATION_LOGISTICS_SUGGESTION
    @BusinessId             BIGINT,
    @QuotationId            BIGINT,
    @QuotationVerId         BIGINT,
    @WorkOrderId            BIGINT        = NULL,
    @LogisticsResourceTypeId BIGINT,
    @ProductsId             BIGINT        = NULL,
    @Description            NVARCHAR(500) = NULL,
    @SuggestedQuantity      DECIMAL(18,4),
    @ApprovedQuantity       DECIMAL(18,4),
    @OfficeObservation      NVARCHAR(500) = NULL,
    @UserId                 BIGINT,
    @Id                     BIGINT        OUTPUT,
    @COutput                INT           OUTPUT,
    @SOutput                NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = 0;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId AND BUSINESS_ID = @BusinessId AND STATUS = 1
    )
    BEGIN SET @COutput = 0; SET @SOutput = 'La cotizacion no existe o no pertenece a la empresa.'; RETURN; END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.SALES_QUOTATION_VER_HDR
        WHERE QUOTATION_VER_ID = @QuotationVerId AND QUOTATION_ID = @QuotationId
    )
    BEGIN SET @COutput = 0; SET @SOutput = 'La version de cotizacion no existe o no esta activa.'; RETURN; END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.LOGISTICS_RESOURCE_TYPE
        WHERE LOGISTICS_RESOURCE_TYPE_ID = @LogisticsResourceTypeId AND STATUS = 1
    )
    BEGIN SET @COutput = 0; SET @SOutput = 'El tipo de recurso logistico no existe o esta inactivo.'; RETURN; END;

    IF @ProductsId IS NOT NULL
       AND NOT EXISTS (
            SELECT 1 FROM dbo.PRODUCTS
            WHERE PRODUCTS_ID = @ProductsId AND BUSINESS_ID = @BusinessId AND STATUS = 1
       )
    BEGIN SET @COutput = 0; SET @SOutput = 'El producto no existe o esta inactivo.'; RETURN; END;

    IF @ProductsId IS NULL AND NULLIF(LTRIM(RTRIM(ISNULL(@Description, ''))), '') IS NULL
    BEGIN SET @COutput = 0; SET @SOutput = 'La descripcion es obligatoria si no se informa producto.'; RETURN; END;

    IF @WorkOrderId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
        )
        BEGIN SET @COutput = 0; SET @SOutput = 'La orden de trabajo seleccionada no existe o no pertenece a la empresa.'; RETURN; END;

        IF EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
              AND ISNULL(NEED_LOGISTICS, 0) = 0
        )
        BEGIN SET @COutput = 0; SET @SOutput = 'La orden de trabajo seleccionada no requiere logistica.'; RETURN; END;
    END;

    IF @SuggestedQuantity <= 0 OR @ApprovedQuantity < 0
    BEGIN SET @COutput = 0; SET @SOutput = 'Las cantidades informadas no son validas.'; RETURN; END;

    IF @ProductsId IS NOT NULL AND NULLIF(LTRIM(RTRIM(ISNULL(@Description, ''))), '') IS NULL
        SELECT @Description = DESCRIPTION FROM dbo.PRODUCTS WHERE PRODUCTS_ID = @ProductsId;

    INSERT INTO dbo.QUOTATION_LOGISTICS_SUGGESTION
    (
        BUSINESS_ID, QUOTATION_ID, QUOTATION_VER_ID, QUOTATION_VER_LIN_ID,
        WORK_ORDER_ID,
        LOGISTICS_SUGGESTION_RULE_ID, LOGISTICS_RESOURCE_TYPE_ID,
        PRODUCTS_ID, DESCRIPTION,
        SUGGESTED_QUANTITY, APPROVED_QUANTITY,
        SUGGESTED_QUANTITY_BASE, ALREADY_REQUESTED_QUANTITY,
        PENDING_TO_REQUEST_QUANTITY, EXCESS_REQUESTED_QUANTITY, IS_FULLY_REQUESTED,
        IS_SELECTED, IS_MANUAL, IS_DUPLICATED,
        SUGGESTION_REASON, OFFICE_OBSERVATION, REVIEWED_BY, REVIEWED_DATE,
        CREATE_USER, STATUS
    )
    VALUES
    (
        @BusinessId, @QuotationId, @QuotationVerId, NULL,
        @WorkOrderId,
        NULL, @LogisticsResourceTypeId,
        @ProductsId, @Description,
        @SuggestedQuantity, @ApprovedQuantity,
        @SuggestedQuantity, 0,   -- base = sugerida, ya solicitado = 0
        @SuggestedQuantity, 0, 0, -- pendiente = sugerida, sin exceso, no cubierto
        1, 1, 0,
        'Sugerencia manual de oficina', @OfficeObservation, @UserId, GETDATE(),
        @UserId, 1
    );

    SET @Id     = SCOPE_IDENTITY();
    SET @COutput = 1;
    SET @SOutput = 'Sugerencia manual agregada correctamente.';
END;
GO

-- ============================================================
-- 7. SP_WS_ASSIGN_WORK_ORDER_TO_QUOTATION_LOGISTICS_SUGGESTION
-- Asigna o limpia la OT asociada a una sugerencia.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_ASSIGN_WORK_ORDER_TO_QUOTATION_LOGISTICS_SUGGESTION
    @BusinessId   BIGINT,
    @SuggestionId BIGINT,
    @WorkOrderId  BIGINT        = NULL,
    @UserId       BIGINT,
    @COutput      INT           OUTPUT,
    @SOutput      NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
        WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La sugerencia no existe o esta desactivada.';
        RETURN;
    END;

    IF @WorkOrderId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
        )
        BEGIN
            SET @COutput = 0;
            SET @SOutput = 'La orden de trabajo seleccionada no existe o no pertenece a la empresa.';
            RETURN;
        END;

        IF EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
              AND ISNULL(NEED_LOGISTICS, 0) = 0
        )
        BEGIN
            SET @COutput = 0;
            SET @SOutput = 'La orden de trabajo seleccionada no requiere logistica.';
            RETURN;
        END;
    END;

    UPDATE dbo.QUOTATION_LOGISTICS_SUGGESTION
    SET WORK_ORDER_ID = @WorkOrderId,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
      AND BUSINESS_ID = @BusinessId
      AND STATUS = 1;

    SET @COutput = 1;
    SET @SOutput = 'Orden de trabajo asignada correctamente.';
END;
GO

-- ============================================================
-- 8. SP_WS_DISABLE_QUOTATION_LOGISTICS_SUGGESTION (sin cambios)
-- Se incluye por completitud; no requiere modificaciones.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_DISABLE_QUOTATION_LOGISTICS_SUGGESTION
    @SuggestionId BIGINT,
    @BusinessId   BIGINT,
    @UserId       BIGINT,
    @COutput      INT           OUTPUT,
    @SOutput      NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
        WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La sugerencia no existe o ya esta desactivada.';
        RETURN;
    END;

    UPDATE dbo.QUOTATION_LOGISTICS_SUGGESTION
    SET STATUS      = 0,
        IS_SELECTED = 0,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
      AND BUSINESS_ID = @BusinessId
      AND STATUS = 1;

    SET @COutput = 1;
    SET @SOutput = 'Sugerencia desactivada correctamente.';
END;
GO

-- ============================================================
-- 9. SP_WS_GET_QUOTATION_LOGISTICS_SUGGESTION_BY_ID
-- Devuelve una sugerencia por ID para auditoría y validaciones.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_QUOTATION_LOGISTICS_SUGGESTION_BY_ID
    @BusinessId   BIGINT,
    @SuggestionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.QUOTATION_LOGISTICS_SUGGESTION_ID  AS QuotationLogisticsSuggestionId,
        s.BUSINESS_ID                        AS BusinessId,
        s.QUOTATION_ID                       AS QuotationId,
        s.QUOTATION_VER_ID                   AS QuotationVerId,
        s.QUOTATION_VER_LIN_ID               AS QuotationVerLinId,
        s.WORK_ORDER_ID                      AS WorkOrderId,
        wo.WORK_ORDER_CODE                   AS WorkOrderCode,
        wo.WORK_ORDER_NAME                   AS WorkOrderName,
        l.DESCRIPTION                        AS LineDescription,
        l.QTY                                AS LineQty,
        s.LOGISTICS_SUGGESTION_RULE_ID       AS LogisticsSuggestionRuleId,
        s.LOGISTICS_RESOURCE_TYPE_ID         AS LogisticsResourceTypeId,
        rt.CODE                              AS ResourceTypeCode,
        rt.DESCRIPTION                       AS ResourceTypeDescription,
        s.PRODUCTS_ID                        AS ProductsId,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 'WITHOUT_PRODUCT'
            ELSE 'WITH_PRODUCT'
        END                                  AS ProductStatus,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 'NOT_EVALUATED'
            WHEN ISNULL(p.IS_STOCKABLE, 0) = 0 THEN 'NOT_STOCKABLE'
            WHEN ISNULL(stock.AvailableQuantity, 0) >= ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) THEN 'AVAILABLE'
            WHEN ISNULL(stock.AvailableQuantity, 0) > 0 THEN 'PARTIAL'
            ELSE 'NOT_AVAILABLE'
        END                                  AS StockStatus,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 'MAP_OR_CREATE_PRODUCT'
            WHEN ISNULL(p.IS_STOCKABLE, 0) = 0 THEN 'REVIEW_BY_LOGISTICS'
            WHEN ISNULL(stock.AvailableQuantity, 0) >= ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) THEN 'ATTEND_FROM_STOCK'
            ELSE 'PURCHASE_OR_RENTAL'
        END                                  AS SuggestedAction,
        CASE WHEN s.PRODUCTS_ID IS NULL THEN 0 ELSE ISNULL(stock.StockQuantity, 0) END AS StockQuantity,
        CASE WHEN s.PRODUCTS_ID IS NULL THEN 0 ELSE ISNULL(stock.AvailableQuantity, 0) END AS AvailableStockQuantity,
        CASE
            WHEN s.PRODUCTS_ID IS NULL THEN 0
            WHEN ISNULL(p.IS_STOCKABLE, 0) = 0 THEN 0
            WHEN ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) - ISNULL(stock.AvailableQuantity, 0) > 0
                THEN ISNULL(s.APPROVED_QUANTITY, s.SUGGESTED_QUANTITY) - ISNULL(stock.AvailableQuantity, 0)
            ELSE 0
        END                                  AS MissingQuantity,
        p.SKU                                AS ProductCode,
        p.DESCRIPTION                        AS ProductName,
        s.DESCRIPTION                        AS Description,
        s.SUGGESTED_QUANTITY                 AS SuggestedQuantity,
        s.SUGGESTED_QUANTITY_BASE            AS SuggestedQuantityBase,
        s.ALREADY_REQUESTED_QUANTITY         AS AlreadyRequestedQuantity,
        s.PENDING_TO_REQUEST_QUANTITY        AS PendingToRequestQuantity,
        s.EXCESS_REQUESTED_QUANTITY          AS ExcessRequestedQuantity,
        s.IS_FULLY_REQUESTED                 AS IsFullyRequested,
        s.APPROVED_QUANTITY                  AS ApprovedQuantity,
        s.IS_SELECTED                        AS IsSelected,
        s.IS_MANUAL                          AS IsManual,
        s.IS_DUPLICATED                      AS IsDuplicated,
        s.SUGGESTION_REASON                  AS SuggestionReason,
        s.OFFICE_OBSERVATION                 AS OfficeObservation,
        s.REVIEWED_BY                        AS ReviewedBy,
        s.REVIEWED_DATE                      AS ReviewedDate,
        s.STATUS                             AS Status
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
    INNER JOIN dbo.LOGISTICS_RESOURCE_TYPE rt
        ON  rt.LOGISTICS_RESOURCE_TYPE_ID = s.LOGISTICS_RESOURCE_TYPE_ID
    LEFT JOIN dbo.SALES_QUOTATION_VER_LIN l
        ON  l.QUOTATION_VER_LIN_ID = s.QUOTATION_VER_LIN_ID
    LEFT JOIN dbo.OPERATIONS_WORK_ORDER wo
        ON  wo.WORK_ORDER_ID = s.WORK_ORDER_ID
    LEFT JOIN dbo.PRODUCTS p
        ON  p.PRODUCTS_ID = s.PRODUCTS_ID
    OUTER APPLY (
        SELECT
            SUM(ISNULL(i.STOCK_QUANTITY, 0)) AS StockQuantity,
            SUM(ISNULL(i.AVAILABLE_QUANTITY, i.STOCK_QUANTITY)) AS AvailableQuantity
        FROM dbo.INVENTORY_STOCK i
        WHERE i.BUSINESS_ID = s.BUSINESS_ID
          AND i.PRODUCTS_ID = s.PRODUCTS_ID
          AND i.STATUS = 1
    ) stock
    WHERE s.BUSINESS_ID = @BusinessId
      AND s.QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId;
END;
GO

-- ============================================================
-- 10. SP_WS_CREATE_LOGISTICS_REQUEST_FROM_SELECTED_SUGGESTIONS
-- Convierte sugerencias seleccionadas en solicitud logistica.
-- Permite detalles descriptivos con PRODUCTS_ID NULL.
-- La transaccion la maneja el backend.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_CREATE_LOGISTICS_REQUEST_FROM_SELECTED_SUGGESTIONS
    @BusinessId             BIGINT,
    @QuotationId            BIGINT        = NULL,
    @QuotationVerId         BIGINT        = NULL,
    @WorkOrderId            BIGINT        = NULL,
    @SuggestionIdsCsv       NVARCHAR(MAX) = NULL,
    @Observation            NVARCHAR(500) = NULL,
    @OfficeObservation      NVARCHAR(500) = NULL,
    @UserId                 BIGINT,
    @LogisticsRequestId     BIGINT        OUTPUT,
    @DetailCount            INT           OUTPUT,
    @COutput                INT           OUTPUT,
    @SOutput                NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @LogisticsRequestId = 0;
    SET @DetailCount = 0;

    DECLARE @SourceTypeId BIGINT;
    DECLARE @RequestStatusId BIGINT;
    DECLARE @DetailStatusId BIGINT;
    DECLARE @OpporId BIGINT;
    DECLARE @RequestNumber NVARCHAR(50) = CONCAT('LR-', CONVERT(CHAR(8), GETDATE(), 112), '-', LEFT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 16));

    DROP TABLE IF EXISTS #RequestedSuggestionIds;
    DROP TABLE IF EXISTS #SelectedSuggestions;

    CREATE TABLE #RequestedSuggestionIds
    (
        SUGGESTION_ID BIGINT NOT NULL PRIMARY KEY
    );

    IF NULLIF(LTRIM(RTRIM(ISNULL(@SuggestionIdsCsv, ''))), '') IS NOT NULL
    BEGIN
        INSERT INTO #RequestedSuggestionIds (SUGGESTION_ID)
        SELECT DISTINCT TRY_CONVERT(BIGINT, value)
        FROM STRING_SPLIT(@SuggestionIdsCsv, ',')
        WHERE TRY_CONVERT(BIGINT, value) IS NOT NULL
          AND TRY_CONVERT(BIGINT, value) > 0;
    END;

    SELECT @SourceTypeId = LOGISTICS_REQUEST_SOURCE_TYPE_ID
    FROM dbo.LOGISTICS_REQUEST_SOURCE_TYPE
    WHERE CODE = 'SYSTEM_SUGGESTION' AND STATUS = 1;

    SELECT @RequestStatusId = LOGISTICS_REQUEST_STATUS_ID
    FROM dbo.LOGISTICS_REQUEST_STATUS
    WHERE CODE = 'OFFICE_APPROVED' AND STATUS = 1;

    SELECT @DetailStatusId = LOGISTICS_REQUEST_DETAIL_STATUS_ID
    FROM dbo.LOGISTICS_REQUEST_DETAIL_STATUS
    WHERE CODE = 'APPROVED' AND STATUS = 1;

    IF @SourceTypeId IS NULL OR @RequestStatusId IS NULL OR @DetailStatusId IS NULL
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'No existe la configuracion de estados para crear la solicitud logistica.';
        RETURN;
    END;

    IF @RequestStatusId IS NULL
        SELECT @RequestStatusId = LOGISTICS_REQUEST_STATUS_ID
        FROM dbo.LOGISTICS_REQUEST_STATUS
        WHERE CODE = 'DRAFT' AND STATUS = 1;

    IF @QuotationId IS NULL AND NOT EXISTS (SELECT 1 FROM #RequestedSuggestionIds) AND @WorkOrderId IS NULL
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'Debe informar cotizacion, orden de trabajo o una lista de sugerencias.';
        RETURN;
    END;

    IF @QuotationId IS NOT NULL AND NOT EXISTS (
        SELECT 1
        FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La cotizacion no existe o no pertenece a la empresa.';
        RETURN;
    END;

    IF @QuotationVerId IS NOT NULL AND @QuotationId IS NOT NULL AND NOT EXISTS (
        SELECT 1
        FROM dbo.SALES_QUOTATION_VER_HDR
        WHERE QUOTATION_VER_ID = @QuotationVerId
          AND QUOTATION_ID = @QuotationId
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La version de cotizacion no existe.';
        RETURN;
    END;

    IF @WorkOrderId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
        )
        BEGIN
            SET @COutput = 0;
            SET @SOutput = 'La orden de trabajo seleccionada no existe o no pertenece a la empresa.';
            RETURN;
        END;

        IF EXISTS (
            SELECT 1
            FROM dbo.OPERATIONS_WORK_ORDER
            WHERE WORK_ORDER_ID = @WorkOrderId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = '1'
              AND ISNULL(NEED_LOGISTICS, 0) = 0
        )
        BEGIN
            SET @COutput = 0;
            SET @SOutput = 'La orden de trabajo seleccionada no requiere logistica.';
            RETURN;
        END;
    END;

    IF @QuotationId IS NOT NULL
        SELECT @OpporId = OPPOR_ID
        FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId
          AND BUSINESS_ID = @BusinessId;

    SELECT
        s.QUOTATION_LOGISTICS_SUGGESTION_ID,
        s.BUSINESS_ID,
        s.QUOTATION_ID,
        s.QUOTATION_VER_ID,
        s.QUOTATION_VER_LIN_ID,
        COALESCE(s.WORK_ORDER_ID, @WorkOrderId) AS WORK_ORDER_ID,
        s.PRODUCTS_ID,
        s.DESCRIPTION,
        s.LOGISTICS_RESOURCE_TYPE_ID,
        s.APPROVED_QUANTITY,
        s.PENDING_TO_REQUEST_QUANTITY,
        s.OFFICE_OBSERVATION
    INTO #SelectedSuggestions
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
    WHERE s.BUSINESS_ID = @BusinessId
      AND (@QuotationId IS NULL OR s.QUOTATION_ID = @QuotationId)
      AND (@QuotationVerId IS NULL OR s.QUOTATION_VER_ID = @QuotationVerId)
      AND (
            @WorkOrderId IS NULL
            OR s.WORK_ORDER_ID = @WorkOrderId
            OR s.WORK_ORDER_ID IS NULL
          )
      AND (
            NOT EXISTS (SELECT 1 FROM #RequestedSuggestionIds)
            OR EXISTS (
                SELECT 1
                FROM #RequestedSuggestionIds ids
                WHERE ids.SUGGESTION_ID = s.QUOTATION_LOGISTICS_SUGGESTION_ID
            )
          )
      AND s.STATUS = 1
      AND s.IS_SELECTED = 1
      AND ISNULL(s.IS_FULLY_REQUESTED, 0) = 0
      AND ISNULL(s.APPROVED_QUANTITY, 0) > 0
      AND ISNULL(s.PENDING_TO_REQUEST_QUANTITY, s.APPROVED_QUANTITY) > 0;

    CREATE UNIQUE INDEX UIX_SELECTED_SUGGESTIONS ON #SelectedSuggestions (QUOTATION_LOGISTICS_SUGGESTION_ID);

    IF EXISTS (
        SELECT 1
        FROM #SelectedSuggestions
        WHERE APPROVED_QUANTITY > PENDING_TO_REQUEST_QUANTITY
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La cantidad aprobada no puede superar la cantidad pendiente por solicitar.';
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM #SelectedSuggestions
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'No existen sugerencias seleccionadas pendientes para crear la solicitud logistica.';
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO dbo.LOGISTICS_REQUEST
        (
            BUSINESS_ID, REQUEST_NUMBER, OPPOR_ID, QUOTATION_ID, QUOTATION_VER_ID, WORK_ORDER_ID,
            LOGISTICS_REQUEST_SOURCE_TYPE_ID, LOGISTICS_REQUEST_STATUS_ID,
            REQUESTED_BY, REQUEST_DATE, REVIEWED_BY, REVIEWED_DATE,
            OBSERVATION, OFFICE_OBSERVATION, CREATE_USER, STATUS
        )
        VALUES
        (
            @BusinessId, @RequestNumber, @OpporId, @QuotationId, @QuotationVerId, @WorkOrderId,
            @SourceTypeId, @RequestStatusId,
            @UserId, GETDATE(), @UserId, GETDATE(),
            @Observation, @OfficeObservation, @UserId, 1
        );

        SET @LogisticsRequestId = SCOPE_IDENTITY();

        INSERT INTO dbo.LOGISTICS_REQUEST_DETAIL
        (
            BUSINESS_ID, LOGISTICS_REQUEST_ID, WORK_ORDER_ID, PRODUCTS_ID, DESCRIPTION,
            LOGISTICS_RESOURCE_TYPE_ID, REQUESTED_QUANTITY, APPROVED_QUANTITY,
            LOGISTICS_REQUEST_DETAIL_STATUS_ID, SOURCE_QUOTATION_VER_LIN_ID,
            SOURCE_SUGGESTION_ID, OBSERVATION, CREATE_USER, STATUS
        )
        SELECT
            s.BUSINESS_ID,
            @LogisticsRequestId,
            s.WORK_ORDER_ID,
            s.PRODUCTS_ID,
            s.DESCRIPTION,
            s.LOGISTICS_RESOURCE_TYPE_ID,
            s.APPROVED_QUANTITY,
            s.APPROVED_QUANTITY,
            @DetailStatusId,
            s.QUOTATION_VER_LIN_ID,
            s.QUOTATION_LOGISTICS_SUGGESTION_ID,
            s.OFFICE_OBSERVATION,
            @UserId,
            1
        FROM #SelectedSuggestions s;

        SET @DetailCount = @@ROWCOUNT;

        UPDATE s
        SET
            s.WORK_ORDER_ID = ss.WORK_ORDER_ID,
            s.ALREADY_REQUESTED_QUANTITY = ISNULL(req.ALREADY_REQUESTED_QUANTITY, 0),
            s.PENDING_TO_REQUEST_QUANTITY =
                CASE
                    WHEN ISNULL(s.SUGGESTED_QUANTITY_BASE, s.SUGGESTED_QUANTITY) - ISNULL(req.ALREADY_REQUESTED_QUANTITY, 0) < 0 THEN 0
                    ELSE ISNULL(s.SUGGESTED_QUANTITY_BASE, s.SUGGESTED_QUANTITY) - ISNULL(req.ALREADY_REQUESTED_QUANTITY, 0)
                END,
            s.EXCESS_REQUESTED_QUANTITY =
                CASE
                    WHEN ISNULL(req.ALREADY_REQUESTED_QUANTITY, 0) > ISNULL(s.SUGGESTED_QUANTITY_BASE, s.SUGGESTED_QUANTITY)
                        THEN ISNULL(req.ALREADY_REQUESTED_QUANTITY, 0) - ISNULL(s.SUGGESTED_QUANTITY_BASE, s.SUGGESTED_QUANTITY)
                    ELSE 0
                END,
            s.IS_FULLY_REQUESTED =
                CASE WHEN ISNULL(s.SUGGESTED_QUANTITY_BASE, s.SUGGESTED_QUANTITY) - ISNULL(req.ALREADY_REQUESTED_QUANTITY, 0) <= 0 THEN 1 ELSE 0 END,
            s.IS_SELECTED = 0,
            s.UPDATE_USER = @UserId,
            s.UPDATE_DATE = GETDATE()
        FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
        INNER JOIN #SelectedSuggestions ss
            ON ss.QUOTATION_LOGISTICS_SUGGESTION_ID = s.QUOTATION_LOGISTICS_SUGGESTION_ID
        OUTER APPLY (
            SELECT SUM(d.APPROVED_QUANTITY - d.CANCELLED_QUANTITY) AS ALREADY_REQUESTED_QUANTITY
            FROM dbo.LOGISTICS_REQUEST_DETAIL d
            INNER JOIN dbo.LOGISTICS_REQUEST h
                ON h.LOGISTICS_REQUEST_ID = d.LOGISTICS_REQUEST_ID
               AND h.STATUS = 1
            INNER JOIN dbo.LOGISTICS_REQUEST_STATUS hrs
                ON hrs.LOGISTICS_REQUEST_STATUS_ID = h.LOGISTICS_REQUEST_STATUS_ID
               AND hrs.CODE NOT IN ('REJECTED', 'CANCELLED')
            INNER JOIN dbo.LOGISTICS_REQUEST_DETAIL_STATUS drs
                ON drs.LOGISTICS_REQUEST_DETAIL_STATUS_ID = d.LOGISTICS_REQUEST_DETAIL_STATUS_ID
               AND drs.CODE NOT IN ('REJECTED', 'CANCELLED')
            WHERE d.BUSINESS_ID = @BusinessId
              AND d.SOURCE_SUGGESTION_ID = s.QUOTATION_LOGISTICS_SUGGESTION_ID
              AND d.STATUS = 1
        ) req;

        SET @COutput = 1;
        SET @SOutput = 'Solicitud logistica creada desde sugerencias seleccionadas.';

        DROP TABLE IF EXISTS #RequestedSuggestionIds;
        DROP TABLE IF EXISTS #SelectedSuggestions;
    END TRY
    BEGIN CATCH
        DROP TABLE IF EXISTS #RequestedSuggestionIds;
        DROP TABLE IF EXISTS #SelectedSuggestions;

        SET @LogisticsRequestId = 0;
        SET @DetailCount = 0;
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH;
END;
GO

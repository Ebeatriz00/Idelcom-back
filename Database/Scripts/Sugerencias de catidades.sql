CREATE TABLE dbo.LOGISTICS_RESOURCE_TYPE
(
    LOGISTICS_RESOURCE_TYPE_ID BIGINT IDENTITY(1,1) NOT NULL,
    CODE VARCHAR(30) NOT NULL,
    DESCRIPTION NVARCHAR(100) NOT NULL,

    CREATE_USER BIGINT NOT NULL,
    CREATE_DATE DATETIME NOT NULL CONSTRAINT DF_LOGISTICS_RESOURCE_TYPE_CREATE_DATE DEFAULT(GETDATE()),
    UPDATE_USER BIGINT NULL,
    UPDATE_DATE DATETIME NULL,
    STATUS BIT NOT NULL CONSTRAINT DF_LOGISTICS_RESOURCE_TYPE_STATUS DEFAULT(1),

    CONSTRAINT PK_LOGISTICS_RESOURCE_TYPE
        PRIMARY KEY (LOGISTICS_RESOURCE_TYPE_ID),

    CONSTRAINT UQ_LOGISTICS_RESOURCE_TYPE_CODE
        UNIQUE (CODE)
);

INSERT INTO dbo.LOGISTICS_RESOURCE_TYPE
(
    CODE,
    DESCRIPTION,
    CREATE_USER,
    STATUS
)
VALUES
('MATERIAL',    'Material',    1, 1),
('TOOL',        'Herramienta', 1, 1),
('EQUIPMENT',   'Equipo',      1, 1),
('EPP',         'EPP',         1, 1),
('CONSUMABLE',  'Consumible',  1, 1),
('SERVICE',     'Servicio',    1, 1);

CREATE TABLE dbo.LOGISTICS_SUGGESTION_RULE
(
    LOGISTICS_SUGGESTION_RULE_ID BIGINT IDENTITY(1,1) NOT NULL,
    BUSINESS_ID BIGINT NOT NULL,

    RULE_NAME NVARCHAR(150) NOT NULL,

    -- Filtros de aplicación
    KEYWORD NVARCHAR(200) NULL,
    PRODUCTS_TYPE_ID BIGINT NULL,
    SYSTEM_ID BIGINT NULL,
    LINE_TYPE VARCHAR(10) NULL,

    -- Recurso sugerido
    LOGISTICS_RESOURCE_TYPE_ID BIGINT NOT NULL,
    SUGGESTED_PRODUCTS_ID BIGINT NULL,
    SUGGESTED_DESCRIPTION NVARCHAR(500) NOT NULL,

    DEFAULT_QUANTITY DECIMAL(18,4) NOT NULL CONSTRAINT DF_LOGISTICS_SUGGESTION_RULE_DEFAULT_QTY DEFAULT(1),
    QUANTITY_FACTOR DECIMAL(18,4) NOT NULL CONSTRAINT DF_LOGISTICS_SUGGESTION_RULE_FACTOR DEFAULT(0),

    IS_REQUIRED BIT NOT NULL CONSTRAINT DF_LOGISTICS_SUGGESTION_RULE_REQUIRED DEFAULT(0),
    REQUIRES_REVIEW BIT NOT NULL CONSTRAINT DF_LOGISTICS_SUGGESTION_RULE_REVIEW DEFAULT(1),

    OBSERVATION NVARCHAR(500) NULL,

    CREATE_USER BIGINT NOT NULL,
    CREATE_DATE DATETIME NOT NULL CONSTRAINT DF_LOGISTICS_SUGGESTION_RULE_CREATE_DATE DEFAULT(GETDATE()),
    UPDATE_USER BIGINT NULL,
    UPDATE_DATE DATETIME NULL,
    STATUS BIT NOT NULL CONSTRAINT DF_LOGISTICS_SUGGESTION_RULE_STATUS DEFAULT(1),

    CONSTRAINT PK_LOGISTICS_SUGGESTION_RULE
        PRIMARY KEY (LOGISTICS_SUGGESTION_RULE_ID),

    CONSTRAINT FK_LOGISTICS_SUGGESTION_RULE_BUSINESS
        FOREIGN KEY (BUSINESS_ID) REFERENCES dbo.BUSINESS(ID),

    CONSTRAINT FK_LOGISTICS_SUGGESTION_RULE_RESOURCE_TYPE
        FOREIGN KEY (LOGISTICS_RESOURCE_TYPE_ID)
        REFERENCES dbo.LOGISTICS_RESOURCE_TYPE(LOGISTICS_RESOURCE_TYPE_ID),

    CONSTRAINT FK_LOGISTICS_SUGGESTION_RULE_PRODUCT
        FOREIGN KEY (SUGGESTED_PRODUCTS_ID)
        REFERENCES dbo.PRODUCTS(PRODUCTS_ID)
);


CREATE TABLE dbo.QUOTATION_LOGISTICS_SUGGESTION
(
    QUOTATION_LOGISTICS_SUGGESTION_ID BIGINT IDENTITY(1,1) NOT NULL,
    BUSINESS_ID BIGINT NOT NULL,

    QUOTATION_ID BIGINT NOT NULL,
    QUOTATION_VER_ID BIGINT NOT NULL,
    QUOTATION_VER_LIN_ID BIGINT NULL,

    LOGISTICS_SUGGESTION_RULE_ID BIGINT NULL,
    LOGISTICS_RESOURCE_TYPE_ID BIGINT NOT NULL,

    PRODUCTS_ID BIGINT NULL,
    DESCRIPTION NVARCHAR(500) NOT NULL,

    SUGGESTED_QUANTITY DECIMAL(18,4) NOT NULL,
    APPROVED_QUANTITY DECIMAL(18,4) NULL,

    IS_SELECTED BIT NOT NULL CONSTRAINT DF_QUOTATION_LOGISTICS_SUGGESTION_SELECTED DEFAULT(1),
    IS_MANUAL BIT NOT NULL CONSTRAINT DF_QUOTATION_LOGISTICS_SUGGESTION_MANUAL DEFAULT(0),
    IS_DUPLICATED BIT NOT NULL CONSTRAINT DF_QUOTATION_LOGISTICS_SUGGESTION_DUP DEFAULT(0),

    SUGGESTION_REASON NVARCHAR(500) NULL,
    OFFICE_OBSERVATION NVARCHAR(500) NULL,

    REVIEWED_BY BIGINT NULL,
    REVIEWED_DATE DATETIME NULL,

    CREATE_USER BIGINT NOT NULL,
    CREATE_DATE DATETIME NOT NULL CONSTRAINT DF_QUOTATION_LOGISTICS_SUGGESTION_CREATE_DATE DEFAULT(GETDATE()),
    UPDATE_USER BIGINT NULL,
    UPDATE_DATE DATETIME NULL,
    STATUS BIT NOT NULL CONSTRAINT DF_QUOTATION_LOGISTICS_SUGGESTION_STATUS DEFAULT(1),

    CONSTRAINT PK_QUOTATION_LOGISTICS_SUGGESTION
        PRIMARY KEY (QUOTATION_LOGISTICS_SUGGESTION_ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_BUSINESS
        FOREIGN KEY (BUSINESS_ID) REFERENCES dbo.BUSINESS(ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_QUOTATION
        FOREIGN KEY (QUOTATION_ID) REFERENCES dbo.SALES_QUOTATION_HDR(QUOTATION_ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_QUOTATION_VER
        FOREIGN KEY (QUOTATION_VER_ID) REFERENCES dbo.SALES_QUOTATION_VER_HDR(QUOTATION_VER_ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_QUOTATION_LINE
        FOREIGN KEY (QUOTATION_VER_LIN_ID) REFERENCES dbo.SALES_QUOTATION_VER_LIN(QUOTATION_VER_LIN_ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_RULE
        FOREIGN KEY (LOGISTICS_SUGGESTION_RULE_ID)
        REFERENCES dbo.LOGISTICS_SUGGESTION_RULE(LOGISTICS_SUGGESTION_RULE_ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_RESOURCE_TYPE
        FOREIGN KEY (LOGISTICS_RESOURCE_TYPE_ID)
        REFERENCES dbo.LOGISTICS_RESOURCE_TYPE(LOGISTICS_RESOURCE_TYPE_ID),

    CONSTRAINT FK_QUOTATION_LOGISTICS_SUGGESTION_PRODUCT
        FOREIGN KEY (PRODUCTS_ID)
        REFERENCES dbo.PRODUCTS(PRODUCTS_ID)
);

CREATE INDEX IX_LOGISTICS_SUGGESTION_RULE_BUSINESS
ON dbo.LOGISTICS_SUGGESTION_RULE
(
    BUSINESS_ID,
    STATUS
);

CREATE INDEX IX_LOGISTICS_SUGGESTION_RULE_KEYWORD
ON dbo.LOGISTICS_SUGGESTION_RULE
(
    BUSINESS_ID,
    KEYWORD,
    STATUS
);

CREATE INDEX IX_QUOTATION_LOGISTICS_SUGGESTION_QUOTATION
ON dbo.QUOTATION_LOGISTICS_SUGGESTION
(
    BUSINESS_ID,
    QUOTATION_ID,
    QUOTATION_VER_ID,
    STATUS
);

CREATE INDEX IX_QUOTATION_LOGISTICS_SUGGESTION_LINE
ON dbo.QUOTATION_LOGISTICS_SUGGESTION
(
    BUSINESS_ID,
    QUOTATION_VER_LIN_ID,
    STATUS
);

CREATE OR ALTER PROCEDURE dbo.SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS
    @BusinessId BIGINT,
    @QuotationId BIGINT,
    @QuotationVerId BIGINT = NULL,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ResolvedQuotationVerId BIGINT;
    DECLARE @Created TABLE (ID BIGINT);

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SELECT
            @QuotationId AS QuotationId,
            CAST(0 AS BIGINT) AS QuotationVerId,
            0 AS CreatedCount,
            0 AS ExistingCount,
            0 AS TotalActiveCount,
            CAST('La cotizacion no existe o no pertenece a la empresa.' AS NVARCHAR(500)) AS Message;
        RETURN;
    END;

    SELECT @ResolvedQuotationVerId = @QuotationVerId;

    IF @ResolvedQuotationVerId IS NULL
    BEGIN
        SELECT @ResolvedQuotationVerId = SELECTED_QUOTATION_VER_ID
        FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1;
    END;

    IF @ResolvedQuotationVerId IS NULL
    BEGIN
        SELECT TOP (1) @ResolvedQuotationVerId = QUOTATION_VER_ID
        FROM dbo.SALES_QUOTATION_VER_HDR
        WHERE QUOTATION_ID = @QuotationId
        ORDER BY QUOTATION_VER_ID DESC;
    END;

    IF @ResolvedQuotationVerId IS NULL
       OR NOT EXISTS (
            SELECT 1
            FROM dbo.SALES_QUOTATION_VER_HDR
            WHERE QUOTATION_VER_ID = @ResolvedQuotationVerId
              AND QUOTATION_ID = @QuotationId
       )
    BEGIN
        SELECT
            @QuotationId AS QuotationId,
            ISNULL(@ResolvedQuotationVerId, 0) AS QuotationVerId,
            0 AS CreatedCount,
            0 AS ExistingCount,
            0 AS TotalActiveCount,
            CAST('No existe una version seleccionada o activa para la cotizacion.' AS NVARCHAR(500)) AS Message;
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.SALES_QUOTATION_VER_LIN
        WHERE QUOTATION_VER_ID = @ResolvedQuotationVerId
    )
    BEGIN
        SELECT
            @QuotationId AS QuotationId,
            @ResolvedQuotationVerId AS QuotationVerId,
            0 AS CreatedCount,
            0 AS ExistingCount,
            0 AS TotalActiveCount,
            CAST('La version de cotizacion no tiene lineas registradas.' AS NVARCHAR(500)) AS Message;
        RETURN;
    END;

    ;WITH CandidateSuggestions AS
    (
        SELECT
            @QuotationId AS QUOTATION_ID,
            l.QUOTATION_VER_ID,
            l.QUOTATION_VER_LIN_ID,
            r.LOGISTICS_SUGGESTION_RULE_ID,
            r.LOGISTICS_RESOURCE_TYPE_ID,
            r.SUGGESTED_PRODUCTS_ID AS PRODUCTS_ID,
            r.SUGGESTED_DESCRIPTION AS DESCRIPTION,
            CAST(
                CASE
                    WHEN ISNULL(r.QUANTITY_FACTOR, 0) > 0 AND l.QTY IS NOT NULL THEN l.QTY * r.QUANTITY_FACTOR
                    ELSE r.DEFAULT_QUANTITY
                END AS DECIMAL(18,4)
            ) AS SUGGESTED_QUANTITY,
            CONCAT(
                'Regla: ', r.RULE_NAME,
                CASE WHEN r.KEYWORD IS NOT NULL THEN CONCAT(' | Keyword: ', r.KEYWORD) ELSE '' END,
                CASE WHEN r.PRODUCTS_TYPE_ID IS NOT NULL THEN ' | Tipo producto' ELSE '' END,
                CASE WHEN r.SYSTEM_ID IS NOT NULL THEN ' | Sistema' ELSE '' END,
                CASE WHEN r.LINE_TYPE IS NOT NULL THEN CONCAT(' | Linea: ', r.LINE_TYPE) ELSE '' END
            ) AS SUGGESTION_REASON
        FROM dbo.SALES_QUOTATION_VER_LIN l
        INNER JOIN dbo.LOGISTICS_SUGGESTION_RULE r
            ON r.BUSINESS_ID = @BusinessId
           AND r.STATUS = 1
        WHERE l.QUOTATION_VER_ID = @ResolvedQuotationVerId
          AND UPPER(ISNULL(l.LINE_TYPE, '')) = 'ITEM'
          AND (
                r.KEYWORD IS NOT NULL
                OR r.PRODUCTS_TYPE_ID IS NOT NULL
                OR r.SYSTEM_ID IS NOT NULL
                OR r.LINE_TYPE IS NOT NULL
          )
          AND (r.KEYWORD IS NULL OR UPPER(ISNULL(l.DESCRIPTION, '')) LIKE '%' + UPPER(r.KEYWORD) + '%')
          AND (r.PRODUCTS_TYPE_ID IS NULL OR r.PRODUCTS_TYPE_ID = l.PRODUCTS_TYPE_ID)
          AND (r.SYSTEM_ID IS NULL OR r.SYSTEM_ID = l.SYSTEM_ID)
          AND (r.LINE_TYPE IS NULL OR UPPER(r.LINE_TYPE) = UPPER(ISNULL(l.LINE_TYPE, '')))
    ),
    NewSuggestions AS
    (
        SELECT c.*
        FROM CandidateSuggestions c
        WHERE NOT EXISTS (
            SELECT 1
            FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
            WHERE s.BUSINESS_ID = @BusinessId
              AND s.QUOTATION_ID = c.QUOTATION_ID
              AND s.QUOTATION_VER_ID = c.QUOTATION_VER_ID
              AND ISNULL(s.QUOTATION_VER_LIN_ID, 0) = ISNULL(c.QUOTATION_VER_LIN_ID, 0)
              AND ISNULL(s.LOGISTICS_SUGGESTION_RULE_ID, 0) = ISNULL(c.LOGISTICS_SUGGESTION_RULE_ID, 0)
              AND (
                    ISNULL(s.PRODUCTS_ID, 0) = ISNULL(c.PRODUCTS_ID, 0)
                    OR UPPER(s.DESCRIPTION) = UPPER(c.DESCRIPTION)
              )
              AND s.STATUS = 1
        )
    )
    INSERT INTO dbo.QUOTATION_LOGISTICS_SUGGESTION
    (
        BUSINESS_ID,
        QUOTATION_ID,
        QUOTATION_VER_ID,
        QUOTATION_VER_LIN_ID,
        LOGISTICS_SUGGESTION_RULE_ID,
        LOGISTICS_RESOURCE_TYPE_ID,
        PRODUCTS_ID,
        DESCRIPTION,
        SUGGESTED_QUANTITY,
        APPROVED_QUANTITY,
        IS_SELECTED,
        IS_MANUAL,
        IS_DUPLICATED,
        SUGGESTION_REASON,
        CREATE_USER,
        STATUS
    )
    OUTPUT INSERTED.QUOTATION_LOGISTICS_SUGGESTION_ID INTO @Created(ID)
    SELECT
        @BusinessId,
        QUOTATION_ID,
        QUOTATION_VER_ID,
        QUOTATION_VER_LIN_ID,
        LOGISTICS_SUGGESTION_RULE_ID,
        LOGISTICS_RESOURCE_TYPE_ID,
        PRODUCTS_ID,
        DESCRIPTION,
        SUGGESTED_QUANTITY,
        SUGGESTED_QUANTITY,
        1,
        0,
        0,
        SUGGESTION_REASON,
        @UserId,
        1
    FROM NewSuggestions;

    ;WITH CandidateSuggestions AS
    (
        SELECT
            @QuotationId AS QUOTATION_ID,
            l.QUOTATION_VER_ID,
            l.QUOTATION_VER_LIN_ID,
            r.LOGISTICS_SUGGESTION_RULE_ID,
            r.SUGGESTED_PRODUCTS_ID AS PRODUCTS_ID,
            r.SUGGESTED_DESCRIPTION AS DESCRIPTION
        FROM dbo.SALES_QUOTATION_VER_LIN l
        INNER JOIN dbo.LOGISTICS_SUGGESTION_RULE r
            ON r.BUSINESS_ID = @BusinessId
           AND r.STATUS = 1
        WHERE l.QUOTATION_VER_ID = @ResolvedQuotationVerId
          AND UPPER(ISNULL(l.LINE_TYPE, '')) = 'ITEM'
          AND (
                r.KEYWORD IS NOT NULL
                OR r.PRODUCTS_TYPE_ID IS NOT NULL
                OR r.SYSTEM_ID IS NOT NULL
                OR r.LINE_TYPE IS NOT NULL
          )
          AND (r.KEYWORD IS NULL OR UPPER(ISNULL(l.DESCRIPTION, '')) LIKE '%' + UPPER(r.KEYWORD) + '%')
          AND (r.PRODUCTS_TYPE_ID IS NULL OR r.PRODUCTS_TYPE_ID = l.PRODUCTS_TYPE_ID)
          AND (r.SYSTEM_ID IS NULL OR r.SYSTEM_ID = l.SYSTEM_ID)
          AND (r.LINE_TYPE IS NULL OR UPPER(r.LINE_TYPE) = UPPER(ISNULL(l.LINE_TYPE, '')))
    )
    SELECT
        @QuotationId AS QuotationId,
        @ResolvedQuotationVerId AS QuotationVerId,
        (SELECT COUNT(1) FROM @Created) AS CreatedCount,
        (
            SELECT COUNT(1)
            FROM CandidateSuggestions c
            WHERE EXISTS (
                SELECT 1
                FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
                WHERE s.BUSINESS_ID = @BusinessId
                  AND s.QUOTATION_ID = c.QUOTATION_ID
                  AND s.QUOTATION_VER_ID = c.QUOTATION_VER_ID
                  AND ISNULL(s.QUOTATION_VER_LIN_ID, 0) = ISNULL(c.QUOTATION_VER_LIN_ID, 0)
                  AND ISNULL(s.LOGISTICS_SUGGESTION_RULE_ID, 0) = ISNULL(c.LOGISTICS_SUGGESTION_RULE_ID, 0)
                  AND (
                        ISNULL(s.PRODUCTS_ID, 0) = ISNULL(c.PRODUCTS_ID, 0)
                        OR UPPER(s.DESCRIPTION) = UPPER(c.DESCRIPTION)
                  )
                  AND s.STATUS = 1
                  AND NOT EXISTS (SELECT 1 FROM @Created cr WHERE cr.ID = s.QUOTATION_LOGISTICS_SUGGESTION_ID)
            )
        ) AS ExistingCount,
        (
            SELECT COUNT(1)
            FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
            WHERE BUSINESS_ID = @BusinessId
              AND QUOTATION_ID = @QuotationId
              AND QUOTATION_VER_ID = @ResolvedQuotationVerId
              AND STATUS = 1
        ) AS TotalActiveCount,
        CAST(
            CASE
                WHEN (SELECT COUNT(1) FROM @Created) = 0 THEN 'No se crearon nuevas sugerencias. Puede que no existan reglas aplicables o que ya hayan sido generadas.'
                ELSE 'Sugerencias logisticas generadas correctamente.'
            END AS NVARCHAR(500)
        ) AS Message;
END;

GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_LIST_QUOTATION_LOGISTICS_SUGGESTIONS
    @BusinessId BIGINT,
    @QuotationId BIGINT,
    @QuotationVerId BIGINT = NULL,
    @ResourceTypeId BIGINT = NULL,
    @OnlySelected BIT = NULL,
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.QUOTATION_LOGISTICS_SUGGESTION_ID AS QuotationLogisticsSuggestionId,
        s.BUSINESS_ID AS BusinessId,
        s.QUOTATION_ID AS QuotationId,
        s.QUOTATION_VER_ID AS QuotationVerId,
        s.QUOTATION_VER_LIN_ID AS QuotationVerLinId,
        l.DESCRIPTION AS LineDescription,
        l.QTY AS LineQty,
        s.LOGISTICS_SUGGESTION_RULE_ID AS LogisticsSuggestionRuleId,
        s.LOGISTICS_RESOURCE_TYPE_ID AS LogisticsResourceTypeId,
        rt.CODE AS ResourceTypeCode,
        rt.DESCRIPTION AS ResourceTypeDescription,
        s.PRODUCTS_ID AS ProductsId,
        p.SKU AS ProductCode,
        p.DESCRIPTION AS ProductName,
        s.DESCRIPTION AS Description,
        s.SUGGESTED_QUANTITY AS SuggestedQuantity,
        s.APPROVED_QUANTITY AS ApprovedQuantity,
        s.IS_SELECTED AS IsSelected,
        s.IS_MANUAL AS IsManual,
        s.IS_DUPLICATED AS IsDuplicated,
        s.SUGGESTION_REASON AS SuggestionReason,
        s.OFFICE_OBSERVATION AS OfficeObservation,
        s.REVIEWED_BY AS ReviewedBy,
        s.REVIEWED_DATE AS ReviewedDate,
        s.STATUS AS Status
    FROM dbo.QUOTATION_LOGISTICS_SUGGESTION s
    INNER JOIN dbo.LOGISTICS_RESOURCE_TYPE rt
        ON rt.LOGISTICS_RESOURCE_TYPE_ID = s.LOGISTICS_RESOURCE_TYPE_ID
    LEFT JOIN dbo.SALES_QUOTATION_VER_LIN l
        ON l.QUOTATION_VER_LIN_ID = s.QUOTATION_VER_LIN_ID
    LEFT JOIN dbo.PRODUCTS p
        ON p.PRODUCTS_ID = s.PRODUCTS_ID
    WHERE s.BUSINESS_ID = @BusinessId
      AND s.QUOTATION_ID = @QuotationId
      AND (@QuotationVerId IS NULL OR s.QUOTATION_VER_ID = @QuotationVerId)
      AND (@ResourceTypeId IS NULL OR s.LOGISTICS_RESOURCE_TYPE_ID = @ResourceTypeId)
      AND (@OnlySelected IS NULL OR s.IS_SELECTED = @OnlySelected)
      AND s.STATUS = 1
      AND (
            @Search IS NULL
            OR s.DESCRIPTION LIKE '%' + @Search + '%'
            OR ISNULL(p.DESCRIPTION, '') LIKE '%' + @Search + '%'
            OR ISNULL(l.DESCRIPTION, '') LIKE '%' + @Search + '%'
      )
    ORDER BY rt.CODE, s.DESCRIPTION;
END;

GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION
    @SuggestionId BIGINT,
    @BusinessId BIGINT,
    @IsSelected BIT,
    @ApprovedQuantity DECIMAL(18,4),
    @OfficeObservation NVARCHAR(500) = NULL,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
        WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
          AND BUSINESS_ID = @BusinessId
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La sugerencia no existe.';
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.QUOTATION_LOGISTICS_SUGGESTION
        WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'No se puede editar una sugerencia desactivada.';
        RETURN;
    END;

    IF @ApprovedQuantity < 0 OR (@IsSelected = 1 AND @ApprovedQuantity <= 0)
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La cantidad aprobada no es valida.';
        RETURN;
    END;

    UPDATE dbo.QUOTATION_LOGISTICS_SUGGESTION
    SET IS_SELECTED = @IsSelected,
        APPROVED_QUANTITY = @ApprovedQuantity,
        OFFICE_OBSERVATION = @OfficeObservation,
        REVIEWED_BY = @UserId,
        REVIEWED_DATE = GETDATE(),
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
      AND BUSINESS_ID = @BusinessId
      AND STATUS = 1;

    SET @COutput = 1;
    SET @SOutput = 'Sugerencia actualizada correctamente.';
END;

GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_ADD_MANUAL_QUOTATION_LOGISTICS_SUGGESTION
    @BusinessId BIGINT,
    @QuotationId BIGINT,
    @QuotationVerId BIGINT,
    @LogisticsResourceTypeId BIGINT,
    @ProductsId BIGINT = NULL,
    @Description NVARCHAR(500) = NULL,
    @SuggestedQuantity DECIMAL(18,4),
    @ApprovedQuantity DECIMAL(18,4),
    @OfficeObservation NVARCHAR(500) = NULL,
    @UserId BIGINT,
    @Id BIGINT OUTPUT,
    @COutput INT OUTPUT,
    @SOutput NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = 0;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.SALES_QUOTATION_HDR
        WHERE QUOTATION_ID = @QuotationId
          AND BUSINESS_ID = @BusinessId
          AND STATUS = 1
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La cotizacion no existe o no pertenece a la empresa.';
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.SALES_QUOTATION_VER_HDR
        WHERE QUOTATION_VER_ID = @QuotationVerId
          AND QUOTATION_ID = @QuotationId
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La version de cotizacion no existe o no esta activa.';
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.LOGISTICS_RESOURCE_TYPE
        WHERE LOGISTICS_RESOURCE_TYPE_ID = @LogisticsResourceTypeId
          AND STATUS = 1
    )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'El tipo de recurso logistico no existe o esta inactivo.';
        RETURN;
    END;

    IF @ProductsId IS NOT NULL
       AND NOT EXISTS (
            SELECT 1 FROM dbo.PRODUCTS
            WHERE PRODUCTS_ID = @ProductsId
              AND BUSINESS_ID = @BusinessId
              AND STATUS = 1
       )
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'El producto no existe o esta inactivo.';
        RETURN;
    END;

    IF @ProductsId IS NULL AND NULLIF(LTRIM(RTRIM(ISNULL(@Description, ''))), '') IS NULL
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'La descripcion es obligatoria si no se informa producto.';
        RETURN;
    END;

    IF @SuggestedQuantity <= 0 OR @ApprovedQuantity < 0
    BEGIN
        SET @COutput = 0;
        SET @SOutput = 'Las cantidades informadas no son validas.';
        RETURN;
    END;

    IF @ProductsId IS NOT NULL AND NULLIF(LTRIM(RTRIM(ISNULL(@Description, ''))), '') IS NULL
    BEGIN
        SELECT @Description = DESCRIPTION
        FROM dbo.PRODUCTS
        WHERE PRODUCTS_ID = @ProductsId;
    END;

    INSERT INTO dbo.QUOTATION_LOGISTICS_SUGGESTION
    (
        BUSINESS_ID,
        QUOTATION_ID,
        QUOTATION_VER_ID,
        QUOTATION_VER_LIN_ID,
        LOGISTICS_SUGGESTION_RULE_ID,
        LOGISTICS_RESOURCE_TYPE_ID,
        PRODUCTS_ID,
        DESCRIPTION,
        SUGGESTED_QUANTITY,
        APPROVED_QUANTITY,
        IS_SELECTED,
        IS_MANUAL,
        IS_DUPLICATED,
        SUGGESTION_REASON,
        OFFICE_OBSERVATION,
        REVIEWED_BY,
        REVIEWED_DATE,
        CREATE_USER,
        STATUS
    )
    VALUES
    (
        @BusinessId,
        @QuotationId,
        @QuotationVerId,
        NULL,
        NULL,
        @LogisticsResourceTypeId,
        @ProductsId,
        @Description,
        @SuggestedQuantity,
        @ApprovedQuantity,
        1,
        1,
        0,
        'Sugerencia manual de oficina',
        @OfficeObservation,
        @UserId,
        GETDATE(),
        @UserId,
        1
    );

    SET @Id = SCOPE_IDENTITY();
    SET @COutput = 1;
    SET @SOutput = 'Sugerencia manual agregada correctamente.';
END;

GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_DISABLE_QUOTATION_LOGISTICS_SUGGESTION
    @SuggestionId BIGINT,
    @BusinessId BIGINT,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput NVARCHAR(500) OUTPUT
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
        SET @SOutput = 'La sugerencia no existe o ya esta desactivada.';
        RETURN;
    END;

    UPDATE dbo.QUOTATION_LOGISTICS_SUGGESTION
    SET STATUS = 0,
        IS_SELECTED = 0,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE QUOTATION_LOGISTICS_SUGGESTION_ID = @SuggestionId
      AND BUSINESS_ID = @BusinessId
      AND STATUS = 1;

    SET @COutput = 1;
    SET @SOutput = 'Sugerencia desactivada correctamente.';
END;

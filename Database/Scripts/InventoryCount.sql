-- =============================================================
-- INVENTARIO FÍSICO - Tipos de tabla (TVP)
-- =============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.types WHERE name = 'InventoryCountDetailUpdateType' AND is_table_type = 1
)
BEGIN
    CREATE TYPE dbo.InventoryCountDetailUpdateType AS TABLE
    (
        INVENTORY_COUNT_DETAIL_ID BIGINT NOT NULL,
        COUNTED_QUANTITY           DECIMAL(18,4) NOT NULL,
        LOT_NUMBER                 NVARCHAR(100) NULL,
        SERIAL_NUMBER              NVARCHAR(100) NULL,
        EXPIRATION_DATE            DATE NULL,
        OBSERVATION                NVARCHAR(500) NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.types WHERE name = 'InventoryCountDetailAdjustmentType' AND is_table_type = 1
)
BEGIN
    CREATE TYPE dbo.InventoryCountDetailAdjustmentType AS TABLE
    (
        INVENTORY_COUNT_DETAIL_ID BIGINT NOT NULL,
        ADJUSTMENT_MOVEMENT_ID    BIGINT NOT NULL
    );
END
GO

-- =============================================================
-- SP_WS_GET_NEXT_INVENTORY_COUNT_NUMBER
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_NEXT_INVENTORY_COUNT_NUMBER
    @BusinessId  BIGINT,
    @CountNumber VARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Next INT;

    SELECT @Next = ISNULL(MAX(
        TRY_CAST(REPLACE(REPLACE(COUNT_NUMBER, 'IC-', ''), 'IC', '') AS INT)
    ), 0) + 1
    FROM dbo.INVENTORY_COUNT
    WHERE BUSINESS_ID = @BusinessId;

    SET @CountNumber = 'IC-' + RIGHT('000000' + CAST(@Next AS VARCHAR(10)), 6);
END
GO

-- =============================================================
-- SP_WS_GET_INVENTORY_COUNT_STATUS_ID
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_INVENTORY_COUNT_STATUS_ID
    @Code     VARCHAR(30),
    @StatusId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @StatusId = INVENTORY_COUNT_STATUS_ID
    FROM dbo.INVENTORY_COUNT_STATUS
    WHERE CODE = @Code AND STATUS = 1;
END
GO

-- =============================================================
-- SP_WS_CREATE_INVENTORY_COUNT
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_CREATE_INVENTORY_COUNT
    @BusinessId  BIGINT,
    @WarehouseId BIGINT,
    @CountDate   DATE,
    @Observation NVARCHAR(500) = NULL,
    @UserId      BIGINT,
    @Id          BIGINT OUTPUT,
    @COutput     INT OUTPUT,
    @SOutput     VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DraftStatusId BIGINT;
    DECLARE @CountNumber   VARCHAR(30);

    IF ISNULL(@BusinessId,  0) <= 0 RAISERROR('La empresa es obligatoria.',       16, 1);
    IF ISNULL(@WarehouseId, 0) <= 0 RAISERROR('Debe seleccionar un almacen.',      16, 1);
    IF @CountDate IS NULL          RAISERROR('La fecha de conteo es obligatoria.', 16, 1);

    IF NOT EXISTS (
        SELECT 1 FROM dbo.WAREHOUSES
        WHERE WAREHOUSES_ID = @WarehouseId AND BUSINESS_ID = @BusinessId AND STATUS = '1'
    ) RAISERROR('El almacen no existe o no esta activo.', 16, 1);

    EXEC dbo.SP_WS_GET_INVENTORY_COUNT_STATUS_ID 'DRAFT', @DraftStatusId OUTPUT;
    IF @DraftStatusId IS NULL
        RAISERROR('No existe el estado DRAFT para inventario fisico.', 16, 1);

    EXEC dbo.SP_WS_GET_NEXT_INVENTORY_COUNT_NUMBER @BusinessId, @CountNumber OUTPUT;

    INSERT INTO dbo.INVENTORY_COUNT
        (BUSINESS_ID, WAREHOUSE_ID, COUNT_NUMBER, COUNT_DATE,
         INVENTORY_COUNT_STATUS_ID, OBSERVATION, CREATE_USER, CREATE_DATE, STATUS)
    VALUES
        (@BusinessId, @WarehouseId, @CountNumber, @CountDate,
         @DraftStatusId, @Observation, @UserId, GETDATE(), 1);

    SET @Id      = SCOPE_IDENTITY();
    SET @COutput = 1;
    SET @SOutput = 'Toma de inventario creada correctamente.';
END
GO

-- =============================================================
-- SP_WS_START_INVENTORY_COUNT
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_START_INVENTORY_COUNT
    @BusinessId       BIGINT,
    @InventoryCountId BIGINT,
    @UserId           BIGINT,
    @COutput          INT OUTPUT,
    @SOutput          VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentStatusCode VARCHAR(30);
    DECLARE @WarehouseId       BIGINT;
    DECLARE @CountingStatusId  BIGINT;

    SELECT
        @CurrentStatusCode = ics.CODE,
        @WarehouseId       = ic.WAREHOUSE_ID
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.INVENTORY_COUNT_ID = @InventoryCountId
      AND ic.BUSINESS_ID = @BusinessId
      AND ic.STATUS = 1;

    IF @CurrentStatusCode IS NULL
        RAISERROR('La toma de inventario no existe.', 16, 1);

    IF @CurrentStatusCode <> 'DRAFT'
        RAISERROR('La toma de inventario no se encuentra en estado valido para iniciar conteo.', 16, 1);

    EXEC dbo.SP_WS_GET_INVENTORY_COUNT_STATUS_ID 'COUNTING', @CountingStatusId OUTPUT;
    IF @CountingStatusId IS NULL
        RAISERROR('No existe el estado COUNTING para inventario fisico.', 16, 1);

    INSERT INTO dbo.INVENTORY_COUNT_DETAIL
        (BUSINESS_ID, INVENTORY_COUNT_ID, PRODUCTS_ID,
         SYSTEM_QUANTITY, COUNTED_QUANTITY, UNIT_COST,
         CREATE_USER, CREATE_DATE, STATUS)
    SELECT
        @BusinessId,
        @InventoryCountId,
        ist.PRODUCTS_ID,
        ISNULL(ist.STOCK_QUANTITY, 0),
        0,
        ISNULL(ist.AVERAGE_COST, 0),
        @UserId,
        GETDATE(),
        1
    FROM dbo.INVENTORY_STOCK ist
    INNER JOIN dbo.PRODUCTS p
        ON p.PRODUCTS_ID   = ist.PRODUCTS_ID
       AND p.BUSINESS_ID   = @BusinessId
       AND p.STATUS        = '1'
       AND ISNULL(p.IS_STOCKABLE, 0) = 1
    WHERE ist.BUSINESS_ID  = @BusinessId
      AND ist.WAREHOUSE_ID = @WarehouseId;

    UPDATE dbo.INVENTORY_COUNT
    SET INVENTORY_COUNT_STATUS_ID = @CountingStatusId,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE INVENTORY_COUNT_ID = @InventoryCountId
      AND BUSINESS_ID = @BusinessId;

    SET @COutput = 1;
    SET @SOutput = 'Conteo de inventario iniciado correctamente.';
END
GO

-- =============================================================
-- SP_WS_UPDATE_INVENTORY_COUNT_DETAILS
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_UPDATE_INVENTORY_COUNT_DETAILS
    @BusinessId       BIGINT,
    @InventoryCountId BIGINT,
    @UserId           BIGINT,
    @Details          dbo.InventoryCountDetailUpdateType READONLY,
    @COutput          INT OUTPUT,
    @SOutput          VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentStatusCode VARCHAR(30);

    SELECT @CurrentStatusCode = ics.CODE
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.INVENTORY_COUNT_ID = @InventoryCountId
      AND ic.BUSINESS_ID = @BusinessId
      AND ic.STATUS = 1;

    IF @CurrentStatusCode IS NULL
        RAISERROR('La toma de inventario no existe.', 16, 1);

    IF @CurrentStatusCode <> 'COUNTING'
        RAISERROR('La toma de inventario no se encuentra en estado valido para editar.', 16, 1);

    IF EXISTS (
        SELECT 1
        FROM @Details d
        LEFT JOIN dbo.INVENTORY_COUNT_DETAIL icd
            ON icd.INVENTORY_COUNT_DETAIL_ID = d.INVENTORY_COUNT_DETAIL_ID
           AND icd.INVENTORY_COUNT_ID = @InventoryCountId
           AND icd.BUSINESS_ID = @BusinessId
           AND icd.STATUS = 1
        WHERE icd.INVENTORY_COUNT_DETAIL_ID IS NULL
    ) RAISERROR('Uno o mas detalles no pertenecen a la toma de inventario.', 16, 1);

    UPDATE icd
    SET COUNTED_QUANTITY = d.COUNTED_QUANTITY,
        LOT_NUMBER       = d.LOT_NUMBER,
        SERIAL_NUMBER    = d.SERIAL_NUMBER,
        EXPIRATION_DATE  = d.EXPIRATION_DATE,
        OBSERVATION      = d.OBSERVATION,
        UPDATE_USER      = @UserId,
        UPDATE_DATE      = GETDATE()
    FROM dbo.INVENTORY_COUNT_DETAIL icd
    INNER JOIN @Details d ON d.INVENTORY_COUNT_DETAIL_ID = icd.INVENTORY_COUNT_DETAIL_ID
    WHERE icd.INVENTORY_COUNT_ID = @InventoryCountId
      AND icd.BUSINESS_ID = @BusinessId
      AND icd.STATUS = 1;

    SET @COutput = 1;
    SET @SOutput = 'Detalles de inventario actualizados correctamente.';
END
GO

-- =============================================================
-- SP_WS_CLOSE_INVENTORY_COUNT
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_CLOSE_INVENTORY_COUNT
    @BusinessId       BIGINT,
    @InventoryCountId BIGINT,
    @UserId           BIGINT,
    @COutput          INT OUTPUT,
    @SOutput          VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentStatusCode VARCHAR(30);
    DECLARE @ClosedStatusId    BIGINT;

    SELECT @CurrentStatusCode = ics.CODE
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.INVENTORY_COUNT_ID = @InventoryCountId
      AND ic.BUSINESS_ID = @BusinessId
      AND ic.STATUS = 1;

    IF @CurrentStatusCode IS NULL
        RAISERROR('La toma de inventario no existe.', 16, 1);

    IF @CurrentStatusCode <> 'COUNTING'
        RAISERROR('La toma de inventario no se encuentra en estado valido para ser cerrada.', 16, 1);

    EXEC dbo.SP_WS_GET_INVENTORY_COUNT_STATUS_ID 'CLOSED', @ClosedStatusId OUTPUT;
    IF @ClosedStatusId IS NULL
        RAISERROR('No existe el estado CLOSED para inventario fisico.', 16, 1);

    UPDATE dbo.INVENTORY_COUNT
    SET INVENTORY_COUNT_STATUS_ID = @ClosedStatusId,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE INVENTORY_COUNT_ID = @InventoryCountId
      AND BUSINESS_ID = @BusinessId;

    SET @COutput = 1;
    SET @SOutput = 'Toma de inventario cerrada correctamente.';
END
GO

-- =============================================================
-- SP_WS_CANCEL_INVENTORY_COUNT
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_CANCEL_INVENTORY_COUNT
    @BusinessId       BIGINT,
    @InventoryCountId BIGINT,
    @UserId           BIGINT,
    @COutput          INT OUTPUT,
    @SOutput          VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentStatusCode VARCHAR(30);
    DECLARE @CancelledStatusId BIGINT;

    SELECT @CurrentStatusCode = ics.CODE
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.INVENTORY_COUNT_ID = @InventoryCountId
      AND ic.BUSINESS_ID = @BusinessId
      AND ic.STATUS = 1;

    IF @CurrentStatusCode IS NULL
        RAISERROR('La toma de inventario no existe.', 16, 1);

    IF @CurrentStatusCode = 'ADJUSTED'
        RAISERROR('No se puede anular una toma de inventario ya ajustada.', 16, 1);

    IF @CurrentStatusCode = 'CANCELLED'
        RAISERROR('La toma de inventario ya se encuentra anulada.', 16, 1);

    EXEC dbo.SP_WS_GET_INVENTORY_COUNT_STATUS_ID 'CANCELLED', @CancelledStatusId OUTPUT;
    IF @CancelledStatusId IS NULL
        RAISERROR('No existe el estado CANCELLED para inventario fisico.', 16, 1);

    UPDATE dbo.INVENTORY_COUNT
    SET INVENTORY_COUNT_STATUS_ID = @CancelledStatusId,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE INVENTORY_COUNT_ID = @InventoryCountId
      AND BUSINESS_ID = @BusinessId;

    SET @COutput = 1;
    SET @SOutput = 'Toma de inventario anulada correctamente.';
END
GO

-- =============================================================
-- SP_WS_MARK_INVENTORY_COUNT_AS_ADJUSTED
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_MARK_INVENTORY_COUNT_AS_ADJUSTED
    @BusinessId       BIGINT,
    @InventoryCountId BIGINT,
    @UserId           BIGINT,
    @Adjustments      dbo.InventoryCountDetailAdjustmentType READONLY,
    @COutput          INT OUTPUT,
    @SOutput          VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentStatusCode VARCHAR(30);
    DECLARE @AdjustedStatusId  BIGINT;

    SELECT @CurrentStatusCode = ics.CODE
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.INVENTORY_COUNT_ID = @InventoryCountId
      AND ic.BUSINESS_ID = @BusinessId
      AND ic.STATUS = 1;

    IF @CurrentStatusCode IS NULL
        RAISERROR('La toma de inventario no existe.', 16, 1);

    IF @CurrentStatusCode <> 'CLOSED'
        RAISERROR('La toma de inventario debe estar cerrada para marcarla como ajustada.', 16, 1);

    UPDATE icd
    SET ADJUSTMENT_MOVEMENT_ID = a.ADJUSTMENT_MOVEMENT_ID,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    FROM dbo.INVENTORY_COUNT_DETAIL icd
    INNER JOIN @Adjustments a ON a.INVENTORY_COUNT_DETAIL_ID = icd.INVENTORY_COUNT_DETAIL_ID
    WHERE icd.INVENTORY_COUNT_ID = @InventoryCountId
      AND icd.BUSINESS_ID = @BusinessId
      AND icd.STATUS = 1;

    EXEC dbo.SP_WS_GET_INVENTORY_COUNT_STATUS_ID 'ADJUSTED', @AdjustedStatusId OUTPUT;
    IF @AdjustedStatusId IS NULL
        RAISERROR('No existe el estado ADJUSTED para inventario fisico.', 16, 1);

    UPDATE dbo.INVENTORY_COUNT
    SET INVENTORY_COUNT_STATUS_ID = @AdjustedStatusId,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    WHERE INVENTORY_COUNT_ID = @InventoryCountId
      AND BUSINESS_ID = @BusinessId;

    SET @COutput = 1;
    SET @SOutput = 'Ajustes de inventario generados correctamente.';
END
GO

-- =============================================================
-- SP_WS_LIST_INVENTORY_COUNT
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_LIST_INVENTORY_COUNT
    @BusinessId             BIGINT,
    @WarehouseId            BIGINT = NULL,
    @InventoryCountStatusId BIGINT = NULL,
    @DateFrom               DATETIME = NULL,
    @DateTo                 DATETIME = NULL,
    @Search                 VARCHAR(100) = NULL,
    @Page                   INT = 1,
    @PageSize               INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ic.INVENTORY_COUNT_ID        AS InventoryCountId,
        ic.BUSINESS_ID               AS BusinessId,
        ic.WAREHOUSE_ID              AS WarehouseId,
        w.DESCRIPTION                AS WarehouseName,
        ic.COUNT_NUMBER              AS CountNumber,
        ic.COUNT_DATE                AS CountDate,
        ic.INVENTORY_COUNT_STATUS_ID AS InventoryCountStatusId,
        ics.CODE                     AS StatusCode,
        ics.DESCRIPTION              AS StatusDescription,
        ic.OBSERVATION               AS Observation,
        ic.CREATE_DATE               AS CreateDate,
        COUNT(1) OVER()              AS TotalCount
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.WAREHOUSES w
        ON w.WAREHOUSES_ID = ic.WAREHOUSE_ID
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.BUSINESS_ID = @BusinessId
      AND ic.STATUS = 1
      AND (@WarehouseId            IS NULL OR ic.WAREHOUSE_ID             = @WarehouseId)
      AND (@InventoryCountStatusId IS NULL OR ic.INVENTORY_COUNT_STATUS_ID = @InventoryCountStatusId)
      AND (@DateFrom IS NULL OR ic.COUNT_DATE >= @DateFrom)
      AND (@DateTo   IS NULL OR ic.COUNT_DATE <  DATEADD(DAY, 1, @DateTo))
      AND (
          NULLIF(LTRIM(RTRIM(@Search)), '') IS NULL
          OR ic.COUNT_NUMBER LIKE '%' + @Search + '%'
          OR w.DESCRIPTION   LIKE '%' + @Search + '%'
      )
    ORDER BY ic.COUNT_DATE DESC, ic.INVENTORY_COUNT_ID DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- =============================================================
-- SP_WS_GET_INVENTORY_COUNT_BY_ID
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_INVENTORY_COUNT_BY_ID
    @BusinessId       BIGINT,
    @InventoryCountId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Cabecera
    SELECT
        ic.INVENTORY_COUNT_ID        AS InventoryCountId,
        ic.BUSINESS_ID               AS BusinessId,
        ic.WAREHOUSE_ID              AS WarehouseId,
        w.DESCRIPTION                AS WarehouseName,
        ic.COUNT_NUMBER              AS CountNumber,
        ic.COUNT_DATE                AS CountDate,
        ic.INVENTORY_COUNT_STATUS_ID AS InventoryCountStatusId,
        ics.CODE                     AS StatusCode,
        ics.DESCRIPTION              AS StatusDescription,
        ic.OBSERVATION               AS Observation,
        ic.CREATE_DATE               AS CreateDate
    FROM dbo.INVENTORY_COUNT ic
    INNER JOIN dbo.WAREHOUSES w
        ON w.WAREHOUSES_ID = ic.WAREHOUSE_ID
    INNER JOIN dbo.INVENTORY_COUNT_STATUS ics
        ON ics.INVENTORY_COUNT_STATUS_ID = ic.INVENTORY_COUNT_STATUS_ID
    WHERE ic.BUSINESS_ID      = @BusinessId
      AND ic.INVENTORY_COUNT_ID = @InventoryCountId
      AND ic.STATUS = 1;

    -- Detalle
    SELECT
        icd.INVENTORY_COUNT_DETAIL_ID AS InventoryCountDetailId,
        icd.INVENTORY_COUNT_ID        AS InventoryCountId,
        icd.PRODUCTS_ID               AS ProductsId,
        p.SKU                         AS ProductCode,
        p.DESCRIPTION                 AS ProductName,
        icd.SYSTEM_QUANTITY           AS SystemQuantity,
        icd.COUNTED_QUANTITY          AS CountedQuantity,
        icd.DIFFERENCE_QUANTITY       AS DifferenceQuantity,
        icd.UNIT_COST                 AS UnitCost,
        icd.TOTAL_DIFFERENCE_COST     AS TotalDifferenceCost,
        icd.LOT_NUMBER                AS LotNumber,
        icd.SERIAL_NUMBER             AS SerialNumber,
        icd.EXPIRATION_DATE           AS ExpirationDate,
        icd.ADJUSTMENT_MOVEMENT_ID    AS AdjustmentMovementId,
        icd.OBSERVATION               AS Observation
    FROM dbo.INVENTORY_COUNT_DETAIL icd
    INNER JOIN dbo.PRODUCTS p
        ON p.PRODUCTS_ID = icd.PRODUCTS_ID
    WHERE icd.BUSINESS_ID       = @BusinessId
      AND icd.INVENTORY_COUNT_ID = @InventoryCountId
      AND icd.STATUS = 1
    ORDER BY icd.INVENTORY_COUNT_DETAIL_ID;
END
GO

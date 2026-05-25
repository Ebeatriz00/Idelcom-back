IF COL_LENGTH('dbo.PURCHASE_RECEIPT', 'RECEIPT_TYPE_ID') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_RECEIPT
    ADD RECEIPT_TYPE_ID BIGINT NOT NULL
        CONSTRAINT DF_PURCHASE_RECEIPT_RECEIPT_TYPE_ID DEFAULT (1);
END
GO

IF NOT EXISTS (
    SELECT 1
FROM sys.indexes
WHERE name = 'IX_PURCHASE_RECEIPT_BUSINESS_TYPE_DATE'
    AND object_id = OBJECT_ID('dbo.PURCHASE_RECEIPT')
)
BEGIN
    CREATE INDEX IX_PURCHASE_RECEIPT_BUSINESS_TYPE_DATE
    ON dbo.PURCHASE_RECEIPT (BUSINESS_ID, RECEIPT_TYPE_ID, RECEIPT_DATE);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_CREATE_PURCHASE_RECEIPT
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT = NULL,
    @SuppliersId BIGINT,
    @WarehouseId BIGINT,
    @ReceiptTypeId BIGINT = 1,
    @ReceiptDate DATETIME,
    @SupplierGuideNumber VARCHAR(30) = NULL,
    @SupplierGuideDate DATETIME = NULL,
    @Observation VARCHAR(500) = NULL,
    @UserId BIGINT,
    @Details dbo.PurchaseReceiptDetailInputType READONLY,
    @Id BIGINT OUTPUT,
    @WarehouseMovementId BIGINT OUTPUT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF;

    BEGIN TRY
        DECLARE @ReceiptNumber VARCHAR(30);
        DECLARE @ReceiptStatusId BIGINT;
        DECLARE @MovementTypeId BIGINT;
        DECLARE @HasStockable BIT = 0;
        DECLARE @HasServiceOnly BIT = 0;

        IF ISNULL(@BusinessId, 0) <= 0 RAISERROR('La empresa es obligatoria.', 16, 1);
        IF ISNULL(@SuppliersId, 0) <= 0 RAISERROR('El proveedor es obligatorio.', 16, 1);
        IF ISNULL(@WarehouseId, 0) <= 0 RAISERROR('El almacen es obligatorio.', 16, 1);
        IF ISNULL(@ReceiptTypeId, 0) NOT IN (1, 2) RAISERROR('El tipo de recepcion no es valido.', 16, 1);
        IF @ReceiptDate IS NULL RAISERROR('La fecha de recepcion es obligatoria.', 16, 1);
        IF NOT EXISTS (SELECT 1
    FROM @Details) RAISERROR('Debe registrar al menos un detalle.', 16, 1);
        IF EXISTS (SELECT 1
    FROM @Details
    WHERE RECEIVED_QUANTITY <= 0) RAISERROR('La cantidad recibida debe ser mayor a cero.', 16, 1);
        IF EXISTS (SELECT 1
    FROM @Details
    WHERE UNIT_COST < 0) RAISERROR('El costo unitario no puede ser negativo.', 16, 1);
        IF EXISTS (SELECT PRODUCTS_ID
    FROM @Details
    GROUP BY PRODUCTS_ID
    HAVING COUNT(1) > 1) RAISERROR('No se permite repetir productos en la misma recepcion.', 16, 1);

        IF NOT EXISTS (SELECT 1
    FROM dbo.SUPPLIERS
    WHERE SUPPLIERS_ID = @SuppliersId AND BUSINESS_ID = @BusinessId AND STATUS = '1')
            RAISERROR('El proveedor no existe o no esta activo.', 16, 1);

        IF NOT EXISTS (SELECT 1
    FROM dbo.WAREHOUSES
    WHERE WAREHOUSES_ID = @WarehouseId AND BUSINESS_ID = @BusinessId AND STATUS = '1')
            RAISERROR('El almacen no existe o no esta activo.', 16, 1);

        IF EXISTS (
            SELECT 1
    FROM @Details d
        LEFT JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = d.PRODUCTS_ID
            AND p.BUSINESS_ID = @BusinessId
            AND p.STATUS = '1'
    WHERE p.PRODUCTS_ID IS NULL
        )
            RAISERROR('Uno o mas productos no existen o no estan activos.', 16, 1);

        SELECT @HasStockable = CASE WHEN EXISTS (
            SELECT 1
        FROM @Details d
            INNER JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = d.PRODUCTS_ID
        WHERE p.BUSINESS_ID = @BusinessId
            AND ISNULL(p.IS_STOCKABLE, 0) = 1
        ) THEN 1 ELSE 0 END;

        SELECT @HasServiceOnly = CASE WHEN EXISTS (
            SELECT 1
        FROM @Details d
            INNER JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = d.PRODUCTS_ID
        WHERE p.BUSINESS_ID = @BusinessId
            AND ISNULL(p.IS_SERVICE, 0) = 1
            AND ISNULL(p.IS_STOCKABLE, 0) = 0
        ) THEN 1 ELSE 0 END;

        IF @HasStockable = 1 AND @HasServiceOnly = 1
            RAISERROR('La recepcion no puede mezclar productos stockeables y servicios. Genere una recepcion de mercaderia para productos y una conformidad de servicio para servicios.', 16, 1);

        IF @ReceiptTypeId = 1 AND EXISTS (
            SELECT 1
        FROM @Details d
            INNER JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = d.PRODUCTS_ID
        WHERE p.BUSINESS_ID = @BusinessId
            AND ISNULL(p.IS_STOCKABLE, 0) <> 1
        )
            RAISERROR('La recepcion de mercaderia solo admite productos stockeables.', 16, 1);

        IF @ReceiptTypeId = 2
        BEGIN
        IF @PurchaseOrderId IS NULL
                RAISERROR('La conformidad de servicio requiere orden de compra.', 16, 1);

        IF EXISTS (
                SELECT 1
        FROM @Details d
            INNER JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = d.PRODUCTS_ID
        WHERE p.BUSINESS_ID = @BusinessId
            AND (ISNULL(p.IS_SERVICE, 0) <> 1 OR ISNULL(p.IS_STOCKABLE, 0) = 1)
            )
                RAISERROR('La conformidad de servicio solo admite items de servicio no stockeables.', 16, 1);
    END

        IF @PurchaseOrderId IS NOT NULL
        BEGIN
        IF NOT EXISTS (
                SELECT 1
        FROM dbo.PURCHASE_ORDER po
        WHERE po.PURCHASE_ORDER_ID = @PurchaseOrderId
            AND po.BUSINESS_ID = @BusinessId
            AND po.SUPPLIERS_ID = @SuppliersId
            AND po.STATUS = '1'
            )
                RAISERROR('La orden de compra no existe, no pertenece al proveedor o no esta activa.', 16, 1);

        IF EXISTS (SELECT 1
        FROM @Details
        WHERE PURCHASE_ORDER_DETAIL_ID IS NULL)
                RAISERROR('Todos los detalles deben indicar detalle de orden de compra.', 16, 1);

        IF EXISTS (
                SELECT 1
        FROM @Details d
            INNER JOIN dbo.PURCHASE_ORDER_DETAIL pod ON pod.PURCHASE_ORDER_DETAIL_ID = d.PURCHASE_ORDER_DETAIL_ID
        WHERE pod.BUSINESS_ID <> @BusinessId
            OR pod.PURCHASE_ORDER_ID <> @PurchaseOrderId
            OR pod.IS_ACTIVE <> 1
            OR pod.PRODUCTS_ID <> d.PRODUCTS_ID
            OR d.RECEIVED_QUANTITY > pod.QUANTITY - ISNULL(pod.RECEIVED_QUANTITY, 0)
            )
                RAISERROR('Uno o mas detalles no coinciden con la orden o exceden la cantidad pendiente.', 16, 1);
    END
        ELSE IF EXISTS (SELECT 1
    FROM @Details
    WHERE PURCHASE_ORDER_DETAIL_ID IS NOT NULL)
            RAISERROR('Una recepcion sin orden de compra no debe indicar detalle de orden de compra.', 16, 1);

        EXEC dbo.SP_WS_GET_NEXT_PURCHASE_RECEIPT_NUMBER @BusinessId, @ReceiptNumber OUTPUT;
        EXEC dbo.SP_WS_GET_PURCHASE_RECEIPT_STATUS_ID @BusinessId, 'REGISTERED', @ReceiptStatusId OUTPUT;

        IF @ReceiptStatusId IS NULL SET @ReceiptStatusId = 1;
        SET @WarehouseMovementId = NULL;

        IF @ReceiptTypeId = 1
        BEGIN
        EXEC dbo.SP_CORE_GET_PURCHASE_ENTRY_MOVEMENT_TYPE_ID @BusinessId, @MovementTypeId OUTPUT;
        IF @MovementTypeId IS NULL RAISERROR('No existe un tipo de movimiento activo para ingreso por compra.', 16, 1);

        INSERT INTO dbo.WAREHOUSE_MOVEMENT
            (
            BUSINESS_ID, MOVEMENT_TYPE_ID, WAREHOUSE_ID, SUPPLIERS_ID, CLIENTS_ID,
            SERIES, NUMBER_DOCUMENT, REFERENCE_DOCUMENT, MOVEMENT_DATE, OBSERVATION,
            SUB_TOTAL, IGV, TOTAL, PURCHASE_ORDER_ID, SOURCE_MODULE, SOURCE_DOCUMENT_TYPE,
            CREATE_USER, CREATE_DATE
            )
        SELECT
            @BusinessId, @MovementTypeId, @WarehouseId, @SuppliersId, NULL,
            NULL, @ReceiptNumber, @SupplierGuideNumber, @ReceiptDate, @Observation,
            SUM(RECEIVED_QUANTITY * UNIT_COST), 0, SUM(RECEIVED_QUANTITY * UNIT_COST),
            @PurchaseOrderId, 'PURCHASE_RECEIPT', 'PURCHASE_RECEIPT',
            @UserId, GETDATE()
        FROM @Details;

        SET @WarehouseMovementId = SCOPE_IDENTITY();
    END

        INSERT INTO dbo.PURCHASE_RECEIPT
        (
        BUSINESS_ID, PURCHASE_ORDER_ID, SUPPLIERS_ID, WAREHOUSE_ID, RECEIPT_TYPE_ID,
        RECEIPT_NUMBER, RECEIPT_DATE, SUPPLIER_GUIDE_NUMBER, SUPPLIER_GUIDE_DATE,
        WAREHOUSE_MOVEMENT_ID, RECEIPT_STATUS_ID, OBSERVATION, IS_WITHOUT_PURCHASE_ORDER,
        IS_REGULARIZED, IS_ACTIVE, CREATE_USER, CREATE_DATE
        )
    VALUES
        (
            @BusinessId, @PurchaseOrderId, @SuppliersId, @WarehouseId, @ReceiptTypeId,
            @ReceiptNumber, @ReceiptDate, @SupplierGuideNumber, @SupplierGuideDate,
            @WarehouseMovementId, @ReceiptStatusId, @Observation,
            CASE WHEN @PurchaseOrderId IS NULL THEN 1 ELSE 0 END, 0, 1, @UserId, GETDATE()
        );

        SET @Id = SCOPE_IDENTITY();

        IF @WarehouseMovementId IS NOT NULL
        BEGIN
        UPDATE dbo.WAREHOUSE_MOVEMENT
            SET SOURCE_DOCUMENT_ID = @Id
            WHERE WAREHOUSE_MOVEMENT_ID = @WarehouseMovementId;
    END

        DECLARE @CreatedDetails TABLE
        (
        PURCHASE_RECEIPT_DETAIL_ID BIGINT,
        PURCHASE_ORDER_DETAIL_ID BIGINT NULL,
        PRODUCTS_ID BIGINT,
        RECEIVED_QUANTITY DECIMAL(18,4),
        UNIT_COST DECIMAL(18,4),
        TOTAL_COST DECIMAL(18,4),
        OBSERVATION VARCHAR(500) NULL
        );

        INSERT INTO dbo.PURCHASE_RECEIPT_DETAIL
        (
        BUSINESS_ID, PURCHASE_RECEIPT_ID, PURCHASE_ORDER_DETAIL_ID, PRODUCTS_ID,
        UOM_ID, ORDERED_QUANTITY, RECEIVED_QUANTITY, UNIT_COST, TOTAL_COST,
        OBSERVATION, IS_ACTIVE, CREATE_USER, CREATE_DATE
        )
    OUTPUT
            INSERTED.PURCHASE_RECEIPT_DETAIL_ID,
            INSERTED.PURCHASE_ORDER_DETAIL_ID,
            INSERTED.PRODUCTS_ID,
            INSERTED.RECEIVED_QUANTITY,
            INSERTED.UNIT_COST,
            INSERTED.TOTAL_COST,
            INSERTED.OBSERVATION
        INTO @CreatedDetails
    SELECT
        @BusinessId, @Id, PURCHASE_ORDER_DETAIL_ID, PRODUCTS_ID,
        UOM_ID, ORDERED_QUANTITY, RECEIVED_QUANTITY, UNIT_COST, RECEIVED_QUANTITY * UNIT_COST,
        OBSERVATION, 1, @UserId, GETDATE()
    FROM @Details;

        IF @ReceiptTypeId = 1
        BEGIN
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
                SELECT PURCHASE_RECEIPT_DETAIL_ID, PURCHASE_ORDER_DETAIL_ID, PRODUCTS_ID, RECEIVED_QUANTITY, UNIT_COST, TOTAL_COST, OBSERVATION
        FROM @CreatedDetails;

        DECLARE @ReceiptDetailId BIGINT;
        DECLARE @PurchaseOrderDetailId BIGINT;
        DECLARE @ProductsId BIGINT;
        DECLARE @Quantity DECIMAL(18,4);
        DECLARE @UnitCost DECIMAL(18,4);
        DECLARE @TotalCost DECIMAL(18,4);
        DECLARE @DetailObservation VARCHAR(500);
        DECLARE @MovementDetailId BIGINT;
        DECLARE @PreviousStock DECIMAL(18,4);
        DECLARE @FinalStock DECIMAL(18,4);
        DECLARE @AverageCost DECIMAL(18,4);
        DECLARE @KardexId BIGINT;
        DECLARE @KardexCOutput INT;
        DECLARE @KardexSOutput VARCHAR(500);

        OPEN cur;
        FETCH NEXT FROM cur INTO @ReceiptDetailId, @PurchaseOrderDetailId, @ProductsId, @Quantity, @UnitCost, @TotalCost, @DetailObservation;

        WHILE @@FETCH_STATUS = 0
            BEGIN
            SELECT @PreviousStock = ISNULL(STOCK_QUANTITY, 0)
            FROM dbo.INVENTORY_STOCK WITH (UPDLOCK, HOLDLOCK)
            WHERE BUSINESS_ID = @BusinessId
                AND WAREHOUSE_ID = @WarehouseId
                AND PRODUCTS_ID = @ProductsId;

            SET @PreviousStock = ISNULL(@PreviousStock, 0);

            INSERT INTO dbo.WAREHOUSE_MOVEMENT_DETAIL
                (
                WAREHOUSE_MOVEMENT_ID, BUSINESS_ID, PRODUCTS_ID, QUANTITY,
                UNIT_COST, TOTAL_COST, OBSERVATION, PURCHASE_ORDER_DETAIL_ID,
                PURCHASE_RECEIPT_DETAIL_ID, CREATE_USER, CREATE_DATE
                )
            VALUES
                (
                    @WarehouseMovementId, @BusinessId, @ProductsId, @Quantity,
                    @UnitCost, @TotalCost, @DetailObservation, @PurchaseOrderDetailId,
                    @ReceiptDetailId, @UserId, GETDATE()
                );

            SET @MovementDetailId = SCOPE_IDENTITY();

            EXEC dbo.SP_WS_INCREASE_INVENTORY_STOCK
                    @BusinessId = @BusinessId,
                    @WarehouseId = @WarehouseId,
                    @ProductsId = @ProductsId,
                    @Quantity = @Quantity,
                    @UnitCost = @UnitCost,
                    @UserId = @UserId;

            SELECT
                @FinalStock = STOCK_QUANTITY,
                @AverageCost = AVERAGE_COST
            FROM dbo.INVENTORY_STOCK
            WHERE BUSINESS_ID = @BusinessId
                AND WAREHOUSE_ID = @WarehouseId
                AND PRODUCTS_ID = @ProductsId;

            EXEC dbo.SP_WS_REGISTER_INVENTORY_KARDEX
                    @BusinessId = @BusinessId,
                    @WarehouseId = @WarehouseId,
                    @ProductsId = @ProductsId,
                    @WareHouseMovementId = @WarehouseMovementId,
                    @WareHouseMovementDetailId = @MovementDetailId,
                    @MovementTypesId = @MovementTypeId,
                    @MovementDate = @ReceiptDate,
                    @EntryQuantity = @Quantity,
                    @ExitQuantity = 0,
                    @PreviousStock = @PreviousStock,
                    @FinalStock = @FinalStock,
                    @UnitCost = @UnitCost,
                    @AverageCost = @AverageCost,
                    @TotalCost = @TotalCost,
                    @ReferenceDocumentType = 'PURCHASE_RECEIPT',
                    @ReferenceDocumentNumber = @ReceiptNumber,
                    @Observation = @DetailObservation,
                    @CreateUser = @UserId,
                    @Id = @KardexId OUTPUT,
                    @COutput = @KardexCOutput OUTPUT,
                    @SOutput = @KardexSOutput OUTPUT;

            IF ISNULL(@KardexCOutput, 0) <> 1
                    RAISERROR(@KardexSOutput, 16, 1);

            FETCH NEXT FROM cur INTO @ReceiptDetailId, @PurchaseOrderDetailId, @ProductsId, @Quantity, @UnitCost, @TotalCost, @DetailObservation;
        END

        CLOSE cur;
        DEALLOCATE cur;
    END

        IF @PurchaseOrderId IS NOT NULL
        BEGIN
        UPDATE pod
            SET RECEIVED_QUANTITY = ISNULL(pod.RECEIVED_QUANTITY, 0) + d.RECEIVED_QUANTITY,
                UPDATE_USER = @UserId,
                UPDATE_DATE = GETDATE()
            FROM dbo.PURCHASE_ORDER_DETAIL pod
            INNER JOIN @Details d ON d.PURCHASE_ORDER_DETAIL_ID = pod.PURCHASE_ORDER_DETAIL_ID
            WHERE pod.BUSINESS_ID = @BusinessId
            AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId;

        EXEC dbo.SP_CORE_RECALCULATE_PURCHASE_ORDER_RECEPTION_STATUS @BusinessId, @PurchaseOrderId;
    END

        SET @COutput = 1;
        SET @SOutput = CASE
            WHEN @ReceiptTypeId = 2 THEN 'Conformidad de servicio registrada correctamente.'
            ELSE 'Recepcion de compra registrada correctamente.'
        END;
    END TRY
    BEGIN CATCH
        SET @Id = 0;
        SET @WarehouseMovementId = NULL;
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_LIST_PURCHASE_RECEIPT
    @BusinessId BIGINT,
    @WarehouseId BIGINT = NULL,
    @SuppliersId BIGINT = NULL,
    @PurchaseOrderId BIGINT = NULL,
    @ReceiptStatusId BIGINT = NULL,
    @ReceiptTypeId BIGINT = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL,
    @Search VARCHAR(100) = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pr.PURCHASE_RECEIPT_ID AS PurchaseReceiptId,
        pr.BUSINESS_ID AS BusinessId,
        pr.PURCHASE_ORDER_ID AS PurchaseOrderId,
        pr.REGULARIZED_PURCHASE_ORDER_ID AS RegularizedPurchaseOrderId,
        po.PURCHASE_ORDER_NUMBER AS PurchaseOrderNumber,
        pr.SUPPLIERS_ID AS SuppliersId,
        s.SUPPLIER_NAME AS SupplierName,
        pr.WAREHOUSE_ID AS WarehouseId,
        w.DESCRIPTION AS WarehouseName,
        ISNULL(pr.RECEIPT_TYPE_ID, 1) AS ReceiptTypeId,
        CASE WHEN ISNULL(pr.RECEIPT_TYPE_ID, 1) = 2 THEN 'Conformidad de servicio' ELSE 'Recepcion de mercaderia' END AS ReceiptTypeName,
        pr.RECEIPT_NUMBER AS ReceiptNumber,
        pr.RECEIPT_DATE AS ReceiptDate,
        pr.SUPPLIER_GUIDE_NUMBER AS SupplierGuideNumber,
        pr.SUPPLIER_GUIDE_DATE AS SupplierGuideDate,
        pr.WAREHOUSE_MOVEMENT_ID AS WarehouseMovementId,
        pr.RECEIPT_STATUS_ID AS ReceiptStatusId,
        prs.DESCRIPTION AS ReceiptStatusName,
        pr.OBSERVATION AS Observation,
        pr.IS_ACTIVE AS IsActive,
        pr.IS_WITHOUT_PURCHASE_ORDER AS IsWithoutPurchaseOrder,
        pr.IS_REGULARIZED AS IsRegularized,
        CAST(CASE WHEN ISNULL(pr.RECEIPT_TYPE_ID, 1) = 2 THEN 1 ELSE 0 END AS BIT) AS IsServiceReceipt,
        CAST(CASE WHEN ISNULL(pr.RECEIPT_TYPE_ID, 1) = 1 THEN 1 ELSE 0 END AS BIT) AS IsWarehouseReceipt,
        pr.CREATE_DATE AS CreateDate,
        COUNT(1) OVER() AS TotalCount
    FROM dbo.PURCHASE_RECEIPT pr
        INNER JOIN dbo.SUPPLIERS s ON s.SUPPLIERS_ID = pr.SUPPLIERS_ID
        INNER JOIN dbo.WAREHOUSES w ON w.WAREHOUSES_ID = pr.WAREHOUSE_ID
        INNER JOIN dbo.PURCHASE_RECEIPT_STATUS prs ON prs.PURCHASE_RECEIPT_STATUS_ID = pr.RECEIPT_STATUS_ID
        LEFT JOIN dbo.PURCHASE_ORDER po ON po.PURCHASE_ORDER_ID = pr.PURCHASE_ORDER_ID
    WHERE pr.BUSINESS_ID = @BusinessId
        AND (@WarehouseId IS NULL OR pr.WAREHOUSE_ID = @WarehouseId)
        AND (@SuppliersId IS NULL OR pr.SUPPLIERS_ID = @SuppliersId)
        AND (@PurchaseOrderId IS NULL OR pr.PURCHASE_ORDER_ID = @PurchaseOrderId)
        AND (@ReceiptStatusId IS NULL OR pr.RECEIPT_STATUS_ID = @ReceiptStatusId)
        AND (@ReceiptTypeId IS NULL OR ISNULL(pr.RECEIPT_TYPE_ID, 1) = @ReceiptTypeId)
        AND (@DateFrom IS NULL OR pr.RECEIPT_DATE >= @DateFrom)
        AND (@DateTo IS NULL OR pr.RECEIPT_DATE < DATEADD(DAY, 1, @DateTo))
        AND (
            NULLIF(LTRIM(RTRIM(@Search)), '') IS NULL
        OR pr.RECEIPT_NUMBER LIKE '%' + @Search + '%'
        OR pr.SUPPLIER_GUIDE_NUMBER LIKE '%' + @Search + '%'
        OR s.SUPPLIER_NAME LIKE '%' + @Search + '%'
      )
    ORDER BY pr.RECEIPT_DATE DESC, pr.PURCHASE_RECEIPT_ID DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_RECEIPT_BY_ID
    @BusinessId BIGINT,
    @PurchaseReceiptId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        pr.PURCHASE_RECEIPT_ID AS PurchaseReceiptId,
        pr.BUSINESS_ID AS BusinessId,
        pr.PURCHASE_ORDER_ID AS PurchaseOrderId,
        pr.REGULARIZED_PURCHASE_ORDER_ID AS RegularizedPurchaseOrderId,
        po.PURCHASE_ORDER_NUMBER AS PurchaseOrderNumber,
        pr.SUPPLIERS_ID AS SuppliersId,
        s.SUPPLIER_NAME AS SupplierName,
        pr.WAREHOUSE_ID AS WarehouseId,
        w.DESCRIPTION AS WarehouseName,
        ISNULL(pr.RECEIPT_TYPE_ID, 1) AS ReceiptTypeId,
        CASE WHEN ISNULL(pr.RECEIPT_TYPE_ID, 1) = 2 THEN 'Conformidad de servicio' ELSE 'Recepcion de mercaderia' END AS ReceiptTypeName,
        pr.RECEIPT_NUMBER AS ReceiptNumber,
        pr.RECEIPT_DATE AS ReceiptDate,
        pr.SUPPLIER_GUIDE_NUMBER AS SupplierGuideNumber,
        pr.SUPPLIER_GUIDE_DATE AS SupplierGuideDate,
        pr.WAREHOUSE_MOVEMENT_ID AS WarehouseMovementId,
        pr.RECEIPT_STATUS_ID AS ReceiptStatusId,
        prs.DESCRIPTION AS ReceiptStatusName,
        pr.OBSERVATION AS Observation,
        pr.IS_ACTIVE AS IsActive,
        pr.IS_WITHOUT_PURCHASE_ORDER AS IsWithoutPurchaseOrder,
        pr.IS_REGULARIZED AS IsRegularized,
        CAST(CASE WHEN ISNULL(pr.RECEIPT_TYPE_ID, 1) = 2 THEN 1 ELSE 0 END AS BIT) AS IsServiceReceipt,
        CAST(CASE WHEN ISNULL(pr.RECEIPT_TYPE_ID, 1) = 1 THEN 1 ELSE 0 END AS BIT) AS IsWarehouseReceipt,
        pr.CREATE_DATE AS CreateDate
    FROM dbo.PURCHASE_RECEIPT pr
        INNER JOIN dbo.SUPPLIERS s ON s.SUPPLIERS_ID = pr.SUPPLIERS_ID
        INNER JOIN dbo.WAREHOUSES w ON w.WAREHOUSES_ID = pr.WAREHOUSE_ID
        INNER JOIN dbo.PURCHASE_RECEIPT_STATUS prs ON prs.PURCHASE_RECEIPT_STATUS_ID = pr.RECEIPT_STATUS_ID
        LEFT JOIN dbo.PURCHASE_ORDER po ON po.PURCHASE_ORDER_ID = pr.PURCHASE_ORDER_ID
    WHERE pr.BUSINESS_ID = @BusinessId
        AND pr.PURCHASE_RECEIPT_ID = @PurchaseReceiptId;

    SELECT
        prd.PURCHASE_RECEIPT_DETAIL_ID AS PurchaseReceiptDetailId,
        prd.PURCHASE_RECEIPT_ID AS PurchaseReceiptId,
        prd.PURCHASE_ORDER_DETAIL_ID AS PurchaseOrderDetailId,
        prd.PRODUCTS_ID AS ProductsId,
        p.DESCRIPTION AS ProductName,
        p.SKU AS ProductCode,
        prd.UOM_ID AS UomId,
        u.DESCRIPTION AS UomName,
        prd.ORDERED_QUANTITY AS OrderedQuantity,
        prd.RECEIVED_QUANTITY AS ReceivedQuantity,
        prd.UNIT_COST AS UnitCost,
        prd.TOTAL_COST AS TotalCost,
        prd.OBSERVATION AS Observation
    FROM dbo.PURCHASE_RECEIPT_DETAIL prd
        INNER JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = prd.PRODUCTS_ID
        LEFT JOIN dbo.UNIT_OF_MEASURE u ON u.UOM_ID = prd.UOM_ID
    WHERE prd.BUSINESS_ID = @BusinessId
        AND prd.PURCHASE_RECEIPT_ID = @PurchaseReceiptId
        AND prd.IS_ACTIVE = 1
    ORDER BY prd.PURCHASE_RECEIPT_DETAIL_ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_CORE_RECALCULATE_PURCHASE_ORDER_RECEPTION_STATUS
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ApprovedStatusId BIGINT;
    DECLARE @PartialStatusId BIGINT;
    DECLARE @CompleteStatusId BIGINT;
    DECLARE @NewStatusId BIGINT;
    DECLARE @CurrentStatusCode VARCHAR(50);
    DECLARE @TotalLines INT = 0;
    DECLARE @LinesWithAnyReceipt INT = 0;
    DECLARE @FullLines INT = 0;

    SELECT @CurrentStatusCode = UPPER(pos.CODE)
    FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
    WHERE po.BUSINESS_ID = @BusinessId
        AND po.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND po.STATUS = '1';

    IF @CurrentStatusCode IS NULL
        RETURN;

    IF @CurrentStatusCode IN ('CLOSED', 'CANCELLED', 'REJECTED')
        RETURN;

    UPDATE pod
        SET RECEIVED_QUANTITY = ISNULL(r.RECEIVED_QUANTITY, 0),
            UPDATE_DATE = GETDATE()
    FROM dbo.PURCHASE_ORDER_DETAIL pod
        OUTER APPLY (
            SELECT SUM(prd.RECEIVED_QUANTITY) AS RECEIVED_QUANTITY
        FROM dbo.PURCHASE_RECEIPT_DETAIL prd
            INNER JOIN dbo.PURCHASE_RECEIPT pr ON pr.PURCHASE_RECEIPT_ID = prd.PURCHASE_RECEIPT_ID
                AND pr.BUSINESS_ID = prd.BUSINESS_ID
        WHERE prd.BUSINESS_ID = pod.BUSINESS_ID
            AND prd.PURCHASE_ORDER_DETAIL_ID = pod.PURCHASE_ORDER_DETAIL_ID
            AND prd.IS_ACTIVE = 1
            AND pr.IS_ACTIVE = 1
        ) r
    WHERE pod.BUSINESS_ID = @BusinessId
        AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND pod.IS_ACTIVE = 1;

    SELECT
        @TotalLines = COUNT(1),
        @LinesWithAnyReceipt = SUM(CASE WHEN ISNULL(pod.RECEIVED_QUANTITY, 0) > 0 THEN 1 ELSE 0 END),
        @FullLines = SUM(CASE WHEN ISNULL(pod.RECEIVED_QUANTITY, 0) >= pod.QUANTITY THEN 1 ELSE 0 END)
    FROM dbo.PURCHASE_ORDER_DETAIL pod
    WHERE pod.BUSINESS_ID = @BusinessId
        AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND pod.IS_ACTIVE = 1;

    IF @TotalLines = 0
        RETURN;

    SELECT @ApprovedStatusId = MAX(CASE WHEN UPPER(CODE) = 'APPROVED' THEN PURCHASE_ORDER_STATUS_ID END),
        @PartialStatusId = MAX(CASE WHEN UPPER(CODE) = 'PARTIALLY_RECEIVED' THEN PURCHASE_ORDER_STATUS_ID END),
        @CompleteStatusId = MAX(CASE WHEN UPPER(CODE) IN ('FULL_RECEIVED', 'FULLY_RECEIVED') THEN PURCHASE_ORDER_STATUS_ID END)
    FROM dbo.PURCHASE_ORDER_STATUS
    WHERE BUSINESS_ID = @BusinessId
        AND IS_ACTIVE = 1
        AND UPPER(CODE) IN ('APPROVED', 'PARTIALLY_RECEIVED', 'FULL_RECEIVED', 'FULLY_RECEIVED');

    IF @ApprovedStatusId IS NULL
        THROW 51000, 'No existe estado APPROVED activo para orden de compra.', 1;
    IF @PartialStatusId IS NULL
        THROW 51000, 'No existe estado PARTIALLY_RECEIVED activo para orden de compra.', 1;
    IF @CompleteStatusId IS NULL
        THROW 51000, 'No existe estado FULL_RECEIVED activo para orden de compra.', 1;

    SET @NewStatusId = CASE
        WHEN @FullLines = @TotalLines THEN @CompleteStatusId
        WHEN @LinesWithAnyReceipt > 0 THEN @PartialStatusId
        WHEN @CurrentStatusCode = 'SENT_TO_SUPPLIER' THEN NULL
        WHEN @LinesWithAnyReceipt = 0 THEN @ApprovedStatusId
        ELSE @PartialStatusId
    END;

    IF @NewStatusId IS NOT NULL
    BEGIN
        UPDATE dbo.PURCHASE_ORDER
        SET PURCHASE_ORDER_STATUS_ID = @NewStatusId,
            UPDATE_DATE = GETDATE()
        WHERE BUSINESS_ID = @BusinessId
            AND PURCHASE_ORDER_ID = @PurchaseOrderId;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_VOID_PURCHASE_RECEIPT
    @BusinessId BIGINT,
    @PurchaseReceiptId BIGINT,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF;

    BEGIN TRY
        DECLARE @VoidStatusId BIGINT;
        DECLARE @PurchaseOrderId BIGINT;
        DECLARE @RegularizedPurchaseOrderId BIGINT;
        DECLARE @WarehouseId BIGINT;
        DECLARE @WarehouseMovementId BIGINT;
        DECLARE @ReceiptNumber VARCHAR(30);
        DECLARE @ReceiptTypeId BIGINT;
        DECLARE @ReceiptStatusId BIGINT;
        DECLARE @IsActive BIT;
        DECLARE @IsWithoutPurchaseOrder BIT;
        DECLARE @IsRegularized BIT;
        DECLARE @MovementTypeId BIGINT;

        SELECT
        @PurchaseOrderId = PURCHASE_ORDER_ID,
        @RegularizedPurchaseOrderId = REGULARIZED_PURCHASE_ORDER_ID,
        @WarehouseId = WAREHOUSE_ID,
        @WarehouseMovementId = WAREHOUSE_MOVEMENT_ID,
        @ReceiptNumber = RECEIPT_NUMBER,
        @ReceiptTypeId = ISNULL(RECEIPT_TYPE_ID, 1),
        @ReceiptStatusId = RECEIPT_STATUS_ID,
        @IsActive = IS_ACTIVE,
        @IsWithoutPurchaseOrder = IS_WITHOUT_PURCHASE_ORDER,
        @IsRegularized = IS_REGULARIZED
    FROM dbo.PURCHASE_RECEIPT WITH (UPDLOCK, HOLDLOCK)
    WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_RECEIPT_ID = @PurchaseReceiptId;

        IF @ReceiptTypeId IS NULL
            THROW 51000, 'La recepcion de compra no existe o no pertenece a la empresa.', 1;

        IF ISNULL(@IsActive, 0) = 0
            THROW 51000, 'La recepcion ya se encuentra anulada.', 1;

        EXEC dbo.SP_WS_GET_PURCHASE_RECEIPT_STATUS_ID @BusinessId, 'VOID', @VoidStatusId OUTPUT;
        IF @VoidStatusId IS NULL SET @VoidStatusId = 5;

        IF @ReceiptTypeId = 1
        BEGIN
        IF @WarehouseMovementId IS NULL
                THROW 51000, 'La recepcion de compra no tiene movimiento de almacen para revertir.', 1;

        SELECT TOP (1)
            @MovementTypeId = MOVEMENT_TYPE_ID
        FROM dbo.WAREHOUSE_MOVEMENT
        WHERE WAREHOUSE_MOVEMENT_ID = @WarehouseMovementId;

        IF @MovementTypeId IS NULL
                EXEC dbo.SP_CORE_GET_PURCHASE_ENTRY_MOVEMENT_TYPE_ID @BusinessId, @MovementTypeId OUTPUT;

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
                SELECT PRODUCTS_ID, RECEIVED_QUANTITY, UNIT_COST, TOTAL_COST, OBSERVATION
        FROM dbo.PURCHASE_RECEIPT_DETAIL
        WHERE BUSINESS_ID = @BusinessId
            AND PURCHASE_RECEIPT_ID = @PurchaseReceiptId
            AND IS_ACTIVE = 1;

        DECLARE @ProductsId BIGINT;
        DECLARE @Quantity DECIMAL(18,4);
        DECLARE @UnitCost DECIMAL(18,4);
        DECLARE @TotalCost DECIMAL(18,4);
        DECLARE @Observation VARCHAR(500);
        DECLARE @PreviousStock DECIMAL(18,4);
        DECLARE @FinalStock DECIMAL(18,4);
        DECLARE @AverageCost DECIMAL(18,4);
        DECLARE @VoidTotalCost DECIMAL(18,4);
        DECLARE @VoidMovementDate DATETIME;
        DECLARE @KardexId BIGINT;
        DECLARE @KardexCOutput INT;
        DECLARE @KardexSOutput VARCHAR(500);

        OPEN cur;
        FETCH NEXT FROM cur INTO @ProductsId, @Quantity, @UnitCost, @TotalCost, @Observation;

        WHILE @@FETCH_STATUS = 0
            BEGIN
            SELECT @PreviousStock = ISNULL(STOCK_QUANTITY, 0)
            FROM dbo.INVENTORY_STOCK WITH (UPDLOCK, HOLDLOCK)
            WHERE BUSINESS_ID = @BusinessId
                AND WAREHOUSE_ID = @WarehouseId
                AND PRODUCTS_ID = @ProductsId;

            IF ISNULL(@PreviousStock, 0) < @Quantity
                    THROW 51000, 'No se puede anular porque generaria stock negativo.', 1;

            EXEC dbo.SP_WS_DECREASE_INVENTORY_STOCK
                    @BusinessId = @BusinessId,
                    @WarehouseId = @WarehouseId,
                    @ProductsId = @ProductsId,
                    @Quantity = @Quantity,
                    @UserId = @UserId,
                    @AllowNegative = 0;

            SELECT
                @FinalStock = STOCK_QUANTITY,
                @AverageCost = AVERAGE_COST
            FROM dbo.INVENTORY_STOCK
            WHERE BUSINESS_ID = @BusinessId
                AND WAREHOUSE_ID = @WarehouseId
                AND PRODUCTS_ID = @ProductsId;

            SET @VoidTotalCost = @Quantity * @AverageCost;
            SET @VoidMovementDate = GETDATE();

            EXEC dbo.SP_WS_REGISTER_INVENTORY_KARDEX
                    @BusinessId = @BusinessId,
                    @WarehouseId = @WarehouseId,
                    @ProductsId = @ProductsId,
                    @WareHouseMovementId = @WarehouseMovementId,
                    @WareHouseMovementDetailId = 0,
                    @MovementTypesId = @MovementTypeId,
                    @MovementDate = @VoidMovementDate,
                    @EntryQuantity = 0,
                    @ExitQuantity = @Quantity,
                    @PreviousStock = @PreviousStock,
                    @FinalStock = @FinalStock,
                    @UnitCost = @AverageCost,
                    @AverageCost = @AverageCost,
                    @TotalCost = @VoidTotalCost,
                    @ReferenceDocumentType = 'PURCHASE_RECEIPT_VOID',
                    @ReferenceDocumentNumber = @ReceiptNumber,
                    @Observation = @Observation,
                    @CreateUser = @UserId,
                    @Id = @KardexId OUTPUT,
                    @COutput = @KardexCOutput OUTPUT,
                    @SOutput = @KardexSOutput OUTPUT;

            IF ISNULL(@KardexCOutput, 0) <> 1
                BEGIN
                DECLARE @KardexMessage NVARCHAR(2048) = ISNULL(@KardexSOutput, 'No se pudo registrar kardex de anulacion.');
                THROW 51000, @KardexMessage, 1;
            END

            FETCH NEXT FROM cur INTO @ProductsId, @Quantity, @UnitCost, @TotalCost, @Observation;
        END

        CLOSE cur;
        DEALLOCATE cur;
    END
        ELSE IF @ReceiptTypeId = 2
        BEGIN
        IF @WarehouseMovementId IS NOT NULL
                THROW 51000, 'La conformidad de servicio no tiene movimiento de almacen para revertir.', 1;
    END
        ELSE
            THROW 51000, 'El tipo de recepcion no es valido.', 1;

        UPDATE dbo.PURCHASE_RECEIPT_DETAIL
        SET IS_ACTIVE = 0,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_RECEIPT_ID = @PurchaseReceiptId
        AND IS_ACTIVE = 1;

        UPDATE dbo.PURCHASE_RECEIPT
        SET IS_ACTIVE = 0,
            RECEIPT_STATUS_ID = @VoidStatusId,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_RECEIPT_ID = @PurchaseReceiptId;

        IF @PurchaseOrderId IS NOT NULL
            EXEC dbo.SP_CORE_RECALCULATE_PURCHASE_ORDER_RECEPTION_STATUS @BusinessId, @PurchaseOrderId;

        IF @RegularizedPurchaseOrderId IS NOT NULL
        AND (@PurchaseOrderId IS NULL OR @RegularizedPurchaseOrderId <> @PurchaseOrderId)
            EXEC dbo.SP_CORE_RECALCULATE_PURCHASE_ORDER_RECEPTION_STATUS @BusinessId, @RegularizedPurchaseOrderId;

        SET @COutput = 1;
        SET @SOutput = CASE
            WHEN @ReceiptTypeId = 2 THEN 'Conformidad de servicio anulada correctamente.'
            ELSE 'Recepcion de compra anulada correctamente.'
        END;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'cur') >= -1
        BEGIN
        IF CURSOR_STATUS('local', 'cur') > -1 CLOSE cur;
        DEALLOCATE cur;
    END
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_REGULARIZE_PURCHASE_RECEIPT_WITH_PO
    @BusinessId BIGINT,
    @PurchaseReceiptId BIGINT,
    @PurchaseOrderId BIGINT,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF;

    BEGIN TRY
        DECLARE @RegularizedStatusId BIGINT;
        DECLARE @ReceiptTypeId BIGINT;
        DECLARE @ReceiptPurchaseOrderId BIGINT;
        DECLARE @ReceiptSupplierId BIGINT;
        DECLARE @IsActive BIT;
        DECLARE @IsWithoutPurchaseOrder BIT;
        DECLARE @IsRegularized BIT;
        DECLARE @PoSupplierId BIGINT;

        SELECT
        @ReceiptTypeId = ISNULL(RECEIPT_TYPE_ID, 1),
        @ReceiptPurchaseOrderId = PURCHASE_ORDER_ID,
        @ReceiptSupplierId = SUPPLIERS_ID,
        @IsActive = IS_ACTIVE,
        @IsWithoutPurchaseOrder = IS_WITHOUT_PURCHASE_ORDER,
        @IsRegularized = IS_REGULARIZED
    FROM dbo.PURCHASE_RECEIPT WITH (UPDLOCK, HOLDLOCK)
    WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_RECEIPT_ID = @PurchaseReceiptId;

        IF @ReceiptTypeId IS NULL
            THROW 51000, 'La recepcion de compra no existe o no pertenece a la empresa.', 1;

        IF ISNULL(@IsActive, 0) = 0
            THROW 51000, 'La recepcion ya se encuentra anulada.', 1;

        IF @ReceiptTypeId = 2
            THROW 51000, 'La regularizacion posterior solo aplica a recepciones de mercaderia sin OC.', 1;

        IF @ReceiptTypeId <> 1 OR ISNULL(@IsWithoutPurchaseOrder, 0) <> 1 OR @ReceiptPurchaseOrderId IS NOT NULL
            THROW 51000, 'La regularizacion posterior solo aplica a recepciones de mercaderia sin OC.', 1;

        IF ISNULL(@IsRegularized, 0) = 1
            THROW 51000, 'La recepcion ya fue regularizada.', 1;

        SELECT @PoSupplierId = SUPPLIERS_ID
    FROM dbo.PURCHASE_ORDER
    WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_ORDER_ID = @PurchaseOrderId
        AND STATUS = '1'
        AND IS_REGULARIZATION = 1;

        IF @PoSupplierId IS NULL
            THROW 51000, 'La OC de regularizacion no existe o no pertenece a la empresa.', 1;

        IF @PoSupplierId <> @ReceiptSupplierId
            THROW 51000, 'La OC de regularizacion pertenece a otro proveedor.', 1;

        IF EXISTS (
            SELECT 1
    FROM dbo.PURCHASE_ORDER_DETAIL
    WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_ORDER_ID = @PurchaseOrderId
        AND IS_ACTIVE = 1
    GROUP BY PRODUCTS_ID
    HAVING COUNT(1) > 1
        )
            THROW 51000, 'Los productos de la OC no coinciden con los productos recibidos.', 1;

        IF EXISTS (
            SELECT 1
    FROM dbo.PURCHASE_RECEIPT_DETAIL prd
        LEFT JOIN dbo.PURCHASE_ORDER_DETAIL pod ON pod.PURCHASE_ORDER_ID = @PurchaseOrderId
            AND pod.BUSINESS_ID = @BusinessId
            AND pod.PRODUCTS_ID = prd.PRODUCTS_ID
            AND pod.IS_ACTIVE = 1
    WHERE prd.BUSINESS_ID = @BusinessId
        AND prd.PURCHASE_RECEIPT_ID = @PurchaseReceiptId
        AND prd.IS_ACTIVE = 1
        AND pod.PURCHASE_ORDER_DETAIL_ID IS NULL
        )
            THROW 51000, 'Los productos de la OC no coinciden con los productos recibidos.', 1;

        IF EXISTS (
            SELECT 1
    FROM dbo.PURCHASE_ORDER_DETAIL pod
        LEFT JOIN dbo.PURCHASE_RECEIPT_DETAIL prd ON prd.PURCHASE_RECEIPT_ID = @PurchaseReceiptId
            AND prd.BUSINESS_ID = @BusinessId
            AND prd.PRODUCTS_ID = pod.PRODUCTS_ID
            AND prd.IS_ACTIVE = 1
    WHERE pod.BUSINESS_ID = @BusinessId
        AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND pod.IS_ACTIVE = 1
        AND prd.PURCHASE_RECEIPT_DETAIL_ID IS NULL
        )
            THROW 51000, 'Los productos de la OC no coinciden con los productos recibidos.', 1;

        IF EXISTS (
            SELECT 1
    FROM dbo.PURCHASE_ORDER_DETAIL pod
        INNER JOIN dbo.PURCHASE_RECEIPT_DETAIL prd ON prd.PURCHASE_RECEIPT_ID = @PurchaseReceiptId
            AND prd.BUSINESS_ID = @BusinessId
            AND prd.PRODUCTS_ID = pod.PRODUCTS_ID
            AND prd.IS_ACTIVE = 1
    WHERE pod.BUSINESS_ID = @BusinessId
        AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND pod.IS_ACTIVE = 1
        AND pod.QUANTITY > prd.RECEIVED_QUANTITY
        )
            THROW 51000, 'Las cantidades de la OC exceden las cantidades recibidas.', 1;

        UPDATE prd
        SET PURCHASE_ORDER_DETAIL_ID = pod.PURCHASE_ORDER_DETAIL_ID,
            ORDERED_QUANTITY = pod.QUANTITY,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        FROM dbo.PURCHASE_RECEIPT_DETAIL prd
        INNER JOIN dbo.PURCHASE_ORDER_DETAIL pod ON pod.PURCHASE_ORDER_ID = @PurchaseOrderId
            AND pod.BUSINESS_ID = @BusinessId
            AND pod.PRODUCTS_ID = prd.PRODUCTS_ID
            AND pod.IS_ACTIVE = 1
        WHERE prd.BUSINESS_ID = @BusinessId
        AND prd.PURCHASE_RECEIPT_ID = @PurchaseReceiptId
        AND prd.IS_ACTIVE = 1;

        EXEC dbo.SP_WS_GET_PURCHASE_RECEIPT_STATUS_ID @BusinessId, 'REGULARIZED', @RegularizedStatusId OUTPUT;
        IF @RegularizedStatusId IS NULL SET @RegularizedStatusId = 4;

        UPDATE dbo.PURCHASE_RECEIPT
        SET REGULARIZED_PURCHASE_ORDER_ID = @PurchaseOrderId,
            IS_REGULARIZED = 1,
            REGULARIZED_DATE = GETDATE(),
            REGULARIZED_USER = @UserId,
            RECEIPT_STATUS_ID = @RegularizedStatusId,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE BUSINESS_ID = @BusinessId
        AND PURCHASE_RECEIPT_ID = @PurchaseReceiptId;

        EXEC dbo.SP_CORE_RECALCULATE_PURCHASE_ORDER_RECEPTION_STATUS @BusinessId, @PurchaseOrderId;

        SET @COutput = 1;
        SET @SOutput = 'Recepcion de compra regularizada correctamente.';
    END TRY
    BEGIN CATCH
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

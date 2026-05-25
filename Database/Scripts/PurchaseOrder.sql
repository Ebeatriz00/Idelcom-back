
IF COL_LENGTH('dbo.PURCHASE_ORDER_DETAIL', 'PRICE_INCLUDES_TAX') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_ORDER_DETAIL
    ADD PRICE_INCLUDES_TAX BIT NOT NULL
        CONSTRAINT DF_PURCHASE_ORDER_DETAIL_PRICE_INCLUDES_TAX DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.PURCHASE_ORDER', 'IS_REGULARIZATION') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_ORDER
    ADD IS_REGULARIZATION BIT NOT NULL
        CONSTRAINT DF_PURCHASE_ORDER_IS_REGULARIZATION DEFAULT (0),
        REGULARIZATION_REASON VARCHAR(500) NULL,
        REGULARIZED_BY BIGINT NULL,
        REGULARIZATION_DATE DATETIME NULL;
END
GO

IF COL_LENGTH('dbo.PURCHASE_ORDER', 'SUPPLIER_QUOTATION_REFERENCE_NUMBER') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_ORDER
    ADD SUPPLIER_QUOTATION_REFERENCE_NUMBER VARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.PURCHASE_ORDER', 'REFERENCE_NOTES') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_ORDER
    ADD REFERENCE_NOTES VARCHAR(500) NULL;
END
GO

IF OBJECT_ID(N'dbo.PURCHASE_ORDER_INVOICE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PURCHASE_ORDER_INVOICE
    (
        PURCHASE_ORDER_INVOICE_ID BIGINT IDENTITY(1,1) NOT NULL,
        BUSINESS_ID BIGINT NOT NULL,
        PURCHASE_ORDER_ID BIGINT NOT NULL,
        SUPPLIER_INVOICE_ID BIGINT NOT NULL,
        OBSERVATION VARCHAR(500) NOT NULL CONSTRAINT DF_PURCHASE_ORDER_INVOICE_OBSERVATION DEFAULT (''),
        STATUS CHAR(1) NOT NULL CONSTRAINT DF_PURCHASE_ORDER_INVOICE_STATUS DEFAULT ('1'),
        CREATE_DATE DATETIME NOT NULL CONSTRAINT DF_PURCHASE_ORDER_INVOICE_CREATE_DATE DEFAULT (GETDATE()),
        CREATE_USER BIGINT NULL,
        UPDATE_DATE DATETIME NULL,
        UPDATE_USER BIGINT NULL,
        CONSTRAINT PK_PURCHASE_ORDER_INVOICE PRIMARY KEY (PURCHASE_ORDER_INVOICE_ID),
        CONSTRAINT FK_PURCHASE_ORDER_INVOICE_PURCHASE_ORDER FOREIGN KEY (PURCHASE_ORDER_ID) REFERENCES dbo.PURCHASE_ORDER(PURCHASE_ORDER_ID),
        CONSTRAINT FK_PURCHASE_ORDER_INVOICE_SUPPLIER_INVOICE FOREIGN KEY (SUPPLIER_INVOICE_ID) REFERENCES dbo.SUPPLIER_INVOICE(SUPPLIER_INVOICE_ID)
    );
END
GO

IF COL_LENGTH('dbo.PURCHASE_ORDER_INVOICE', 'OBSERVATION') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_ORDER_INVOICE
    ADD OBSERVATION VARCHAR(500) NOT NULL
        CONSTRAINT DF_PURCHASE_ORDER_INVOICE_OBSERVATION DEFAULT ('');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PURCHASE_ORDER_INVOICE_ACTIVE' AND object_id = OBJECT_ID(N'dbo.PURCHASE_ORDER_INVOICE'))
BEGIN
    CREATE UNIQUE INDEX UX_PURCHASE_ORDER_INVOICE_ACTIVE
    ON dbo.PURCHASE_ORDER_INVOICE (BUSINESS_ID, PURCHASE_ORDER_ID, SUPPLIER_INVOICE_ID)
    WHERE STATUS = '1';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PURCHASE_ORDER_INVOICE_PURCHASE_ORDER' AND object_id = OBJECT_ID(N'dbo.PURCHASE_ORDER_INVOICE'))
BEGIN
    CREATE INDEX IX_PURCHASE_ORDER_INVOICE_PURCHASE_ORDER
    ON dbo.PURCHASE_ORDER_INVOICE (BUSINESS_ID, PURCHASE_ORDER_ID, STATUS);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PURCHASE_ORDER_INVOICE_SUPPLIER_INVOICE' AND object_id = OBJECT_ID(N'dbo.PURCHASE_ORDER_INVOICE'))
BEGIN
    CREATE INDEX IX_PURCHASE_ORDER_INVOICE_SUPPLIER_INVOICE
    ON dbo.PURCHASE_ORDER_INVOICE (BUSINESS_ID, SUPPLIER_INVOICE_ID, STATUS);
END
GO

IF TYPE_ID(N'dbo.PurchaseOrderDetailInputType') IS NOT NULL
BEGIN
    DROP PROCEDURE IF EXISTS dbo.SP_WS_REGISTER_PURCHASE_ORDER;
    DROP PROCEDURE IF EXISTS dbo.SP_WS_UPDATE_PURCHASE_ORDER;
    DROP PROCEDURE IF EXISTS dbo.SP_WS_VALIDATE_PURCHASE_ORDER_DETAILS;
    DROP PROCEDURE IF EXISTS dbo.SP_WS_CALCULATE_PURCHASE_ORDER_TOTALS;
    DROP PROCEDURE IF EXISTS dbo.SP_WS_INSERT_PURCHASE_ORDER_DETAILS;
    DROP PROCEDURE IF EXISTS dbo.SP_WS_SYNC_PURCHASE_ORDER_DETAILS;
    DROP PROCEDURE IF EXISTS dbo.SP_WS_CREATE_PURCHASE_ORDER_FROM_INVOICE;
    DROP TYPE dbo.PurchaseOrderDetailInputType;
END
GO

IF TYPE_ID(N'dbo.PurchaseOrderDetailInputType') IS NULL
BEGIN
    CREATE TYPE dbo.PurchaseOrderDetailInputType AS TABLE
    (
        PURCHASE_ORDER_DETAIL_ID BIGINT NULL,
        PRODUCTS_ID BIGINT NOT NULL,
        UOM_ID BIGINT NULL,
        QUANTITY DECIMAL(18,4) NOT NULL,
        UNIT_PRICE DECIMAL(18,4) NOT NULL,
        DISCOUNT_PERCENT DECIMAL(18,4) NOT NULL,
        TAXES_ID BIGINT NULL,
        PRICE_INCLUDES_TAX BIT NOT NULL,
        OBSERVATION VARCHAR(500) NULL,
        IS_ACTIVE BIT NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.PURCHASE_ORDER_CORRELATIVE', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PURCHASE_ORDER_CORRELATIVE
    (
        BUSINESS_ID BIGINT NOT NULL,
        PERIOD_YEAR INT NOT NULL,
        PREFIX VARCHAR(10) NOT NULL CONSTRAINT DF_PURCHASE_ORDER_CORRELATIVE_PREFIX DEFAULT ('OC'),
        CURRENT_NUMBER BIGINT NOT NULL,
        CREATE_DATE DATETIME NOT NULL CONSTRAINT DF_PURCHASE_ORDER_CORRELATIVE_CREATE_DATE DEFAULT (GETDATE()),
        UPDATE_DATE DATETIME NULL,
        CONSTRAINT PK_PURCHASE_ORDER_CORRELATIVE PRIMARY KEY (BUSINESS_ID, PERIOD_YEAR)
    );
END
GO

IF COL_LENGTH('dbo.PURCHASE_ORDER_CORRELATIVE', 'PERIOD_YEAR') IS NULL
BEGIN
    ALTER TABLE dbo.PURCHASE_ORDER_CORRELATIVE ADD PERIOD_YEAR INT NULL;

    UPDATE dbo.PURCHASE_ORDER_CORRELATIVE
    SET PERIOD_YEAR = YEAR(GETDATE())
    WHERE PERIOD_YEAR IS NULL;

    IF EXISTS (
        SELECT 1
    FROM sys.key_constraints
    WHERE name = 'PK_PURCHASE_ORDER_CORRELATIVE'
        AND parent_object_id = OBJECT_ID(N'dbo.PURCHASE_ORDER_CORRELATIVE')
    )
        ALTER TABLE dbo.PURCHASE_ORDER_CORRELATIVE DROP CONSTRAINT PK_PURCHASE_ORDER_CORRELATIVE;

    ALTER TABLE dbo.PURCHASE_ORDER_CORRELATIVE ALTER COLUMN PERIOD_YEAR INT NOT NULL;

    ALTER TABLE dbo.PURCHASE_ORDER_CORRELATIVE
    ADD CONSTRAINT PK_PURCHASE_ORDER_CORRELATIVE PRIMARY KEY (BUSINESS_ID, PERIOD_YEAR);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_ORDER_STATUS_ID
    @BusinessId BIGINT,
    @StatusCode VARCHAR(50),
    @PurchaseOrderStatusId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        @PurchaseOrderStatusId = PURCHASE_ORDER_STATUS_ID
    FROM dbo.PURCHASE_ORDER_STATUS
    WHERE BUSINESS_ID = @BusinessId
        AND STATUS = '1'
        AND (
            UPPER(CODE) = UPPER(@StatusCode)
            OR (@StatusCode = 'DRAFT' AND (UPPER(DESCRIPTION) LIKE '%BORRADOR%' OR UPPER(DESCRIPTION) LIKE '%DRAFT%'))
            OR (@StatusCode = 'PENDING_APPROVAL' AND UPPER(DESCRIPTION) LIKE '%PENDIENTE%')
        OR (@StatusCode = 'APPROVED' AND UPPER(DESCRIPTION) LIKE '%APROB%')
        OR (@StatusCode = 'CANCELLED' AND (UPPER(DESCRIPTION) LIKE '%ANUL%' OR UPPER(DESCRIPTION) LIKE '%CANCEL%'))
          )
    ORDER BY PURCHASE_ORDER_STATUS_ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_ORDER_DETAIL_STATUS_ID
    @BusinessId BIGINT,
    @StatusCode VARCHAR(50),
    @DetailStatusId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        @DetailStatusId = PURCHASE_ORDER_DETAIL_STATUS_ID
    FROM dbo.PURCHASE_ORDER_DETAIL_STATUS
    WHERE BUSINESS_ID = @BusinessId
        AND STATUS = '1'
        AND (
            UPPER(CODE) = UPPER(@StatusCode)
        OR (@StatusCode = 'PENDING' AND UPPER(DESCRIPTION) LIKE '%PENDIENTE%')
        OR (@StatusCode = 'CANCELLED' AND (UPPER(DESCRIPTION) LIKE '%ANUL%' OR UPPER(DESCRIPTION) LIKE '%CANCEL%'))
          )
    ORDER BY PURCHASE_ORDER_DETAIL_STATUS_ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_VALIDATE_PURCHASE_ORDER_DETAILS
    @BusinessId BIGINT,
    @Details dbo.PurchaseOrderDetailInputType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1
    FROM @Details
    WHERE IS_ACTIVE = 1)
        RAISERROR('Debe registrar al menos un detalle activo.', 16, 1);

    IF EXISTS (SELECT 1
    FROM @Details
    WHERE IS_ACTIVE = 1 AND QUANTITY <= 0)
        RAISERROR('La cantidad debe ser mayor a cero.', 16, 1);

    IF EXISTS (SELECT 1
    FROM @Details
    WHERE IS_ACTIVE = 1 AND UNIT_PRICE < 0)
        RAISERROR('El precio unitario no puede ser negativo.', 16, 1);

    IF EXISTS (
        SELECT 1
    FROM @Details d
        LEFT JOIN dbo.PRODUCTS p
        ON p.PRODUCTS_ID = d.PRODUCTS_ID
            AND p.BUSINESS_ID = @BusinessId
            AND p.STATUS = '1'
    WHERE d.IS_ACTIVE = 1
        AND p.PRODUCTS_ID IS NULL
    )
        RAISERROR('Uno o mas productos no existen o no estan activos.', 16, 1);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_CALCULATE_PURCHASE_ORDER_TOTALS
    @BusinessId BIGINT,
    @Details dbo.PurchaseOrderDetailInputType READONLY,
    @Subtotal DECIMAL(18,4) OUTPUT,
    @DiscountAmount DECIMAL(18,4) OUTPUT,
    @TaxAmount DECIMAL(18,4) OUTPUT,
    @Total DECIMAL(18,4) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    ;
    WITH
        DetailCalc
        AS
        (
            SELECT
                d.PRICE_INCLUDES_TAX AS PriceIncludesTax,
                ROUND(d.QUANTITY * d.UNIT_PRICE * d.DISCOUNT_PERCENT / 100.0, 4) AS DiscountAmount,
                ROUND((d.QUANTITY * d.UNIT_PRICE) - (d.QUANTITY * d.UNIT_PRICE * d.DISCOUNT_PERCENT / 100.0), 4) AS LineAmount,
                CAST(ISNULL(t.PERCENTAGE, 0) AS DECIMAL(18,4)) AS IgvPercent
            FROM @Details d
                LEFT JOIN dbo.TAXES t
                ON t.TAXES_ID = d.TAXES_ID
                    AND t.BUSINESS_ID = @BusinessId
                    AND t.STATUS = '1'
            WHERE d.IS_ACTIVE = 1
        ),
        DetailTotals
        AS
        (
            SELECT
                DiscountAmount,
                CASE
                WHEN PriceIncludesTax = 1 AND IgvPercent > 0 THEN ROUND(LineAmount / (1 + (IgvPercent / 100.0)), 4)
                ELSE ROUND(LineAmount, 4)
            END AS Subtotal,
                CASE
                WHEN PriceIncludesTax = 1 AND IgvPercent > 0 THEN ROUND(LineAmount - ROUND(LineAmount / (1 + (IgvPercent / 100.0)), 4), 4)
                WHEN IgvPercent > 0 THEN ROUND(LineAmount * IgvPercent / 100.0, 4)
                ELSE 0
            END AS TaxAmount,
                CASE
                WHEN PriceIncludesTax = 1 THEN ROUND(LineAmount, 4)
                ELSE ROUND(LineAmount + (LineAmount * IgvPercent / 100.0), 4)
            END AS Total
            FROM DetailCalc
        )
    SELECT
        @Subtotal = ISNULL(SUM(Subtotal), 0),
        @DiscountAmount = ISNULL(SUM(DiscountAmount), 0),
        @TaxAmount = ISNULL(SUM(TaxAmount), 0),
        @Total = ISNULL(SUM(Total), 0)
    FROM DetailTotals;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_NEXT_PURCHASE_ORDER_NUMBER
    @BusinessId BIGINT,
    @PurchaseOrderNumber VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextNumber BIGINT, @Prefix VARCHAR(10), @PeriodYear INT, @ShortYear CHAR(2);

    SET @PeriodYear = YEAR(GETDATE());
    SET @ShortYear = RIGHT(CONVERT(CHAR(4), @PeriodYear), 2);

    UPDATE dbo.PURCHASE_ORDER_CORRELATIVE WITH (UPDLOCK, HOLDLOCK)
    SET CURRENT_NUMBER = CURRENT_NUMBER + 1,
        @NextNumber = CURRENT_NUMBER + 1,
        @Prefix = PREFIX,
        UPDATE_DATE = GETDATE()
    WHERE BUSINESS_ID = @BusinessId
        AND PERIOD_YEAR = @PeriodYear;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT @NextNumber = ISNULL(MAX(TRY_CONVERT(BIGINT, SUBSTRING(PURCHASE_ORDER_NUMBER, 6, 20))), 0) + 1
        FROM dbo.PURCHASE_ORDER WITH (UPDLOCK, HOLDLOCK)
        WHERE BUSINESS_ID = @BusinessId
            AND PURCHASE_ORDER_NUMBER LIKE CONCAT('OC-', @ShortYear, '%');

        SET @Prefix = 'OC';

        BEGIN TRY
            INSERT INTO dbo.PURCHASE_ORDER_CORRELATIVE
            (
            BUSINESS_ID, PERIOD_YEAR, PREFIX, CURRENT_NUMBER, CREATE_DATE
            )
        VALUES
            (
                @BusinessId, @PeriodYear, @Prefix, @NextNumber, GETDATE()
            );
        END TRY
        BEGIN CATCH
            IF ERROR_NUMBER() NOT IN (2601, 2627)
                THROW;

            UPDATE dbo.PURCHASE_ORDER_CORRELATIVE WITH (UPDLOCK, HOLDLOCK)
            SET CURRENT_NUMBER = CURRENT_NUMBER + 1,
                @NextNumber = CURRENT_NUMBER + 1,
                @Prefix = PREFIX,
                UPDATE_DATE = GETDATE()
            WHERE BUSINESS_ID = @BusinessId
            AND PERIOD_YEAR = @PeriodYear;
        END CATCH
    END

    SET @PurchaseOrderNumber = CONCAT(@Prefix, '-', @ShortYear, RIGHT(CONCAT('00000', @NextNumber), 5));
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_INSERT_PURCHASE_ORDER_DETAILS
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @DetailStatusId BIGINT,
    @UserId BIGINT,
    @Details dbo.PurchaseOrderDetailInputType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    ;
    WITH
        DetailCalc
        AS
        (
            SELECT
                d.PRODUCTS_ID,
                d.UOM_ID,
                d.QUANTITY,
                d.UNIT_PRICE,
                d.DISCOUNT_PERCENT,
                d.TAXES_ID,
                d.PRICE_INCLUDES_TAX,
                d.OBSERVATION,
                ROUND(d.QUANTITY * d.UNIT_PRICE * d.DISCOUNT_PERCENT / 100.0, 4) AS DISCOUNT_AMOUNT,
                ROUND((d.QUANTITY * d.UNIT_PRICE) - (d.QUANTITY * d.UNIT_PRICE * d.DISCOUNT_PERCENT / 100.0), 4) AS LINE_AMOUNT,
                CAST(ISNULL(t.PERCENTAGE, 0) AS DECIMAL(18,4)) AS IGV_PERCENT
            FROM @Details d
                LEFT JOIN dbo.TAXES t
                ON t.TAXES_ID = d.TAXES_ID
                    AND t.BUSINESS_ID = @BusinessId
                    AND t.STATUS = '1'
            WHERE d.IS_ACTIVE = 1
        ),
        DetailTotals
        AS
        (
            SELECT
                PRODUCTS_ID,
                UOM_ID,
                QUANTITY,
                UNIT_PRICE,
                DISCOUNT_PERCENT,
                TAXES_ID,
                PRICE_INCLUDES_TAX,
                OBSERVATION,
                DISCOUNT_AMOUNT,
                IGV_PERCENT,
                CASE
                WHEN PRICE_INCLUDES_TAX = 1 AND IGV_PERCENT > 0 THEN ROUND(LINE_AMOUNT / (1 + (IGV_PERCENT / 100.0)), 4)
                ELSE ROUND(LINE_AMOUNT, 4)
            END AS SUBTOTAL,
                CASE
                WHEN PRICE_INCLUDES_TAX = 1 AND IGV_PERCENT > 0 THEN ROUND(LINE_AMOUNT - ROUND(LINE_AMOUNT / (1 + (IGV_PERCENT / 100.0)), 4), 4)
                WHEN IGV_PERCENT > 0 THEN ROUND(LINE_AMOUNT * IGV_PERCENT / 100.0, 4)
                ELSE 0
            END AS IGV_AMOUNT,
                CASE
                WHEN PRICE_INCLUDES_TAX = 1 THEN ROUND(LINE_AMOUNT, 4)
                ELSE ROUND(LINE_AMOUNT + (LINE_AMOUNT * IGV_PERCENT / 100.0), 4)
            END AS TOTAL
            FROM DetailCalc
        )
    INSERT INTO dbo.PURCHASE_ORDER_DETAIL
        (
        BUSINESS_ID, PURCHASE_ORDER_ID, PRODUCTS_ID, UOM_ID, QUANTITY, UNIT_PRICE,
        DISCOUNT_PERCENT, DISCOUNT_AMOUNT, TAXES_ID, PRICE_INCLUDES_TAX, IGV_PERCENT, IGV_AMOUNT,
        SUBTOTAL, TOTAL, RECEIVED_QUANTITY, DETAIL_STATUS_ID, OBSERVATION,
        IS_ACTIVE, CREATE_USER, CREATE_DATE
        )
    SELECT
        @BusinessId, @PurchaseOrderId, PRODUCTS_ID, UOM_ID, QUANTITY, UNIT_PRICE,
        DISCOUNT_PERCENT, DISCOUNT_AMOUNT, TAXES_ID, PRICE_INCLUDES_TAX, IGV_PERCENT, IGV_AMOUNT,
        SUBTOTAL, TOTAL, 0, @DetailStatusId, OBSERVATION,
        1, @UserId, GETDATE()
    FROM DetailTotals;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_SYNC_PURCHASE_ORDER_DETAILS
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @DetailStatusId BIGINT,
    @UserId BIGINT,
    @Details dbo.PurchaseOrderDetailInputType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    ;
    WITH
        DetailCalc
        AS
        (
            SELECT
                d.PURCHASE_ORDER_DETAIL_ID,
                d.PRODUCTS_ID,
                d.UOM_ID,
                d.QUANTITY,
                d.UNIT_PRICE,
                d.DISCOUNT_PERCENT,
                d.TAXES_ID,
                d.PRICE_INCLUDES_TAX,
                d.OBSERVATION,
                d.IS_ACTIVE,
                ROUND(d.QUANTITY * d.UNIT_PRICE * d.DISCOUNT_PERCENT / 100.0, 4) AS DISCOUNT_AMOUNT,
                ROUND((d.QUANTITY * d.UNIT_PRICE) - (d.QUANTITY * d.UNIT_PRICE * d.DISCOUNT_PERCENT / 100.0), 4) AS LINE_AMOUNT,
                CAST(ISNULL(t.PERCENTAGE, 0) AS DECIMAL(18,4)) AS IGV_PERCENT
            FROM @Details d
                LEFT JOIN dbo.TAXES t
                ON t.TAXES_ID = d.TAXES_ID
                    AND t.BUSINESS_ID = @BusinessId
                    AND t.STATUS = '1'
        ),
        DetailTotals
        AS
        (
            SELECT
                PURCHASE_ORDER_DETAIL_ID,
                PRODUCTS_ID,
                UOM_ID,
                QUANTITY,
                UNIT_PRICE,
                DISCOUNT_PERCENT,
                TAXES_ID,
                PRICE_INCLUDES_TAX,
                OBSERVATION,
                IS_ACTIVE,
                DISCOUNT_AMOUNT,
                IGV_PERCENT,
                CASE
                WHEN PRICE_INCLUDES_TAX = 1 AND IGV_PERCENT > 0 THEN ROUND(LINE_AMOUNT / (1 + (IGV_PERCENT / 100.0)), 4)
                ELSE ROUND(LINE_AMOUNT, 4)
            END AS SUBTOTAL,
                CASE
                WHEN PRICE_INCLUDES_TAX = 1 AND IGV_PERCENT > 0 THEN ROUND(LINE_AMOUNT - ROUND(LINE_AMOUNT / (1 + (IGV_PERCENT / 100.0)), 4), 4)
                WHEN IGV_PERCENT > 0 THEN ROUND(LINE_AMOUNT * IGV_PERCENT / 100.0, 4)
                ELSE 0
            END AS IGV_AMOUNT,
                CASE
                WHEN PRICE_INCLUDES_TAX = 1 THEN ROUND(LINE_AMOUNT, 4)
                ELSE ROUND(LINE_AMOUNT + (LINE_AMOUNT * IGV_PERCENT / 100.0), 4)
            END AS TOTAL
            FROM DetailCalc
        )
    SELECT *
    INTO #DetailCalc
    FROM DetailTotals;

    UPDATE pod
    SET IS_ACTIVE = 0,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    FROM dbo.PURCHASE_ORDER_DETAIL pod
    WHERE pod.BUSINESS_ID = @BusinessId
        AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND NOT EXISTS (
          SELECT 1
        FROM #DetailCalc d
        WHERE d.PURCHASE_ORDER_DETAIL_ID = pod.PURCHASE_ORDER_DETAIL_ID
      );

    UPDATE pod
    SET PRODUCTS_ID = d.PRODUCTS_ID,
        UOM_ID = d.UOM_ID,
        QUANTITY = d.QUANTITY,
        UNIT_PRICE = d.UNIT_PRICE,
        DISCOUNT_PERCENT = d.DISCOUNT_PERCENT,
        DISCOUNT_AMOUNT = d.DISCOUNT_AMOUNT,
        TAXES_ID = d.TAXES_ID,
        PRICE_INCLUDES_TAX = d.PRICE_INCLUDES_TAX,
        IGV_PERCENT = d.IGV_PERCENT,
        IGV_AMOUNT = d.IGV_AMOUNT,
        SUBTOTAL = d.SUBTOTAL,
        TOTAL = d.TOTAL,
        OBSERVATION = d.OBSERVATION,
        IS_ACTIVE = d.IS_ACTIVE,
        UPDATE_USER = @UserId,
        UPDATE_DATE = GETDATE()
    FROM dbo.PURCHASE_ORDER_DETAIL pod
        INNER JOIN #DetailCalc d ON d.PURCHASE_ORDER_DETAIL_ID = pod.PURCHASE_ORDER_DETAIL_ID
    WHERE pod.BUSINESS_ID = @BusinessId
        AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND d.PURCHASE_ORDER_DETAIL_ID IS NOT NULL;

    INSERT INTO dbo.PURCHASE_ORDER_DETAIL
        (
        BUSINESS_ID, PURCHASE_ORDER_ID, PRODUCTS_ID, UOM_ID, QUANTITY, UNIT_PRICE,
        DISCOUNT_PERCENT, DISCOUNT_AMOUNT, TAXES_ID, PRICE_INCLUDES_TAX, IGV_PERCENT, IGV_AMOUNT,
        SUBTOTAL, TOTAL, RECEIVED_QUANTITY, DETAIL_STATUS_ID, OBSERVATION,
        IS_ACTIVE, CREATE_USER, CREATE_DATE
        )
    SELECT
        @BusinessId, @PurchaseOrderId, PRODUCTS_ID, UOM_ID, QUANTITY, UNIT_PRICE,
        DISCOUNT_PERCENT, DISCOUNT_AMOUNT, TAXES_ID, PRICE_INCLUDES_TAX, IGV_PERCENT, IGV_AMOUNT,
        SUBTOTAL, TOTAL, 0, @DetailStatusId, OBSERVATION,
        1, @UserId, GETDATE()
    FROM #DetailCalc
    WHERE PURCHASE_ORDER_DETAIL_ID IS NULL
        AND IS_ACTIVE = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_REGISTER_PURCHASE_ORDER
    @BusinessId BIGINT,
    @SuppliersId BIGINT,
    @PurchaseOrderDate DATETIME,
    @CurrencyId BIGINT,
    @ExchangeRate DECIMAL(18,6) = NULL,
    @PmConditionId BIGINT = NULL,
    @ExpectedDeliveryDate DATETIME = NULL,
    @WarehouseId BIGINT = NULL,
    @SupplierQuotationReferenceNumber VARCHAR(100) = NULL,
    @References VARCHAR(500) = NULL,
    @Observation VARCHAR(500) = NULL,
    @UserId BIGINT,
    @Details dbo.PurchaseOrderDetailInputType READONLY,
    @Id BIGINT OUTPUT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @PurchaseOrderStatusId BIGINT, @DetailStatusId BIGINT, @PurchaseOrderNumber VARCHAR(20);
        DECLARE @Subtotal DECIMAL(18,4), @DiscountAmount DECIMAL(18,4), @TaxAmount DECIMAL(18,4), @Total DECIMAL(18,4);

        IF ISNULL(@BusinessId, 0) <= 0 RAISERROR('La empresa es obligatoria.', 16, 1);
        IF ISNULL(@SuppliersId, 0) <= 0 RAISERROR('El proveedor es obligatorio.', 16, 1);
        IF ISNULL(@CurrencyId, 0) <= 0 RAISERROR('La moneda es obligatoria.', 16, 1);
        IF @PurchaseOrderDate IS NULL RAISERROR('La fecha de orden de compra es obligatoria.', 16, 1);
        IF NOT EXISTS (SELECT 1
    FROM dbo.SUPPLIERS
    WHERE SUPPLIERS_ID = @SuppliersId AND BUSINESS_ID = @BusinessId AND STATUS = '1')
            RAISERROR('El proveedor no existe o no esta activo.', 16, 1);

        EXEC dbo.SP_WS_VALIDATE_PURCHASE_ORDER_DETAILS @BusinessId, @Details;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_STATUS_ID @BusinessId, 'DRAFT', @PurchaseOrderStatusId OUTPUT;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_DETAIL_STATUS_ID @BusinessId, 'PENDING', @DetailStatusId OUTPUT;
        EXEC dbo.SP_WS_GET_NEXT_PURCHASE_ORDER_NUMBER @BusinessId, @PurchaseOrderNumber OUTPUT;
        EXEC dbo.SP_WS_CALCULATE_PURCHASE_ORDER_TOTALS @BusinessId, @Details, @Subtotal OUTPUT, @DiscountAmount OUTPUT, @TaxAmount OUTPUT, @Total OUTPUT;

        IF @PurchaseOrderStatusId IS NULL RAISERROR('No existe estado DRAFT para orden de compra.', 16, 1);
        IF @DetailStatusId IS NULL RAISERROR('No existe estado PENDING para detalle de orden de compra.', 16, 1);

        INSERT INTO dbo.PURCHASE_ORDER
        (
        BUSINESS_ID, SUPPLIERS_ID, PURCHASE_ORDER_NUMBER, PURCHASE_ORDER_DATE, CURRENCY_ID,
        EXCHANGE_RATE, PM_CONDITION_ID, EXPECTED_DELIVERY_DATE, WAREHOUSE_ID,
        SUPPLIER_QUOTATION_REFERENCE_NUMBER, REFERENCE_NOTES,
        SUBTOTAL, DISCOUNT_AMOUNT, TAX_AMOUNT, TOTAL, PURCHASE_ORDER_STATUS_ID,
        OBSERVATION, REQUESTED_BY, CREATE_USER, CREATE_DATE, STATUS
        )
    VALUES
        (
            @BusinessId, @SuppliersId, @PurchaseOrderNumber, @PurchaseOrderDate, @CurrencyId,
            @ExchangeRate, @PmConditionId, @ExpectedDeliveryDate, @WarehouseId,
            @SupplierQuotationReferenceNumber, @References,
            @Subtotal, @DiscountAmount, @TaxAmount, @Total, @PurchaseOrderStatusId,
            @Observation, @UserId, @UserId, GETDATE(), '1'
        );

        SET @Id = SCOPE_IDENTITY();
        EXEC dbo.SP_WS_INSERT_PURCHASE_ORDER_DETAILS @BusinessId, @Id, @DetailStatusId, @UserId, @Details;

        SET @COutput = 1;
        SET @SOutput = 'Orden de compra registrada correctamente.';
    END TRY
    BEGIN CATCH
        SET @Id = 0;
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_LIST_PURCHASE_ORDER
    @BusinessId BIGINT,
    @SuppliersId BIGINT = NULL,
    @PurchaseOrderStatusId BIGINT = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL,
    @Search VARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        po.PURCHASE_ORDER_ID AS PurchaseOrderId,
        po.PURCHASE_ORDER_NUMBER AS PurchaseOrderNumber,
        po.PURCHASE_ORDER_DATE AS PurchaseOrderDate,
        po.SUPPLIERS_ID AS SuppliersId,
        s.SUPPLIER_NAME AS SupplierName,
        s.DOCUMENT_NUMBER AS SupplierDocumentNumber,
        po.CURRENCY_ID AS CurrencyId,
        c.DESCRIPTION AS CurrencyDescription,
        po.PURCHASE_ORDER_STATUS_ID AS PurchaseOrderStatusId,
        pos.DESCRIPTION AS StatusDescription,
        po.IS_REGULARIZATION AS IsRegularization,
        po.SUBTOTAL AS Subtotal,
        po.TAX_AMOUNT AS TaxAmount,
        po.TOTAL AS Total,
        po.EXPECTED_DELIVERY_DATE AS ExpectedDeliveryDate,
        COUNT(1) OVER() AS TotalRecords
    FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.SUPPLIERS s ON s.SUPPLIERS_ID = po.SUPPLIERS_ID
        INNER JOIN dbo.CURRENCY c ON c.CURRENCY_ID = po.CURRENCY_ID
        INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
    WHERE po.BUSINESS_ID = @BusinessId
        AND po.STATUS = '1'
        AND (@SuppliersId IS NULL OR po.SUPPLIERS_ID = @SuppliersId)
        AND (@PurchaseOrderStatusId IS NULL OR po.PURCHASE_ORDER_STATUS_ID = @PurchaseOrderStatusId)
        AND (@DateFrom IS NULL OR po.PURCHASE_ORDER_DATE >= @DateFrom)
        AND (@DateTo IS NULL OR po.PURCHASE_ORDER_DATE < DATEADD(DAY, 1, @DateTo))
        AND (
            NULLIF(LTRIM(RTRIM(@Search)), '') IS NULL
        OR po.PURCHASE_ORDER_NUMBER LIKE '%' + @Search + '%'
        OR s.SUPPLIER_NAME LIKE '%' + @Search + '%'
        OR s.DOCUMENT_NUMBER LIKE '%' + @Search + '%'
          )
    ORDER BY po.PURCHASE_ORDER_DATE DESC, po.PURCHASE_ORDER_ID DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_ORDER_BY_ID
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        po.PURCHASE_ORDER_ID AS PurchaseOrderId,
        po.BUSINESS_ID AS BusinessId,
        po.PURCHASE_ORDER_NUMBER AS PurchaseOrderNumber,
        po.SUPPLIERS_ID AS SuppliersId,
        s.SUPPLIER_NAME AS SupplierName,
        s.DOCUMENT_NUMBER AS SupplierDocumentNumber,
        po.PURCHASE_ORDER_DATE AS PurchaseOrderDate,
        po.CURRENCY_ID AS CurrencyId,
        c.DESCRIPTION AS CurrencyDescription,
        po.EXCHANGE_RATE AS ExchangeRate,
        po.PM_CONDITION_ID AS PmConditionId,
        pc.DESCRIPTION AS PmConditionDescription,
        po.EXPECTED_DELIVERY_DATE AS ExpectedDeliveryDate,
        po.WAREHOUSE_ID AS WarehouseId,
        w.DESCRIPTION AS WarehouseDescription,
        po.SUPPLIER_QUOTATION_REFERENCE_NUMBER AS SupplierQuotationReferenceNumber,
        po.REFERENCE_NOTES AS [References],
        po.SUBTOTAL AS Subtotal,
        po.DISCOUNT_AMOUNT AS DiscountAmount,
        po.TAX_AMOUNT AS TaxAmount,
        po.TOTAL AS Total,
        po.PURCHASE_ORDER_STATUS_ID AS PurchaseOrderStatusId,
        pos.DESCRIPTION AS StatusDescription,
        po.IS_REGULARIZATION AS IsRegularization,
        po.REGULARIZATION_REASON AS RegularizationReason,
        po.REGULARIZED_BY AS RegularizedBy,
        po.REGULARIZATION_DATE AS RegularizationDate,
        po.OBSERVATION AS Observation,
        po.REQUESTED_BY AS RequestedBy,
        po.APPROVED_BY AS ApprovedBy,
        po.APPROVED_AT AS ApprovedAt
    FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.SUPPLIERS s ON s.SUPPLIERS_ID = po.SUPPLIERS_ID
        INNER JOIN dbo.CURRENCY c ON c.CURRENCY_ID = po.CURRENCY_ID
        INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
        LEFT JOIN dbo.PM_CONDITION pc ON pc.PM_CONDITION_ID = po.PM_CONDITION_ID
        LEFT JOIN dbo.WAREHOUSES w ON w.WAREHOUSES_ID = po.WAREHOUSE_ID
    WHERE po.BUSINESS_ID = @BusinessId
        AND po.PURCHASE_ORDER_ID = @PurchaseOrderId;

    SELECT
        pod.PURCHASE_ORDER_DETAIL_ID AS PurchaseOrderDetailId,
        pod.PRODUCTS_ID AS ProductsId,
        p.DESCRIPTION AS ProductDescription,
        pod.UOM_ID AS UomId,
        u.DESCRIPTION AS UomDescription,
        pod.QUANTITY AS Quantity,
        pod.RECEIVED_QUANTITY AS ReceivedQuantity,
        pod.QUANTITY - ISNULL(pod.RECEIVED_QUANTITY, 0) AS PendingQuantity,
        pod.UNIT_PRICE AS UnitPrice,
        pod.DISCOUNT_PERCENT AS DiscountPercent,
        pod.DISCOUNT_AMOUNT AS DiscountAmount,
        pod.TAXES_ID AS TaxesId,
        pod.PRICE_INCLUDES_TAX AS PriceIncludesTax,
        pod.IGV_PERCENT AS IgvPercent,
        pod.IGV_AMOUNT AS IgvAmount,
        pod.SUBTOTAL AS Subtotal,
        pod.TOTAL AS Total,
        pod.DETAIL_STATUS_ID AS DetailStatusId,
        pods.DESCRIPTION AS DetailStatusDescription,
        pod.OBSERVATION AS Observation,
        pod.IS_ACTIVE AS IsActive
    FROM dbo.PURCHASE_ORDER_DETAIL pod
        INNER JOIN dbo.PRODUCTS p ON p.PRODUCTS_ID = pod.PRODUCTS_ID
        LEFT JOIN dbo.UOM u ON u.UOM_ID = pod.UOM_ID
        LEFT JOIN dbo.PURCHASE_ORDER_DETAIL_STATUS pods ON pods.PURCHASE_ORDER_DETAIL_STATUS_ID = pod.DETAIL_STATUS_ID
    WHERE pod.BUSINESS_ID = @BusinessId
      AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
    ORDER BY pod.PURCHASE_ORDER_DETAIL_ID;

    SELECT
        poi.PURCHASE_ORDER_INVOICE_ID AS PurchaseOrderInvoiceId,
        poi.BUSINESS_ID AS BusinessId,
        poi.PURCHASE_ORDER_ID AS PurchaseOrderId,
        poi.SUPPLIER_INVOICE_ID AS SupplierInvoiceId,
        poi.OBSERVATION AS Observation,
        CONCAT(si.SERIES, '-', si.CORRELATIVE) AS SupplierInvoiceNumber,
        si.ISSUE_DATE AS SupplierInvoiceDate,
        si.TOTAL AS SupplierInvoiceTotal,
        poi.STATUS AS Status
    FROM dbo.PURCHASE_ORDER_INVOICE poi
    INNER JOIN dbo.SUPPLIER_INVOICE si ON si.SUPPLIER_INVOICE_ID = poi.SUPPLIER_INVOICE_ID
    WHERE poi.BUSINESS_ID = @BusinessId
      AND poi.PURCHASE_ORDER_ID = @PurchaseOrderId
      AND poi.STATUS = '1'
    ORDER BY poi.PURCHASE_ORDER_INVOICE_ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_UPDATE_PURCHASE_ORDER
    @PurchaseOrderId BIGINT,
    @BusinessId BIGINT,
    @SuppliersId BIGINT,
    @PurchaseOrderDate DATETIME,
    @CurrencyId BIGINT,
    @ExchangeRate DECIMAL(18,6) = NULL,
    @PmConditionId BIGINT = NULL,
    @ExpectedDeliveryDate DATETIME = NULL,
    @WarehouseId BIGINT = NULL,
    @SupplierQuotationReferenceNumber VARCHAR(100) = NULL,
    @References VARCHAR(500) = NULL,
    @Observation VARCHAR(500) = NULL,
    @UserId BIGINT,
    @Details dbo.PurchaseOrderDetailInputType READONLY,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @DetailStatusId BIGINT;
        DECLARE @Subtotal DECIMAL(18,4), @DiscountAmount DECIMAL(18,4), @TaxAmount DECIMAL(18,4), @Total DECIMAL(18,4);

        IF NOT EXISTS (SELECT 1
    FROM dbo.PURCHASE_ORDER
    WHERE PURCHASE_ORDER_ID = @PurchaseOrderId AND BUSINESS_ID = @BusinessId)
            RAISERROR('La orden de compra no existe.', 16, 1);

        IF EXISTS (
            SELECT 1
    FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
    WHERE po.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND po.BUSINESS_ID = @BusinessId
        AND (po.STATUS = '0' OR UPPER(pos.CODE) IN ('APPROVED','PARTIALLY_RECEIVED','FULLY_RECEIVED','CLOSED','CANCELLED','REJECTED'))
        )
            RAISERROR('Solo se puede actualizar una orden en borrador o pendiente de aprobacion.', 16, 1);

        EXEC dbo.SP_WS_VALIDATE_PURCHASE_ORDER_DETAILS @BusinessId, @Details;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_DETAIL_STATUS_ID @BusinessId, 'PENDING', @DetailStatusId OUTPUT;
        EXEC dbo.SP_WS_CALCULATE_PURCHASE_ORDER_TOTALS @BusinessId, @Details, @Subtotal OUTPUT, @DiscountAmount OUTPUT, @TaxAmount OUTPUT, @Total OUTPUT;
        EXEC dbo.SP_WS_SYNC_PURCHASE_ORDER_DETAILS @BusinessId, @PurchaseOrderId, @DetailStatusId, @UserId, @Details;

        UPDATE dbo.PURCHASE_ORDER
        SET SUPPLIERS_ID = @SuppliersId,
            PURCHASE_ORDER_DATE = @PurchaseOrderDate,
            CURRENCY_ID = @CurrencyId,
            EXCHANGE_RATE = @ExchangeRate,
            PM_CONDITION_ID = @PmConditionId,
            EXPECTED_DELIVERY_DATE = @ExpectedDeliveryDate,
            WAREHOUSE_ID = @WarehouseId,
            SUPPLIER_QUOTATION_REFERENCE_NUMBER = @SupplierQuotationReferenceNumber,
            REFERENCE_NOTES = @References,
            OBSERVATION = @Observation,
            SUBTOTAL = @Subtotal,
            DISCOUNT_AMOUNT = @DiscountAmount,
            TAX_AMOUNT = @TaxAmount,
            TOTAL = @Total,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE PURCHASE_ORDER_ID = @PurchaseOrderId
        AND BUSINESS_ID = @BusinessId;

        SET @COutput = 1;
        SET @SOutput = 'Orden de compra actualizada correctamente.';
    END TRY
    BEGIN CATCH
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_ORDER_INVOICE_BY_ID
    @BusinessId BIGINT,
    @PurchaseOrderInvoiceId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        poi.PURCHASE_ORDER_INVOICE_ID AS PurchaseOrderInvoiceId,
        poi.BUSINESS_ID AS BusinessId,
        poi.PURCHASE_ORDER_ID AS PurchaseOrderId,
        poi.SUPPLIER_INVOICE_ID AS SupplierInvoiceId,
        poi.OBSERVATION AS Observation,
        CONCAT(si.SERIES, '-', si.CORRELATIVE) AS SupplierInvoiceNumber,
        si.ISSUE_DATE AS SupplierInvoiceDate,
        si.TOTAL AS SupplierInvoiceTotal,
        poi.STATUS AS Status
    FROM dbo.PURCHASE_ORDER_INVOICE poi
    INNER JOIN dbo.SUPPLIER_INVOICE si ON si.SUPPLIER_INVOICE_ID = poi.SUPPLIER_INVOICE_ID
    WHERE poi.BUSINESS_ID = @BusinessId
      AND poi.PURCHASE_ORDER_INVOICE_ID = @PurchaseOrderInvoiceId;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_ORDER_INVOICE_BY_PAIR
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @SupplierInvoiceId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        poi.PURCHASE_ORDER_INVOICE_ID AS PurchaseOrderInvoiceId,
        poi.BUSINESS_ID AS BusinessId,
        poi.PURCHASE_ORDER_ID AS PurchaseOrderId,
        poi.SUPPLIER_INVOICE_ID AS SupplierInvoiceId,
        poi.OBSERVATION AS Observation,
        CONCAT(si.SERIES, '-', si.CORRELATIVE) AS SupplierInvoiceNumber,
        si.ISSUE_DATE AS SupplierInvoiceDate,
        si.TOTAL AS SupplierInvoiceTotal,
        poi.STATUS AS Status
    FROM dbo.PURCHASE_ORDER_INVOICE poi
    INNER JOIN dbo.SUPPLIER_INVOICE si ON si.SUPPLIER_INVOICE_ID = poi.SUPPLIER_INVOICE_ID
    WHERE poi.BUSINESS_ID = @BusinessId
      AND poi.PURCHASE_ORDER_ID = @PurchaseOrderId
      AND poi.SUPPLIER_INVOICE_ID = @SupplierInvoiceId
      AND poi.STATUS = '1'
    ORDER BY poi.PURCHASE_ORDER_INVOICE_ID DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_ATTACH_PURCHASE_ORDER_INVOICE
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @SupplierInvoiceId BIGINT,
    @RegularizationReason VARCHAR(500) = NULL,
    @UserId BIGINT,
    @Id BIGINT OUTPUT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @PurchaseOrderDate DATE, @SupplierInvoiceDate DATE, @PurchaseOrderSupplierId BIGINT, @InvoiceSupplierId BIGINT;
        DECLARE @PurchaseOrderStatusCode VARCHAR(50);

        IF ISNULL(@BusinessId, 0) <= 0 RAISERROR('La empresa es obligatoria.', 16, 1);
        IF ISNULL(@PurchaseOrderId, 0) <= 0 RAISERROR('La orden de compra es obligatoria.', 16, 1);
        IF ISNULL(@SupplierInvoiceId, 0) <= 0 RAISERROR('La factura proveedor es obligatoria.', 16, 1);

        SELECT
            @PurchaseOrderDate = po.PURCHASE_ORDER_DATE,
            @PurchaseOrderSupplierId = po.SUPPLIERS_ID,
            @PurchaseOrderStatusCode = UPPER(pos.CODE)
        FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
        WHERE po.BUSINESS_ID = @BusinessId
          AND po.PURCHASE_ORDER_ID = @PurchaseOrderId
          AND po.STATUS = '1';

        IF @PurchaseOrderDate IS NULL RAISERROR('La orden de compra no existe o no esta activa.', 16, 1);
        IF @PurchaseOrderStatusCode IN ('CANCELLED','REJECTED','FULLY_RECEIVED','CLOSED','INVOICED')
            RAISERROR('No se puede asociar factura a una orden anulada, cerrada, recibida o facturada.', 16, 1);

        SELECT
            @SupplierInvoiceDate = si.ISSUE_DATE,
            @InvoiceSupplierId = si.SUPPLIERS_ID
        FROM dbo.SUPPLIER_INVOICE si
        WHERE si.BUSINESS_ID = @BusinessId
          AND si.SUPPLIER_INVOICE_ID = @SupplierInvoiceId
          AND si.IS_ACTIVE = 1;

        IF @SupplierInvoiceDate IS NULL RAISERROR('La factura proveedor no existe o no esta activa.', 16, 1);
        IF @PurchaseOrderSupplierId <> @InvoiceSupplierId RAISERROR('El proveedor de la factura no coincide con el proveedor de la orden de compra.', 16, 1);

        IF EXISTS (
            SELECT 1
            FROM dbo.PURCHASE_ORDER_INVOICE
            WHERE BUSINESS_ID = @BusinessId
              AND PURCHASE_ORDER_ID = @PurchaseOrderId
              AND SUPPLIER_INVOICE_ID = @SupplierInvoiceId
              AND STATUS = '1'
        )
            RAISERROR('La factura proveedor ya esta asociada a esta orden de compra.', 16, 1);

        IF @PurchaseOrderDate > @SupplierInvoiceDate AND NULLIF(LTRIM(RTRIM(@RegularizationReason)), '') IS NULL
            RAISERROR('Debe ingresar el motivo de regularizacion porque la factura tiene fecha anterior a la orden de compra.', 16, 1);

        INSERT INTO dbo.PURCHASE_ORDER_INVOICE
        (
            BUSINESS_ID, PURCHASE_ORDER_ID, SUPPLIER_INVOICE_ID, OBSERVATION, STATUS, CREATE_DATE, CREATE_USER
        )
        VALUES
        (
            @BusinessId, @PurchaseOrderId, @SupplierInvoiceId, ISNULL(@RegularizationReason, ''), '1', GETDATE(), @UserId
        );

        SET @Id = SCOPE_IDENTITY();

        IF @PurchaseOrderDate > @SupplierInvoiceDate
        BEGIN
            UPDATE dbo.PURCHASE_ORDER
            SET IS_REGULARIZATION = 1,
                REGULARIZATION_REASON = @RegularizationReason,
                REGULARIZED_BY = @UserId,
                REGULARIZATION_DATE = GETDATE(),
                UPDATE_USER = @UserId,
                UPDATE_DATE = GETDATE()
            WHERE BUSINESS_ID = @BusinessId
              AND PURCHASE_ORDER_ID = @PurchaseOrderId;
        END

        SET @COutput = 1;
        SET @SOutput = 'Factura proveedor asociada correctamente a la orden de compra.';
    END TRY
    BEGIN CATCH
        SET @Id = 0;
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_CREATE_PURCHASE_ORDER_FROM_INVOICE
    @BusinessId BIGINT,
    @SupplierInvoiceId BIGINT,
    @WarehouseId BIGINT = NULL,
    @Observation VARCHAR(500) = NULL,
    @UserId BIGINT,
    @Id BIGINT OUTPUT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @SuppliersId BIGINT, @InvoiceDate DATE, @CurrencyId BIGINT, @ExchangeRate DECIMAL(18,6), @InvoiceObservation VARCHAR(500);
        DECLARE @PurchaseOrderStatusId BIGINT, @DetailStatusId BIGINT, @PurchaseOrderNumber VARCHAR(20);
        DECLARE @Subtotal DECIMAL(18,4), @DiscountAmount DECIMAL(18,4), @TaxAmount DECIMAL(18,4), @Total DECIMAL(18,4);
        DECLARE @Details dbo.PurchaseOrderDetailInputType;

        IF ISNULL(@BusinessId, 0) <= 0 RAISERROR('La empresa es obligatoria.', 16, 1);
        IF ISNULL(@SupplierInvoiceId, 0) <= 0 RAISERROR('La factura proveedor es obligatoria.', 16, 1);

        SELECT
            @SuppliersId = si.SUPPLIERS_ID,
            @InvoiceDate = si.ISSUE_DATE,
            @CurrencyId = si.CURRENCY_ID,
            @ExchangeRate = si.EXCHANGE_RATE,
            @InvoiceObservation = si.OBSERVATION
        FROM dbo.SUPPLIER_INVOICE si
        WHERE si.BUSINESS_ID = @BusinessId
          AND si.SUPPLIER_INVOICE_ID = @SupplierInvoiceId
          AND si.IS_ACTIVE = 1;

        IF @SuppliersId IS NULL RAISERROR('La factura proveedor no existe o no esta activa.', 16, 1);

        INSERT INTO @Details
        (
            PURCHASE_ORDER_DETAIL_ID, PRODUCTS_ID, UOM_ID, QUANTITY, UNIT_PRICE,
            DISCOUNT_PERCENT, TAXES_ID, PRICE_INCLUDES_TAX, OBSERVATION, IS_ACTIVE
        )
        SELECT
            NULL,
            sid.PRODUCTS_ID,
            sid.UOM_ID,
            sid.QUANTITY,
            sid.UNIT_PRICE,
            ISNULL(sid.DISCOUNT_PERCENT, 0),
            sid.TAXES_ID,
            1,
            NULL,
            1
        FROM dbo.SUPPLIER_INVOICE_DETAIL sid
        WHERE sid.BUSINESS_ID = @BusinessId
          AND sid.SUPPLIER_INVOICE_ID = @SupplierInvoiceId
          AND sid.IS_ACTIVE = 1;

        EXEC dbo.SP_WS_VALIDATE_PURCHASE_ORDER_DETAILS @BusinessId, @Details;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_STATUS_ID @BusinessId, 'DRAFT', @PurchaseOrderStatusId OUTPUT;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_DETAIL_STATUS_ID @BusinessId, 'PENDING', @DetailStatusId OUTPUT;
        EXEC dbo.SP_WS_GET_NEXT_PURCHASE_ORDER_NUMBER @BusinessId, @PurchaseOrderNumber OUTPUT;
        EXEC dbo.SP_WS_CALCULATE_PURCHASE_ORDER_TOTALS @BusinessId, @Details, @Subtotal OUTPUT, @DiscountAmount OUTPUT, @TaxAmount OUTPUT, @Total OUTPUT;

        IF @PurchaseOrderStatusId IS NULL RAISERROR('No existe estado DRAFT para orden de compra.', 16, 1);
        IF @DetailStatusId IS NULL RAISERROR('No existe estado PENDING para detalle de orden de compra.', 16, 1);

        INSERT INTO dbo.PURCHASE_ORDER
        (
            BUSINESS_ID, SUPPLIERS_ID, PURCHASE_ORDER_NUMBER, PURCHASE_ORDER_DATE, CURRENCY_ID,
            EXCHANGE_RATE, PM_CONDITION_ID, EXPECTED_DELIVERY_DATE, WAREHOUSE_ID,
            SUBTOTAL, DISCOUNT_AMOUNT, TAX_AMOUNT, TOTAL, PURCHASE_ORDER_STATUS_ID,
            OBSERVATION, REQUESTED_BY, IS_REGULARIZATION, REGULARIZATION_REASON,
            REGULARIZED_BY, REGULARIZATION_DATE, CREATE_USER, CREATE_DATE, STATUS
        )
        VALUES
        (
            @BusinessId, @SuppliersId, @PurchaseOrderNumber, CAST(GETDATE() AS DATE), @CurrencyId,
            @ExchangeRate, NULL, NULL, @WarehouseId,
            @Subtotal, @DiscountAmount, @TaxAmount, @Total, @PurchaseOrderStatusId,
            COALESCE(NULLIF(@Observation, ''), @InvoiceObservation), @UserId, 1, 'OC creada desde factura proveedor para regularizacion administrativa.',
            @UserId, GETDATE(), @UserId, GETDATE(), '1'
        );

        SET @Id = SCOPE_IDENTITY();

        EXEC dbo.SP_WS_INSERT_PURCHASE_ORDER_DETAILS @BusinessId, @Id, @DetailStatusId, @UserId, @Details;

        INSERT INTO dbo.PURCHASE_ORDER_INVOICE
        (
            BUSINESS_ID, PURCHASE_ORDER_ID, SUPPLIER_INVOICE_ID, OBSERVATION, STATUS, CREATE_DATE, CREATE_USER
        )
        VALUES
        (
            @BusinessId, @Id, @SupplierInvoiceId, 'OC creada desde factura proveedor para regularizacion administrativa.', '1', GETDATE(), @UserId
        );

        SET @COutput = 1;
        SET @SOutput = 'Orden de compra creada desde factura proveedor correctamente.';
    END TRY
    BEGIN CATCH
        SET @Id = 0;
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_SEND_PURCHASE_ORDER_FOR_APPROVAL
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @PurchaseOrderStatusId BIGINT;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_STATUS_ID @BusinessId, 'PENDING_APPROVAL', @PurchaseOrderStatusId OUTPUT;

        IF @PurchaseOrderStatusId IS NULL RAISERROR('No existe estado PENDING_APPROVAL para orden de compra.', 16, 1);

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.PURCHASE_ORDER po
                INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
            WHERE po.PURCHASE_ORDER_ID = @PurchaseOrderId
              AND po.BUSINESS_ID = @BusinessId
              AND po.STATUS = '1'
              AND (UPPER(pos.CODE) = 'DRAFT' OR UPPER(pos.DESCRIPTION) LIKE '%BORRADOR%')
        )
            RAISERROR('Solo se puede enviar a aprobacion una orden en borrador.', 16, 1);

        UPDATE dbo.PURCHASE_ORDER
        SET PURCHASE_ORDER_STATUS_ID = @PurchaseOrderStatusId,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE PURCHASE_ORDER_ID = @PurchaseOrderId
          AND BUSINESS_ID = @BusinessId;

        SET @COutput = 1;
        SET @SOutput = 'Orden de compra enviada a aprobacion correctamente.';
    END TRY
    BEGIN CATCH
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_APPROVE_PURCHASE_ORDER
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @ApprovedBy BIGINT,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @PurchaseOrderStatusId BIGINT;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_STATUS_ID @BusinessId, 'APPROVED', @PurchaseOrderStatusId OUTPUT;

        IF @PurchaseOrderStatusId IS NULL RAISERROR('No existe estado APPROVED para orden de compra.', 16, 1);

        IF NOT EXISTS (
            SELECT 1
    FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.PURCHASE_ORDER_STATUS pos ON pos.PURCHASE_ORDER_STATUS_ID = po.PURCHASE_ORDER_STATUS_ID
    WHERE po.PURCHASE_ORDER_ID = @PurchaseOrderId
        AND po.BUSINESS_ID = @BusinessId
        AND po.STATUS = '1'
        AND (UPPER(pos.CODE) = 'PENDING_APPROVAL' OR UPPER(pos.DESCRIPTION) LIKE '%PENDIENTE%')
        )
            RAISERROR('Solo se puede aprobar una orden pendiente de aprobacion.', 16, 1);

        UPDATE dbo.PURCHASE_ORDER
        SET PURCHASE_ORDER_STATUS_ID = @PurchaseOrderStatusId,
            APPROVED_BY = @ApprovedBy,
            APPROVED_AT = GETDATE(),
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE PURCHASE_ORDER_ID = @PurchaseOrderId
        AND BUSINESS_ID = @BusinessId;

        SET @COutput = 1;
        SET @SOutput = 'Orden de compra aprobada correctamente.';
    END TRY
    BEGIN CATCH
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_WS_CANCEL_PURCHASE_ORDER
    @BusinessId BIGINT,
    @PurchaseOrderId BIGINT,
    @CancelledBy BIGINT,
    @Reason VARCHAR(500) = NULL,
    @UserId BIGINT,
    @COutput INT OUTPUT,
    @SOutput VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        DECLARE @PurchaseOrderStatusId BIGINT, @DetailStatusId BIGINT;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_STATUS_ID @BusinessId, 'CANCELLED', @PurchaseOrderStatusId OUTPUT;
        EXEC dbo.SP_WS_GET_PURCHASE_ORDER_DETAIL_STATUS_ID @BusinessId, 'CANCELLED', @DetailStatusId OUTPUT;

        IF @PurchaseOrderStatusId IS NULL RAISERROR('No existe estado CANCELLED para orden de compra.', 16, 1);
        IF NOT EXISTS (SELECT 1
    FROM dbo.PURCHASE_ORDER
    WHERE PURCHASE_ORDER_ID = @PurchaseOrderId AND BUSINESS_ID = @BusinessId)
            RAISERROR('La orden de compra no existe.', 16, 1);

        IF OBJECT_ID(N'dbo.PURCHASE_RECEIPT', N'U') IS NOT NULL
        AND EXISTS (SELECT 1
        FROM dbo.PURCHASE_RECEIPT
        WHERE PURCHASE_ORDER_ID = @PurchaseOrderId AND BUSINESS_ID = @BusinessId AND STATUS = '1')
            RAISERROR('No se puede anular una orden con recepciones activas.', 16, 1);

        IF OBJECT_ID(N'dbo.SUPPLIER_INVOICE', N'U') IS NOT NULL
        AND OBJECT_ID(N'dbo.PURCHASE_ORDER_INVOICE', N'U') IS NOT NULL
        AND EXISTS (SELECT 1
        FROM dbo.PURCHASE_ORDER_INVOICE poi
        INNER JOIN dbo.SUPPLIER_INVOICE si ON si.SUPPLIER_INVOICE_ID = poi.SUPPLIER_INVOICE_ID
        WHERE poi.PURCHASE_ORDER_ID = @PurchaseOrderId
          AND poi.BUSINESS_ID = @BusinessId
          AND poi.STATUS = '1'
          AND si.IS_ACTIVE = 1)
            RAISERROR('No se puede anular una orden con factura de proveedor activa.', 16, 1);

        UPDATE dbo.PURCHASE_ORDER
        SET PURCHASE_ORDER_STATUS_ID = @PurchaseOrderStatusId,
            OBSERVATION = CASE
                WHEN NULLIF(@Reason, '') IS NULL THEN OBSERVATION
                WHEN NULLIF(OBSERVATION, '') IS NULL THEN CONCAT('Motivo anulacion: ', @Reason)
                ELSE CONCAT(OBSERVATION, CHAR(13), CHAR(10), 'Motivo anulacion: ', @Reason)
            END,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE PURCHASE_ORDER_ID = @PurchaseOrderId
        AND BUSINESS_ID = @BusinessId;

        UPDATE dbo.PURCHASE_ORDER_DETAIL
        SET DETAIL_STATUS_ID = COALESCE(@DetailStatusId, DETAIL_STATUS_ID),
            IS_ACTIVE = 0,
            UPDATE_USER = @UserId,
            UPDATE_DATE = GETDATE()
        WHERE PURCHASE_ORDER_ID = @PurchaseOrderId
        AND BUSINESS_ID = @BusinessId;

        SET @COutput = 1;
        SET @SOutput = 'Orden de compra anulada correctamente.';
    END TRY
    BEGIN CATCH
        SET @COutput = 0;
        SET @SOutput = ERROR_MESSAGE();
    END CATCH
END
GO

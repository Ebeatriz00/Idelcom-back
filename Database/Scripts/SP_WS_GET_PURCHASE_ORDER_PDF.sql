-- ============================================================
-- Stored Procedure: SP_WS_GET_PURCHASE_ORDER_PDF
-- Description: Retrieves all data needed to generate a PDF for
--              a Purchase Order (header + company + supplier
--              + signatories + detail lines).
-- Returns: 2 result sets
--   1. Header projection (1 row)
--   2. Detail lines (N rows ordered by line number)
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_WS_GET_PURCHASE_ORDER_PDF
    @BusinessId     BIGINT,
    @PurchaseOrderId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- ── Result Set 1: Header ─────────────────────────────────────────────────
    SELECT
        -- Company info
        b.COMPANY_NAME                              AS CompanyName,
        b.BUSINESS_RUC                              AS CompanyTaxId,
        b.BUSINESS_ADDRESS                          AS CompanyAddress,
        b.BUSINESS_PHONE                            AS CompanyPhone,
        b.BUSINESS_LOGO                             AS CompanyLogoPath,

        -- Purchase Order
        po.PURCHASE_ORDER_NUMBER                    AS PurchaseOrderNumber,
        po.PURCHASE_ORDER_DATE                      AS PurchaseOrderDate,
        po.EXPECTED_DELIVERY_DATE                   AS ExpectedDeliveryDate,
        pc.DESCRIPTION                              AS PaymentCondition,
        cur.DESCRIPTION                             AS CurrencyDescription,
        cur.CURRENCY_SYMBOL                         AS CurrencySymbol,
        po.EXCHANGE_RATE                            AS ExchangeRate,
        po.SUPPLIER_QUOTATION_REFERENCE_NUMBER      AS SupplierQuotationReferenceNumber,
        po.REFERENCE_NOTES                          AS [References],
        po.OBSERVATION                              AS Observation,
        po.SUBTOTAL                                 AS Subtotal,
        po.TAX_AMOUNT                               AS TaxAmount,
        po.TOTAL                                    AS Total,

        -- Supplier
        s.SUPPLIER_NAME                             AS SupplierName,
        s.DOCUMENT_NUMBER                           AS SupplierTaxId,
        s.ADDRESS                                   AS SupplierAddress,
        s.PHONE                                     AS SupplierPhone,
        s.CONTACT_NAME                              AS SupplierContact,
        s.EMAIL                                     AS SupplierEmail,

        -- Approver name
        ISNULL(
            RTRIM(wa.WORKER_NAME + ' ' + wa.WORKER_LAST_NAME),
            NULL
        )                                           AS ApproverName,

        -- Purchase manager (requested by)
        ISNULL(
            RTRIM(wr.WORKER_NAME + ' ' + wr.WORKER_LAST_NAME),
            NULL
        )                                           AS PurchaseManagerName,

        -- Bill To (same as company for now – can be overridden)
        b.COMPANY_NAME                              AS BillToName,
        b.BUSINESS_RUC                              AS BillToTaxId,
        b.BUSINESS_ADDRESS                          AS BillToAddress,

        -- Ship To (warehouse if set, else company)
        ISNULL(w.DESCRIPTION, b.COMPANY_NAME)       AS ShipToName,
        NULL                                        AS ShipToTaxId,
        CASE
            WHEN po.WAREHOUSE_ID IS NOT NULL THEN b.BUSINESS_ADDRESS
            ELSE NULL
        END                                         AS ShipToAddress

    FROM dbo.PURCHASE_ORDER po
        INNER JOIN dbo.BUSINESS b
            ON b.BUSINESS_ID = po.BUSINESS_ID
        INNER JOIN dbo.SUPPLIERS s
            ON s.SUPPLIERS_ID = po.SUPPLIERS_ID
        INNER JOIN dbo.CURRENCY cur
            ON cur.CURRENCY_ID = po.CURRENCY_ID
        LEFT  JOIN dbo.PM_CONDITION pc
            ON pc.PM_CONDITION_ID = po.PM_CONDITION_ID
        LEFT  JOIN dbo.WAREHOUSES w
            ON w.WAREHOUSES_ID = po.WAREHOUSE_ID
        LEFT  JOIN dbo.WORKERS wa
            ON wa.WORKER_ID = po.APPROVED_BY
            AND wa.BUSINESS_ID = po.BUSINESS_ID
        LEFT  JOIN dbo.WORKERS wr
            ON wr.WORKER_ID = po.REQUESTED_BY
            AND wr.BUSINESS_ID = po.BUSINESS_ID
    WHERE po.BUSINESS_ID     = @BusinessId
      AND po.PURCHASE_ORDER_ID = @PurchaseOrderId;

    -- ── Result Set 2: Detail Lines ───────────────────────────────────────────
    SELECT
        ROW_NUMBER() OVER (ORDER BY pod.PURCHASE_ORDER_DETAIL_ID) AS LineNumber,
        p.SKU                                           AS ProductCode,
        p.DESCRIPTION                                   AS ProductDescription,
        u.DESCRIPTION                                   AS UomDescription,
        pod.QUANTITY                                    AS Quantity,
        -- Unit price always shown net (pre-IGV)
        CASE
            WHEN pod.PRICE_INCLUDES_TAX = 1
                THEN ROUND(pod.UNIT_PRICE / (1.0 + pod.IGV_PERCENT / 100.0), 4)
            ELSE pod.UNIT_PRICE
        END                                             AS UnitPrice,
        -- Amount = net subtotal (pre-IGV) regardless of how price was entered
        pod.SUBTOTAL                                    AS Amount,
        pod.PRICE_INCLUDES_TAX                          AS PriceIncludesTax
    FROM dbo.PURCHASE_ORDER_DETAIL pod
        INNER JOIN dbo.PRODUCTS p
            ON p.PRODUCTS_ID = pod.PRODUCTS_ID
        LEFT  JOIN dbo.UOM u
            ON u.UOM_ID = pod.UOM_ID
    WHERE pod.BUSINESS_ID      = @BusinessId
      AND pod.PURCHASE_ORDER_ID = @PurchaseOrderId
      AND pod.IS_ACTIVE         = 1
    ORDER BY pod.PURCHASE_ORDER_DETAIL_ID;
END
GO

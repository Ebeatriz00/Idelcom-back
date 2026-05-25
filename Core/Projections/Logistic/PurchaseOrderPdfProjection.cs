namespace Core.Projections.Logistic
{
    public class PurchaseOrderPdfHeaderProjection
    {
        // Company
        public string? CompanyName { get; set; }
        public string? CompanyTaxId { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyLogoPath { get; set; }

        // Purchase Order
        public string? PurchaseOrderNumber { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? PaymentCondition { get; set; }
        public string? CurrencyDescription { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? SupplierQuotationReferenceNumber { get; set; }
        public string? References { get; set; }
        public string? Observation { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }

        // Supplier
        public string? SupplierName { get; set; }
        public string? SupplierTaxId { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhone { get; set; }
        public string? SupplierContact { get; set; }
        public string? SupplierEmail { get; set; }

        // Signatories
        public string? ApproverName { get; set; }
        public string? PurchaseManagerName { get; set; }

        // Bill To / Ship To
        public string? BillToName { get; set; }
        public string? BillToTaxId { get; set; }
        public string? BillToAddress { get; set; }
        public string? ShipToName { get; set; }
        public string? ShipToTaxId { get; set; }
        public string? ShipToAddress { get; set; }
    }

    public class PurchaseOrderPdfDetailProjection
    {
        public int LineNumber { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public string? UomDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public bool PriceIncludesTax { get; set; }
    }

    public class PurchaseOrderPdfProjection
    {
        public PurchaseOrderPdfHeaderProjection? Header { get; set; }
        public IReadOnlyList<PurchaseOrderPdfDetailProjection> Details { get; set; } = Array.Empty<PurchaseOrderPdfDetailProjection>();
    }
}

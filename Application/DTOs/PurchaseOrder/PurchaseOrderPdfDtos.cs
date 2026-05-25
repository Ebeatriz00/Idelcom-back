namespace Application.DTOs.PurchaseOrder
{
    public class CompanyInfo
    {
        public string Name { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public byte[]? LogoBytes { get; set; }
    }

    public class SupplierInfo
    {
        public string Name { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Contact { get; set; }
        public string? Email { get; set; }
    }

    public class OrderItem
    {
        public int Number { get; set; }
        public string? Code { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Um { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }

    public class BillingAddress
    {
        public string Name { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Address { get; set; }
    }

    public class PurchaseOrderPdfData
    {
        public CompanyInfo Company { get; set; } = new();
        public SupplierInfo Supplier { get; set; } = new();
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? PaymentCondition { get; set; }
        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? SupplierQuotationNumber { get; set; }
        public string? Reference { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public string? Notes { get; set; }
        public string? Approver { get; set; }
        public string? PurchaseManager { get; set; }
        public BillingAddress? BillTo { get; set; }
        public BillingAddress? ShipTo { get; set; }
    }
}

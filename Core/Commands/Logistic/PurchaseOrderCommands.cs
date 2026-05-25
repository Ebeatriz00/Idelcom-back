namespace Core.Commands.Logistic
{
    public class PurchaseOrderCommand
    {
        public long BusinessId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long SuppliersId { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public long CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public long? PmConditionId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public long? WarehouseId { get; set; }
        public string? SupplierQuotationReferenceNumber { get; set; }
        public string? References { get; set; }
        public string? Observation { get; set; }
        public long UserId { get; set; }
        public IReadOnlyList<PurchaseOrderDetailCommand> Details { get; set; } = Array.Empty<PurchaseOrderDetailCommand>();
    }

    public class PurchaseOrderDetailCommand
    {
        public long? PurchaseOrderDetailId { get; set; }
        public long ProductsId { get; set; }
        public long? UomId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public long? TaxesId { get; set; }
        public bool PriceIncludesTax { get; set; } = true;
        public string? Observation { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PurchaseOrderAttachInvoiceCommand
    {
        public long BusinessId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long SupplierInvoiceId { get; set; }
        public string? RegularizationReason { get; set; }
        public long UserId { get; set; }
    }

    public class PurchaseOrderCreateFromInvoiceCommand
    {
        public long BusinessId { get; set; }
        public long SupplierInvoiceId { get; set; }
        public long? WarehouseId { get; set; }
        public string? Observation { get; set; }
        public long UserId { get; set; }
    }

    public class SendPurchaseOrderForApprovalCommand
    {
        public long BusinessId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long UserId { get; set; }
    }
}

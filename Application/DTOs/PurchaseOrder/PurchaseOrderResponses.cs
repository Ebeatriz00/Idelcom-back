namespace Application.DTOs.PurchaseOrder
{
    public class PurchaseOrderListResponse
    {
        public long PurchaseOrderId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public long SuppliersId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDocumentNumber { get; set; }
        public long CurrencyId { get; set; }
        public string? CurrencyDescription { get; set; }
        public long PurchaseOrderStatusId { get; set; }
        public string? StatusDescription { get; set; }
        public bool IsRegularization { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
    }

    public class PurchaseOrderGetByIdResponse
    {
        public long PurchaseOrderId { get; set; }
        public long BusinessId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public long SuppliersId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDocumentNumber { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public long CurrencyId { get; set; }
        public string? CurrencyDescription { get; set; }
        public decimal? ExchangeRate { get; set; }
        public long? PmConditionId { get; set; }
        public string? PmConditionDescription { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public long? WarehouseId { get; set; }
        public string? WarehouseDescription { get; set; }
        public string? SupplierQuotationReferenceNumber { get; set; }
        public string? References { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public long PurchaseOrderStatusId { get; set; }
        public string? StatusDescription { get; set; }
        public bool IsRegularization { get; set; }
        public string? RegularizationReason { get; set; }
        public long? RegularizedBy { get; set; }
        public DateTime? RegularizationDate { get; set; }
        public string? Observation { get; set; }
        public long? RequestedBy { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public IReadOnlyList<PurchaseOrderDetailResponse> Details { get; set; } = Array.Empty<PurchaseOrderDetailResponse>();
        public IReadOnlyList<PurchaseOrderInvoiceResponse> Invoices { get; set; } = Array.Empty<PurchaseOrderInvoiceResponse>();
    }

    public class PurchaseOrderDetailResponse
    {
        public long PurchaseOrderDetailId { get; set; }
        public long ProductsId { get; set; }
        public string? ProductDescription { get; set; }
        public long? UomId { get; set; }
        public string? UomDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal PendingQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public long? TaxesId { get; set; }
        public bool PriceIncludesTax { get; set; }
        public decimal IgvPercent { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public long? DetailStatusId { get; set; }
        public string? DetailStatusDescription { get; set; }
        public string? Observation { get; set; }
        public bool IsActive { get; set; }
    }

    public class PurchaseOrderInvoiceResponse
    {
        public long PurchaseOrderInvoiceId { get; set; }
        public long BusinessId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long SupplierInvoiceId { get; set; }
        public string? Observation { get; set; }
        public string? SupplierInvoiceNumber { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }
        public decimal? SupplierInvoiceTotal { get; set; }
        public string Status { get; set; } = "1";
    }
}

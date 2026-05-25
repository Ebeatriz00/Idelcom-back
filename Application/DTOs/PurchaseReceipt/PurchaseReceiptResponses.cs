namespace Application.DTOs.PurchaseReceipt
{
    public class PurchaseReceiptResponseDto
    {
        public long PurchaseReceiptId { get; set; }
        public long BusinessId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long? RegularizedPurchaseOrderId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public long SuppliersId { get; set; }
        public string? SupplierName { get; set; }
        public long WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public long ReceiptTypeId { get; set; }
        public string? ReceiptTypeName { get; set; }
        public string? ReceiptNumber { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? SupplierGuideNumber { get; set; }
        public DateTime? SupplierGuideDate { get; set; }
        public long? WarehouseMovementId { get; set; }
        public long ReceiptStatusId { get; set; }
        public string? ReceiptStatusName { get; set; }
        public string? Observation { get; set; }
        public bool IsActive { get; set; }
        public bool IsWithoutPurchaseOrder { get; set; }
        public bool IsRegularized { get; set; }
        public bool IsServiceReceipt { get; set; }
        public bool IsWarehouseReceipt { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PurchaseReceiptDetailResponseDto
    {
        public long PurchaseReceiptDetailId { get; set; }
        public long PurchaseReceiptId { get; set; }
        public long? PurchaseOrderDetailId { get; set; }
        public long ProductsId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public long? UomId { get; set; }
        public string? UomName { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string? Observation { get; set; }
    }

    public class PurchaseReceiptFullResponseDto
    {
        public PurchaseReceiptResponseDto Header { get; set; } = new();
        public IReadOnlyList<PurchaseReceiptDetailResponseDto> Details { get; set; } = Array.Empty<PurchaseReceiptDetailResponseDto>();
    }
}

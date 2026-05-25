namespace Core.Commands.Logistic
{
    public class PurchaseReceiptCommand
    {
        public long BusinessId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long SuppliersId { get; set; }
        public long WarehouseId { get; set; }
        public long ReceiptTypeId { get; set; } = 1;
        public DateTime ReceiptDate { get; set; }
        public string? SupplierGuideNumber { get; set; }
        public DateTime? SupplierGuideDate { get; set; }
        public string? Observation { get; set; }
        public long UserId { get; set; }
        public IReadOnlyList<PurchaseReceiptDetailCommand> Details { get; set; } = Array.Empty<PurchaseReceiptDetailCommand>();
    }

    public class PurchaseReceiptDetailCommand
    {
        public long? PurchaseOrderDetailId { get; set; }
        public long ProductsId { get; set; }
        public long? UomId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Observation { get; set; }
    }

    public class PurchaseReceiptRegularizeCommand
    {
        public long BusinessId { get; set; }
        public long PurchaseReceiptId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long UserId { get; set; }
    }
}

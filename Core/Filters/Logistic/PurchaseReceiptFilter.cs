namespace Core.Filters.Logistic
{
    public class PurchaseReceiptFilter
    {
        public long BusinessId { get; set; }
        public long? WarehouseId { get; set; }
        public long? SuppliersId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long? ReceiptStatusId { get; set; }
        public long? ReceiptTypeId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

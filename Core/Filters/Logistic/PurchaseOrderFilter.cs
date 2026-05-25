namespace Core.Filters.Logistic
{
    public class PurchaseOrderFilter
    {
        public long BusinessId { get; set; }
        public long? SuppliersId { get; set; }
        public long? PurchaseOrderStatusId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

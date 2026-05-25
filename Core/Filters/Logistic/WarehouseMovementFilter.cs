namespace Core.Filters.Logistic
{
    public class WarehouseMovementFilter
    {
        public long BusinessId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public long? MovementTypeId { get; set; }
        public long? MovOperId { get; set; }
        public long? WarehouseId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}

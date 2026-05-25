namespace Application.DTOs.WarehousesMovement
{
    public class WarehouseMovementFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public long? MovementTypeId { get; set; }
        public long? MovOperId { get; set; }
        public long? WarehouseId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}

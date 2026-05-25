namespace Application.DTOs.InventoryCount
{
    public class InventoryCountListResponseDto
    {
        public long InventoryCountId { get; set; }
        public long BusinessId { get; set; }
        public long WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string? CountNumber { get; set; }
        public DateTime CountDate { get; set; }
        public long InventoryCountStatusId { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusDescription { get; set; }
        public string? Observation { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class InventoryCountDetailResponseDto
    {
        public long InventoryCountDetailId { get; set; }
        public long InventoryCountId { get; set; }
        public long ProductsId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal CountedQuantity { get; set; }
        public decimal DifferenceQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalDifferenceCost { get; set; }
        public string? LotNumber { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public long? AdjustmentMovementId { get; set; }
        public string? Observation { get; set; }
    }

    public class InventoryCountFullResponseDto
    {
        public InventoryCountListResponseDto Header { get; set; } = new();
        public IReadOnlyList<InventoryCountDetailResponseDto> Details { get; set; } = Array.Empty<InventoryCountDetailResponseDto>();
    }
}

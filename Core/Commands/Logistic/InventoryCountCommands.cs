namespace Core.Commands.Logistic
{
    public class InventoryCountCreateCommand
    {
        public long BusinessId { get; set; }
        public long WarehouseId { get; set; }
        public DateTime CountDate { get; set; }
        public string? Observation { get; set; }
        public long UserId { get; set; }
    }

    public class InventoryCountUpdateDetailsCommand
    {
        public long BusinessId { get; set; }
        public long InventoryCountId { get; set; }
        public long UserId { get; set; }
        public IReadOnlyList<InventoryCountDetailUpdateCommand> Details { get; set; } = Array.Empty<InventoryCountDetailUpdateCommand>();
    }

    public class InventoryCountDetailUpdateCommand
    {
        public long InventoryCountDetailId { get; set; }
        public decimal CountedQuantity { get; set; }
        public string? LotNumber { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Observation { get; set; }
    }

    public class InventoryCountMarkAdjustedCommand
    {
        public long BusinessId { get; set; }
        public long InventoryCountId { get; set; }
        public long UserId { get; set; }
        public IReadOnlyList<InventoryCountDetailAdjustmentCommand> Adjustments { get; set; } = Array.Empty<InventoryCountDetailAdjustmentCommand>();
    }

    public class InventoryCountDetailAdjustmentCommand
    {
        public long InventoryCountDetailId { get; set; }
        public long AdjustmentMovementId { get; set; }
    }
}

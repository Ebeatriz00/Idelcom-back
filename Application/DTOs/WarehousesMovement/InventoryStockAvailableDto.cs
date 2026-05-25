namespace Application.DTOs.WarehousesMovement
{
    public class InventoryStockAvailableDto
    {
        public long ProductsId { get; set; }
        public string? ProductDescription { get; set; }
        public long WarehouseId { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal LastCost { get; set; }
    }
}

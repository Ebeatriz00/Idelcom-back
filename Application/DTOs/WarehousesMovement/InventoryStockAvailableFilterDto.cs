namespace Application.DTOs.WarehousesMovement
{
    public class InventoryStockAvailableFilterDto
    {
        public long WarehouseId { get; set; }
        public long? ProductsId { get; set; }
        public string? Search { get; set; }
    }
}

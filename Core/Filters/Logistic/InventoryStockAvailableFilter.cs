namespace Core.Filters.Logistic
{
    public class InventoryStockAvailableFilter
    {
        public long BusinessId { get; set; }
        public long WarehouseId { get; set; }
        public long? ProductsId { get; set; }
        public string? Search { get; set; }
    }
}

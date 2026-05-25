using System;

namespace Application.DTOs.InventoryStock
{
    public class InventoryStockCreateDto
    {
        public long ProductsId { get; set; }
        public long WarehouseId { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal LastCost { get; set; }
        public DateTime? LastEntryDate { get; set; }
        public DateTime? LastOutputDate { get; set; }
    }
}

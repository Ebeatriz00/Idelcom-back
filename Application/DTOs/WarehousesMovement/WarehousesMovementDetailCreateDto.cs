using System;

namespace Application.DTOs.WarehousesMovement
{
    public class WarehousesMovementDetailCreateDto
    {
        public long ProductsId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? LotNumber { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Observation { get; set; }
    }
}

namespace Core.Projections.Logistic
{
    public class WarehouseMovementDetailProjection
    {
        public long WarehouseMovementDetailId { get; set; }
        public long ProductsId { get; set; }
        public string? ProductDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string? LotNumber { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Observation { get; set; }
    }
}

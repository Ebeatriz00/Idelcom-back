namespace Core.Projections.Logistic
{
    public class WarehouseMovementListItem
    {
        public long WarehouseMovementId { get; set; }
        public long MovementTypeId { get; set; }
        public string? MovementTypeDescription { get; set; }
        public long MovOperId { get; set; }
        public string? MovOperDescription { get; set; }
        public long WarehouseId { get; set; }
        public string? WarehouseDescription { get; set; }
        public string? SupplierName { get; set; }
        public string? ClientName { get; set; }
        public DateTime? MovementDate { get; set; }
        public decimal SubTotal { get; set; }
        public long? TaxesId { get; set; }
        public decimal IgvPercent { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
    }
}

namespace Core.Projections.Logistic
{
    public class WarehouseMovementHeaderProjection
    {
        public long WarehouseMovementId { get; set; }
        public long BusinessId { get; set; }
        public long MovementTypeId { get; set; }
        public string? MovementTypeCode { get; set; }
        public string? MovementTypeDescription { get; set; }
        public long MovOperId { get; set; }
        public string? MovOperDescription { get; set; }
        public long WarehouseId { get; set; }
        public string? WarehouseDescription { get; set; }
        public long WarehouseDestinationId { get; set; }
        public string? WarehouseDestinationDescription { get; set; }
        public long SuppliersId { get; set; }
        public string? SupplierName { get; set; }
        public long ClientsId { get; set; }
        public string? ClientName { get; set; }
        public string? Series { get; set; }
        public string? NumberDocument { get; set; }
        public string? ReferenceDocument { get; set; }
        public DateTime? MovementDate { get; set; }
        public string? Observation { get; set; }
        public long? TaxesId { get; set; }
        public decimal IgvPercent { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUser { get; set; }
    }
}

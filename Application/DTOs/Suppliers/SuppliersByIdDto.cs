namespace Application.DTOs.Suppliers
{
    public class SuppliersByIdDto
    {
        public long SuppliersId { get; set; }
        public long BusinessId { get; set; }
        public long SupplierTypeId { get; set; }
        public long SupplierGroupsId { get; set; }
        public long PaymentConditionId { get; set; }
        public long PaymentMethodId { get; set; }
        public long DocumentTypeId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? SupplierName { get; set; }
        public string? TradeName { get; set; }
        public string? ContactName { get; set; }
        public string? Address { get; set; }
        public long? DepartmentId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public bool RetainerAgent { get; set; }
        public bool PerceptionAgent { get; set; }
        public bool DetractionAgent { get; set; }
        public bool ForeignAgent { get; set; }
        public string? Observation { get; set; }
    }
}

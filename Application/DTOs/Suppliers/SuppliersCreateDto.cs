using SharedKernel.Helpers;
using System.Text.Json.Serialization;

namespace Application.DTOs.Suppliers
{
    public class SuppliersCreateDto
    {
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
        [JsonConverter(typeof(FlexibleBooleanConverter))]
        public bool RetainerAgent { get; set; }
        [JsonConverter(typeof(FlexibleBooleanConverter))]
        public bool PerceptionAgent { get; set; }
        [JsonConverter(typeof(FlexibleBooleanConverter))]
        public bool DetractionAgent { get; set; }
        [JsonConverter(typeof(FlexibleBooleanConverter))]
        public bool ForeignAgent { get; set; }
        public string? Observation { get; set; }

        [JsonPropertyName("typeSuppliersId")]
        public long TypeSuppliersId
        {
            get => SupplierTypeId;
            set => SupplierTypeId = value;
        }

        [JsonPropertyName("suppliersGroupsId")]
        public long SuppliersGroupsId
        {
            get => SupplierGroupsId;
            set => SupplierGroupsId = value;
        }

        [JsonPropertyName("paymentTypeId")]
        public long PaymentTypeId
        {
            get => PaymentConditionId;
            set => PaymentConditionId = value;
        }

        [JsonPropertyName("departamentId")]
        public long? DepartamentId
        {
            get => DepartmentId;
            set => DepartmentId = value;
        }

        [JsonPropertyName("movil")]
        public string? Movil
        {
            get => Mobile;
            set => Mobile = value;
        }
    }
}

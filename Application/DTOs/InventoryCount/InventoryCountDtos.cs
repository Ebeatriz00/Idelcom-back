using System.Text.Json.Serialization;
using SharedKernel.Helpers;

namespace Application.DTOs.InventoryCount
{
    public class InventoryCountCreateDto
    {
        public long WarehouseId { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? CountDate { get; set; }
        public string? Observation { get; set; }
    }

    public class InventoryCountUpdateDetailsDto
    {
        public long InventoryCountId { get; set; }
        public List<InventoryCountDetailUpdateDto> Details { get; set; } = new();
    }

    public class InventoryCountDetailUpdateDto
    {
        public long InventoryCountDetailId { get; set; }
        public decimal CountedQuantity { get; set; }
        public string? LotNumber { get; set; }
        public string? SerialNumber { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? ExpirationDate { get; set; }
        public string? Observation { get; set; }
    }

    public class InventoryCountListFilterDto
    {
        public long? WarehouseId { get; set; }
        public long? InventoryCountStatusId { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DateFrom { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

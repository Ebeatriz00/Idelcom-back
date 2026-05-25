using System.Text.Json.Serialization;
using SharedKernel.Helpers;

namespace Application.DTOs.PurchaseReceipt
{
    public class PurchaseReceiptCreateDto
    {
        public long BusinessId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long SuppliersId { get; set; }
        public long WarehouseId { get; set; }
        public long ReceiptTypeId { get; set; } = 1;
        public DateTime ReceiptDate { get; set; }
        public string? SupplierGuideNumber { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? SupplierGuideDate { get; set; }
        public string? Observation { get; set; }
        public List<PurchaseReceiptDetailCreateDto> Details { get; set; } = new();
    }

    public class PurchaseReceiptDetailCreateDto
    {
        public long? PurchaseOrderDetailId { get; set; }
        public long ProductsId { get; set; }
        public long? UomId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Observation { get; set; }
    }

    public class PurchaseReceiptRegularizeDto
    {
        public long PurchaseReceiptId { get; set; }
        public long PurchaseOrderId { get; set; }
    }

    public class PurchaseReceiptListFilterDto
    {
        public long BusinessId { get; set; }
        public long? WarehouseId { get; set; }
        public long? SuppliersId { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long? ReceiptStatusId { get; set; }
        public long? ReceiptTypeId { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DateFrom { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

using System.Text.Json.Serialization;
using SharedKernel.Helpers;

namespace Application.DTOs.PurchaseOrder
{
    public class PurchaseOrderCreateDto
    {
        public long SuppliersId { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime PurchaseOrderDate { get; set; }
        public long CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public long? PmConditionId { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? ExpectedDeliveryDate { get; set; }
        public long? WarehouseId { get; set; }
        public string? SupplierQuotationReferenceNumber { get; set; }
        public string? References { get; set; }
        public string? Observation { get; set; }
        public List<PurchaseOrderDetailCreateDto> Details { get; set; } = new();
    }

    public class PurchaseOrderDetailCreateDto
    {
        public long ProductsId { get; set; }
        public long? UomId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public long? TaxesId { get; set; }
        public bool PriceIncludesTax { get; set; } = true;
        public string? Observation { get; set; }
    }

    public class PurchaseOrderUpdateDto
    {
        public long PurchaseOrderId { get; set; }
        public long SuppliersId { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime PurchaseOrderDate { get; set; }
        public long CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public long? PmConditionId { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? ExpectedDeliveryDate { get; set; }
        public long? WarehouseId { get; set; }
        public string? SupplierQuotationReferenceNumber { get; set; }
        public string? References { get; set; }
        public string? Observation { get; set; }
        public List<PurchaseOrderDetailUpdateDto> Details { get; set; } = new();
    }

    public class PurchaseOrderDetailUpdateDto
    {
        public long? PurchaseOrderDetailId { get; set; }
        public long ProductsId { get; set; }
        public long? UomId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public long? TaxesId { get; set; }
        public bool PriceIncludesTax { get; set; } = true;
        public string? Observation { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PurchaseOrderApproveDto
    {
        public long PurchaseOrderId { get; set; }
        public long ApprovedBy { get; set; }
    }

    public class PurchaseOrderCancelDto
    {
        public long PurchaseOrderId { get; set; }
        public long CancelledBy { get; set; }
        public string? Reason { get; set; }
    }

    public class PurchaseOrderAttachInvoiceDto
    {
        public long PurchaseOrderId { get; set; }
        public long SupplierInvoiceId { get; set; }
        public string? RegularizationReason { get; set; }
    }

    public class PurchaseOrderCreateFromInvoiceDto
    {
        public long SupplierInvoiceId { get; set; }
        public long? WarehouseId { get; set; }
        public string? Observation { get; set; }
    }

    public class PurchaseOrderSendForApprovalDto
    {
        public long PurchaseOrderId { get; set; }
    }

    public class PurchaseOrderListFilterDto
    {
        public long? SuppliersId { get; set; }
        public long? PurchaseOrderStatusId { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DateFrom { get; set; }
        [JsonConverter(typeof(DateTimeNullableConverter))]
        public DateTime? DateTo { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

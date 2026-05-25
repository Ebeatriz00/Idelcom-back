namespace Core.Filters.Logistic
{
    public class QuotationLogisticsSuggestionFilter
    {
        public long BusinessId { get; set; }
        public long QuotationId { get; set; }
        public long? QuotationVerId { get; set; }
        public long? ResourceTypeId { get; set; }
        public long? WorkOrderId { get; set; }
        public bool? OnlySelected { get; set; }
        public string? Search { get; set; }
    }
}

namespace Core.Commands.Logistic
{
    public class GenerateQuotationLogisticsSuggestionCommand
    {
        public long BusinessId { get; set; }
        public long QuotationId { get; set; }
        public long? QuotationVerId { get; set; }
        public long UserId { get; set; }
    }

    public class UpdateQuotationLogisticsSuggestionCommand
    {
        public long SuggestionId { get; set; }
        public long BusinessId { get; set; }
        public bool IsSelected { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string? OfficeObservation { get; set; }
        public long? WorkOrderId { get; set; }
        public long UserId { get; set; }
    }

    public class AddManualQuotationLogisticsSuggestionCommand
    {
        public long BusinessId { get; set; }
        public long QuotationId { get; set; }
        public long QuotationVerId { get; set; }
        public long? WorkOrderId { get; set; }
        public long LogisticsResourceTypeId { get; set; }
        public long? ProductsId { get; set; }
        public string? Description { get; set; }
        public decimal SuggestedQuantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string? OfficeObservation { get; set; }
        public long UserId { get; set; }
    }

    public class AssignWorkOrderQuotationLogisticsSuggestionCommand
    {
        public long BusinessId { get; set; }
        public long SuggestionId { get; set; }
        public long? WorkOrderId { get; set; }
        public long UserId { get; set; }
    }

    public class CreateLogisticsRequestFromSelectedSuggestionsCommand
    {
        public long BusinessId { get; set; }
        public long? QuotationId { get; set; }
        public long? QuotationVerId { get; set; }
        public long? WorkOrderId { get; set; }
        public string? SuggestionIdsCsv { get; set; }
        public string? Observation { get; set; }
        public string? OfficeObservation { get; set; }
        public long UserId { get; set; }
    }
}

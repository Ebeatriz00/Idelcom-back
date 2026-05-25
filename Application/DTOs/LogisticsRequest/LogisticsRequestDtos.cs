namespace Application.DTOs.LogisticsRequest
{
    public class CreateLogisticsRequestFromSelectedSuggestionsDto
    {
        public long? QuotationId { get; set; }
        public long? QuotationVerId { get; set; }
        public long? WorkOrderId { get; set; }
        public IReadOnlyList<long>? SuggestionIds { get; set; }
        public string? Observation { get; set; }
        public string? OfficeObservation { get; set; }
    }

    public class CreateLogisticsRequestFromSelectedSuggestionsResponseDto
    {
        public long LogisticsRequestId { get; set; }
        public int DetailCount { get; set; }
        public string? Message { get; set; }
    }
}

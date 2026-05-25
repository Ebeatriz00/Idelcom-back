namespace Core.Projections.Logistic
{
    public class QuotationLogisticsSuggestionGenerateResult
    {
        public long QuotationId { get; set; }
        public long QuotationVerId { get; set; }
        public int CreatedCount { get; set; }
        public int ExistingCount { get; set; }
        public int FullyRequestedCount { get; set; }
        public int PendingTotalCount { get; set; }
        public int TotalActiveCount { get; set; }
        public string? Message { get; set; }
    }

    public class QuotationLogisticsSuggestionItem
    {
        public long QuotationLogisticsSuggestionId { get; set; }
        public long BusinessId { get; set; }
        public long QuotationId { get; set; }
        public long QuotationVerId { get; set; }
        public long? QuotationVerLinId { get; set; }
        public long? WorkOrderId { get; set; }
        public string? WorkOrderCode { get; set; }
        public string? WorkOrderName { get; set; }
        public string? LineDescription { get; set; }
        public decimal? LineQty { get; set; }
        public long? LogisticsSuggestionRuleId { get; set; }
        public long LogisticsResourceTypeId { get; set; }
        public string? ResourceTypeCode { get; set; }
        public string? ResourceTypeDescription { get; set; }
        public long? ProductsId { get; set; }
        public string ProductStatus { get; set; } = string.Empty;
        public string StockStatus { get; set; } = string.Empty;
        public string SuggestedAction { get; set; } = string.Empty;
        public decimal StockQuantity { get; set; }
        public decimal AvailableStockQuantity { get; set; }
        public decimal MissingQuantity { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal SuggestedQuantity { get; set; }
        public decimal? SuggestedQuantityBase { get; set; }
        public decimal AlreadyRequestedQuantity { get; set; }
        public decimal PendingToRequestQuantity { get; set; }
        public decimal ExcessRequestedQuantity { get; set; }
        public bool IsFullyRequested { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public bool IsSelected { get; set; }
        public bool IsManual { get; set; }
        public bool IsDuplicated { get; set; }
        public string? SuggestionReason { get; set; }
        public string? OfficeObservation { get; set; }
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public bool Status { get; set; }
    }

    public class CreateLogisticsRequestFromSelectedSuggestionsResult
    {
        public long LogisticsRequestId { get; set; }
        public int DetailCount { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }
    }
}

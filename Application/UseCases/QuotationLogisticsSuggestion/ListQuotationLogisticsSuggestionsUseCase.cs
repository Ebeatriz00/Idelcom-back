using Application.DTOs.QuotationLogisticsSuggestion;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using FluentValidation;

namespace Application.UseCases.QuotationLogisticsSuggestion
{
    public class ListQuotationLogisticsSuggestionsUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        IValidator<ListQuotationLogisticsSuggestionFilterDto> validator)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly IValidator<ListQuotationLogisticsSuggestionFilterDto> _validator = validator;

        public async Task<IReadOnlyList<QuotationLogisticsSuggestionResponseDto>> ExecuteAsync(ListQuotationLogisticsSuggestionFilterDto filter, long businessId)
        {
            await QuotationLogisticsSuggestionValidation.ValidateAsync(_validator, filter);

            var items = await _repository.ListAsync(new QuotationLogisticsSuggestionFilter
            {
                BusinessId = businessId,
                QuotationId = filter.QuotationId,
                QuotationVerId = NormalizeId(filter.QuotationVerId),
                ResourceTypeId = NormalizeId(filter.ResourceTypeId),
                WorkOrderId = NormalizeId(filter.WorkOrderId),
                OnlySelected = filter.OnlySelected,
                Search = filter.Search
            });

            return items.Select(x => new QuotationLogisticsSuggestionResponseDto
            {
                QuotationLogisticsSuggestionId = x.QuotationLogisticsSuggestionId,
                BusinessId = x.BusinessId,
                QuotationId = x.QuotationId,
                QuotationVerId = x.QuotationVerId,
                QuotationVerLinId = x.QuotationVerLinId,
                WorkOrderId = x.WorkOrderId,
                WorkOrderCode = x.WorkOrderCode,
                WorkOrderName = x.WorkOrderName,
                LineDescription = x.LineDescription,
                LineQty = x.LineQty,
                LogisticsSuggestionRuleId = x.LogisticsSuggestionRuleId,
                LogisticsResourceTypeId = x.LogisticsResourceTypeId,
                ResourceTypeCode = x.ResourceTypeCode,
                ResourceTypeDescription = x.ResourceTypeDescription,
                ProductsId = x.ProductsId,
                ProductStatus = x.ProductStatus,
                StockStatus = x.StockStatus,
                SuggestedAction = x.SuggestedAction,
                StockQuantity = x.StockQuantity,
                AvailableStockQuantity = x.AvailableStockQuantity,
                MissingQuantity = x.MissingQuantity,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Description = x.Description,
                SuggestedQuantity = x.SuggestedQuantity,
                SuggestedQuantityBase = x.SuggestedQuantityBase,
                AlreadyRequestedQuantity = x.AlreadyRequestedQuantity,
                PendingToRequestQuantity = x.PendingToRequestQuantity,
                ExcessRequestedQuantity = x.ExcessRequestedQuantity,
                IsFullyRequested = x.IsFullyRequested,
                ApprovedQuantity = x.ApprovedQuantity,
                IsSelected = x.IsSelected,
                IsManual = x.IsManual,
                IsDuplicated = x.IsDuplicated,
                SuggestionReason = x.SuggestionReason,
                OfficeObservation = x.OfficeObservation,
                ReviewedBy = x.ReviewedBy,
                ReviewedDate = x.ReviewedDate,
                Status = x.Status
            }).ToList();
        }

        private static long? NormalizeId(long? value) => value.HasValue && value.Value > 0 ? value.Value : null;
    }
}

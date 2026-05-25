using Core.Commands.Logistic;
using Core.Filters.Logistic;
using Core.Projections.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IQuotationLogisticsSuggestionRepository
    {
        Task<QuotationLogisticsSuggestionGenerateResult> GenerateAsync(GenerateQuotationLogisticsSuggestionCommand command);
        Task<IReadOnlyList<QuotationLogisticsSuggestionItem>> ListAsync(QuotationLogisticsSuggestionFilter filter);
        Task<QuotationLogisticsSuggestionItem?> GetByIdAsync(long businessId, long suggestionId, IDbTransaction? transaction = null);
        Task<BaseResponse> UpdateAsync(UpdateQuotationLogisticsSuggestionCommand command, IDbTransaction? transaction = null);
        Task<BaseResponseId> AddManualAsync(AddManualQuotationLogisticsSuggestionCommand command, IDbTransaction? transaction = null);
        Task<BaseResponse> AssignWorkOrderAsync(AssignWorkOrderQuotationLogisticsSuggestionCommand command, IDbTransaction? transaction = null);
        Task<BaseResponse> DisableAsync(long businessId, long suggestionId, long userId, IDbTransaction? transaction = null);
        Task<CreateLogisticsRequestFromSelectedSuggestionsResult> CreateLogisticsRequestFromSelectedSuggestionsAsync(CreateLogisticsRequestFromSelectedSuggestionsCommand command, IDbTransaction? transaction = null);
    }
}

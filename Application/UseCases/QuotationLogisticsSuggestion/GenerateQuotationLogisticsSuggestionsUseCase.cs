using Application.DTOs.QuotationLogisticsSuggestion;
using Core.Commands.Logistic;
using Core.Interfaces.Logistic;
using FluentValidation;

namespace Application.UseCases.QuotationLogisticsSuggestion
{
    public class GenerateQuotationLogisticsSuggestionsUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        IValidator<GenerateQuotationLogisticsSuggestionDto> validator)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly IValidator<GenerateQuotationLogisticsSuggestionDto> _validator = validator;

        public async Task<QuotationLogisticsSuggestionGenerateResponseDto> ExecuteAsync(GenerateQuotationLogisticsSuggestionDto dto, long businessId, long userId)
        {
            await QuotationLogisticsSuggestionValidation.ValidateAsync(_validator, dto);

            var result = await _repository.GenerateAsync(new GenerateQuotationLogisticsSuggestionCommand
            {
                BusinessId = businessId,
                QuotationId = dto.QuotationId,
                QuotationVerId = NormalizeId(dto.QuotationVerId),
                UserId = userId
            });

            return new QuotationLogisticsSuggestionGenerateResponseDto
            {
                QuotationId = result.QuotationId,
                QuotationVerId = result.QuotationVerId,
                CreatedCount = result.CreatedCount,
                ExistingCount = result.ExistingCount,
                FullyRequestedCount = result.FullyRequestedCount,
                PendingTotalCount = result.PendingTotalCount,
                TotalActiveCount = result.TotalActiveCount,
                Message = result.Message
            };
        }

        private static long? NormalizeId(long? value) => value.HasValue && value.Value > 0 ? value.Value : null;
    }
}

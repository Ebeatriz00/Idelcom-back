using Application.DTOs.LogisticsRequest;
using Application.UseCases.QuotationLogisticsSuggestion;
using Core.Commands.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel.Constants;

namespace Application.UseCases.LogisticsRequest
{
    public class CreateLogisticsRequestFromSelectedSuggestionsUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        IValidator<CreateLogisticsRequestFromSelectedSuggestionsDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly IValidator<CreateLogisticsRequestFromSelectedSuggestionsDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<CreateLogisticsRequestFromSelectedSuggestionsResponseDto> ExecuteAsync(
            CreateLogisticsRequestFromSelectedSuggestionsDto dto,
            long businessId,
            long userId)
        {
            await QuotationLogisticsSuggestionValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await _repository.CreateLogisticsRequestFromSelectedSuggestionsAsync(new CreateLogisticsRequestFromSelectedSuggestionsCommand
                {
                    BusinessId = businessId,
                    QuotationId = NormalizeId(dto.QuotationId),
                    QuotationVerId = NormalizeId(dto.QuotationVerId),
                    WorkOrderId = NormalizeId(dto.WorkOrderId),
                    SuggestionIdsCsv = BuildIdsCsv(dto.SuggestionIds),
                    Observation = dto.Observation,
                    OfficeObservation = dto.OfficeObservation,
                    UserId = userId
                }, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.LogisticsRequest,
                    result.LogisticsRequestId,
                    userId);

                await _auditService.RegisterCreateAsync(result, auditLog, transaction);

                transaction.Commit();

                return new CreateLogisticsRequestFromSelectedSuggestionsResponseDto
                {
                    LogisticsRequestId = result.LogisticsRequestId,
                    DetailCount = result.DetailCount,
                    Message = result.Message
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static long? NormalizeId(long? value) => value.HasValue && value.Value > 0 ? value.Value : null;

        private static string? BuildIdsCsv(IReadOnlyList<long>? ids)
        {
            var normalized = ids?
                .Where(id => id > 0)
                .Distinct()
                .ToArray();

            return normalized is { Length: > 0 } ? string.Join(",", normalized) : null;
        }
    }
}

using Application.Exceptions;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.QuotationLogisticsSuggestion
{
    public class DisableQuotationLogisticsSuggestionUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponse> ExecuteAsync(long businessId, long suggestionId, long userId)
        {
            if (businessId <= 0)
                throw new ValidationException(new List<GlobalErrorDetail> { new("VALIDATION_NEGATIVE", "La empresa es obligatoria.") });

            if (suggestionId <= 0)
                throw new ValidationException(new List<GlobalErrorDetail> { new("VALIDATION_NEGATIVE", "La sugerencia es obligatoria.") });

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, suggestionId, transaction)
                    ?? throw new NotFoundException("Sugerencia logistica", suggestionId);

                var result = await _repository.DisableAsync(businessId, suggestionId, userId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.QuotationLogisticsSuggestion,
                    suggestionId,
                    userId);

                await _auditService.RegisterDeleteAsync(before, auditLog, transaction);

                transaction.Commit();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

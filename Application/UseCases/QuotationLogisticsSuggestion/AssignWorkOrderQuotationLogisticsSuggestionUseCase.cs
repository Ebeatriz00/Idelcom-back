using Application.DTOs.QuotationLogisticsSuggestion;
using Application.Exceptions;
using Core.Commands.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.QuotationLogisticsSuggestion
{
    public class AssignWorkOrderQuotationLogisticsSuggestionUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        IValidator<AssignWorkOrderQuotationLogisticsSuggestionDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly IValidator<AssignWorkOrderQuotationLogisticsSuggestionDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponse> ExecuteAsync(AssignWorkOrderQuotationLogisticsSuggestionDto dto, long businessId, long userId)
        {
            await QuotationLogisticsSuggestionValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, dto.SuggestionId, transaction)
                    ?? throw new NotFoundException("Sugerencia logistica", dto.SuggestionId);

                var result = await _repository.AssignWorkOrderAsync(new AssignWorkOrderQuotationLogisticsSuggestionCommand
                {
                    BusinessId = businessId,
                    SuggestionId = dto.SuggestionId,
                    WorkOrderId = NormalizeId(dto.WorkOrderId),
                    UserId = userId
                }, transaction);

                var after = await _repository.GetByIdAsync(businessId, dto.SuggestionId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.QuotationLogisticsSuggestion,
                    dto.SuggestionId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static long? NormalizeId(long? value) => value.HasValue && value.Value > 0 ? value.Value : null;
    }
}

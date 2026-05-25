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
    public class UpdateQuotationLogisticsSuggestionUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        IValidator<UpdateQuotationLogisticsSuggestionDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly IValidator<UpdateQuotationLogisticsSuggestionDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponse> ExecuteAsync(UpdateQuotationLogisticsSuggestionDto dto, long businessId, long userId)
        {
            await QuotationLogisticsSuggestionValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, dto.QuotationLogisticsSuggestionId, transaction)
                    ?? throw new NotFoundException("Sugerencia logistica", dto.QuotationLogisticsSuggestionId);

                var result = await _repository.UpdateAsync(new UpdateQuotationLogisticsSuggestionCommand
                {
                    SuggestionId = dto.QuotationLogisticsSuggestionId,
                    BusinessId = businessId,
                    IsSelected = dto.IsSelected,
                    ApprovedQuantity = dto.ApprovedQuantity,
                    OfficeObservation = dto.OfficeObservation,
                    WorkOrderId = NormalizeId(dto.WorkOrderId),
                    UserId = userId
                }, transaction);

                var after = await _repository.GetByIdAsync(businessId, dto.QuotationLogisticsSuggestionId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.QuotationLogisticsSuggestion,
                    dto.QuotationLogisticsSuggestionId,
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

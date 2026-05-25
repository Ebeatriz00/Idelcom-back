using Application.DTOs.QuotationLogisticsSuggestion;
using Core.Commands.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.QuotationLogisticsSuggestion
{
    public class AddManualQuotationLogisticsSuggestionUseCase(
        IQuotationLogisticsSuggestionRepository repository,
        IValidator<AddManualQuotationLogisticsSuggestionDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory)
    {
        private readonly IQuotationLogisticsSuggestionRepository _repository = repository;
        private readonly IValidator<AddManualQuotationLogisticsSuggestionDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;

        public async Task<BaseResponseId> ExecuteAsync(AddManualQuotationLogisticsSuggestionDto dto, long businessId, long userId)
        {
            await QuotationLogisticsSuggestionValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var created = await _repository.AddManualAsync(new AddManualQuotationLogisticsSuggestionCommand
                {
                    BusinessId = businessId,
                    QuotationId = dto.QuotationId,
                    QuotationVerId = dto.QuotationVerId,
                    WorkOrderId = NormalizeId(dto.WorkOrderId),
                    LogisticsResourceTypeId = dto.LogisticsResourceTypeId,
                    ProductsId = NormalizeId(dto.ProductsId),
                    Description = dto.Description,
                    SuggestedQuantity = dto.SuggestedQuantity,
                    ApprovedQuantity = dto.ApprovedQuantity,
                    OfficeObservation = dto.OfficeObservation,
                    UserId = userId
                }, transaction);

                var entity = await _repository.GetByIdAsync(businessId, created.Id!.Value, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.QuotationLogisticsSuggestion,
                    created.Id!.Value,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                transaction.Commit();
                return created;
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

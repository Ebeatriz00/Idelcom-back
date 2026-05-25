using Application.Exceptions;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.Operations.OperationsWorkOrder
{
    public class DeleteOperationsWorkOrder(
        IOperationsWorkOrderRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        ISqlConnectionFactory sqlConnectionFactory
        )
    {
        private readonly IOperationsWorkOrderRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long workOrderId, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(workOrderId, businessId);

                if (before == null)
                    throw new NotFoundException("Operations Work Order", workOrderId);

                var deleted = await _repository.DeleteAsync(workOrderId, businessId, userId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrder,
                    workOrderId,
                    userId);

                await _auditService.RegisterDeleteAsync(before, auditLog, transaction);

                await transaction.CommitAsync();
                return deleted;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

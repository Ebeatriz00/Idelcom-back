using Application.Exceptions;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.Operations.OperationsWorkOrderResponsible
{
    public class DeleteOperationsWorkOrderResponsible(
        IOperationsWorkOrderResponsibleRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly IOperationsWorkOrderResponsibleRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(
            long workOrderResponsibleId,
            long businessId,
            long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(workOrderResponsibleId, businessId);

                if (before == null)
                    throw new NotFoundException("Operations Work Order Responsible", workOrderResponsibleId);

                var deleted = await _repository.DeleteAsync(workOrderResponsibleId, businessId, userId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrderResponsible,
                    workOrderResponsibleId,
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

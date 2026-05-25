using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.Operations.Operations
{
    public class DeleteOperations(
        IOperationsRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        ISqlConnectionFactory sqlConnectionFactory
        )
    {
        private readonly IOperationsRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long operationId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(operationId);

                if (before == null)
                    throw new Exception("No se encontró la operación.");

                var updated = await _repository.DeleteAsync(operationId, userId, transaction);

                var auditLog = _auditLogFactory.Create(
                    before.BusinessId,
                    TableNames.Operations,
                    before.OperationsId,
                    userId);

                await _auditService.RegisterDeleteAsync(before, auditLog, trans: transaction);

                transaction.Commit();
                return updated;
            }
            catch (InvalidCastException ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }

    }
}

using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class DeleteOperationsWorkOrderActivity(
        IOperationsWorkOrderActivityRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(long activityId, long businessId, long userId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var before = await _repository.GetByIdAsync(activityId, businessId, transaction);
                if (before == null)
                    return new BaseResponse { Status = 0, Message = "No se encontró la actividad para eliminar." };

                var result = await _repository.DeleteAsync(activityId, businessId, userId, transaction);

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrderActivity,
                    activityId,
                    userId);

                await _auditService.RegisterDeleteAsync(before, auditLog, transaction);

                await transaction.CommitAsync();
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
    
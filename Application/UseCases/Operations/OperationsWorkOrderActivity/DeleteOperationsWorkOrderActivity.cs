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

        public async Task<BaseResponse> ExecuteAsync(List<long> activityIds, long businessId, long userId)
        {
            if (activityIds == null || activityIds.Count == 0)
                return new BaseResponse { Status = 0, Message = "No se proporcionaron actividades para eliminar." };

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                int successCount = 0;
                BaseResponse lastResult = new BaseResponse { Status = 1 };

                foreach (var activityId in activityIds)
                {
                    var before = await _repository.GetByIdAsync(activityId, businessId, transaction);
                    // Si ya no existe (por ejemplo, fue eliminada como sub-actividad de un padre procesado antes), la ignoramos.
                    if (before == null)
                        continue;

                    var result = await _repository.DeleteAsync(activityId, businessId, userId, transaction);
                    if (result.Status != 1)
                    {
                        await transaction.RollbackAsync();
                        return result; // Retornamos el error amigable del SP en lugar de crashear con un 500
                    }

                    lastResult = result;

                    var auditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.OperationsWorkOrderActivity,
                        activityId,
                        userId);

                    await _auditService.RegisterDeleteAsync(before, auditLog, transaction);
                    successCount++;
                }

                await transaction.CommitAsync();

                if (successCount == 0 && activityIds.Count > 0)
                    return new BaseResponse { Status = 0, Message = "No se encontró ninguna actividad válida para eliminar." };

                return lastResult;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
    
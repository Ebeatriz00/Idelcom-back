using Application.DTOs.Operations.OperationsWorkOrderActivity;
using Core.Entities.Operations;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System.Data;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class CloneOperationsWorkOrderActivity(
        IOperationsWorkOrderActivityRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        public async Task<BaseResponse> ExecuteAsync(
            OperationsWorkOrderActivityCloneDto dto,
            long userId,
            long businessId)
        {
            if (dto.Quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(dto.Quantity));

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var originalActivity = await _repository.GetByIdAsync(dto.ActivityId, businessId, transaction);
                if (originalActivity == null)
                    throw new InvalidOperationException("La actividad a clonar no existe.");

                // Check if it's already a subactivity
                if (originalActivity.ParentActivityId.HasValue)
                    throw new InvalidOperationException("No se puede clonar directamente una subactividad. Clona la actividad principal.");

                var subActivities = await _repository.GetSubActivitiesAsync(dto.ActivityId, businessId);

                for (int i = 0; i < dto.Quantity; i++)
                {
                    // Clone parent
                    var newParent = new OperationWorkOrderActivity
                    {
                        WorkOrderId = originalActivity.WorkOrderId,
                        ActivityName = originalActivity.ActivityName,
                        MeasurementUnitId = originalActivity.MeasurementUnitId,
                        ComplexityId = originalActivity.ComplexityId,
                        TargetQuantity = originalActivity.TargetQuantity,
                        ParentActivityId = null
                    };

                    var parentResult = await _repository.CreateAsync(newParent, userId, businessId, transaction);
                    if (parentResult.Id == null)
                        throw new InvalidOperationException("No se pudo crear el clon de la actividad principal.");

                    newParent.ActivityId = (long)parentResult.Id;
                    newParent.BusinessId = businessId;

                    var auditLogParent = _auditLogFactory.Create(businessId, TableNames.OperationsWorkOrderActivity, newParent.ActivityId, userId);
                    await _auditService.RegisterCreateAsync(newParent, auditLogParent, transaction);

                    // Clone subactivities
                    foreach (var sub in subActivities)
                    {
                        var newSub = new OperationWorkOrderActivity
                        {
                            WorkOrderId = sub.WorkOrderId,
                            ActivityName = sub.ActivityName,
                            MeasurementUnitId = sub.MeasurementUnitId,
                            ComplexityId = sub.ComplexityId,
                            TargetQuantity = sub.TargetQuantity,
                            ParentActivityId = newParent.ActivityId
                        };

                        var subResult = await _repository.CreateAsync(newSub, userId, businessId, transaction);
                        if (subResult.Id == null)
                            throw new InvalidOperationException("No se pudo crear el clon de una subactividad.");

                        newSub.ActivityId = (long)subResult.Id;
                        newSub.BusinessId = businessId;

                        var auditLogSub = _auditLogFactory.Create(businessId, TableNames.OperationsWorkOrderActivity, newSub.ActivityId, userId);
                        await _auditService.RegisterCreateAsync(newSub, auditLogSub, transaction);
                    }
                }

                await transaction.CommitAsync();

                return new BaseResponse
                {
                    Status = 1,
                    Message = "Clonación realizada con éxito."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

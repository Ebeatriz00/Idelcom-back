using Application.DTOs.Operations.OperationsWorkOrderActivity;
using Core.Interfaces.Operations;
using Core.Projections.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class GetAppActivitiesByResponsible(IOperationsWorkOrderActivityRepository repository)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;

        public async Task<AppActivitiesListResponseDto> ExecuteAsync(long userId, long businessId)
        {
            var (operations, workOrders, rootActivities, subActivities) = await _repository.GetAppActivitiesByResponsibleAsync(userId, businessId);

            if (operations == null || !operations.Any())
            {
                return new AppActivitiesListResponseDto
                {
                    Status = 1,
                    Message = "No se encontraron actividades asignadas para su usuario."
                };
            }

            var subActivitiesByParent = subActivities
                .Where(s => s.ParentActivityId.HasValue)
                .GroupBy(s => s.ParentActivityId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var rootActivitiesByWorkOrder = rootActivities
                .GroupBy(r => r.WorkOrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var workOrdersByOperation = workOrders
                .GroupBy(w => w.OperationId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var groupedData = operations.Select(op => new AppProjectGroupResponseDto
            {
                OperationId = op.OperationId,
                OperationName = op.OperationName,
                WorkOrders = workOrdersByOperation.TryGetValue(op.OperationId, out var opWorkOrders)
                    ? opWorkOrders.Select(wo => new AppWorkOrderGroupResponseDto
                    {
                        WorkOrderId = wo.WorkOrderId,
                        WorkOrderName = wo.WorkOrderName,
                        WorkOrderProgress = wo.WorkOrderProgress,
                        Activities = rootActivitiesByWorkOrder.TryGetValue(wo.WorkOrderId, out var woRootActivities)
                            ? woRootActivities.Select(ra => MapToActivityDto(ra, subActivitiesByParent)).ToList()
                            : new List<AppActivityDetailResponseDto>()
                    }).ToList()
                    : new List<AppWorkOrderGroupResponseDto>()
            }).ToList();

            return new AppActivitiesListResponseDto
            {
                Status = 1,
                Message = "Actividades recuperadas exitosamente.",
                Data = groupedData
            };
        }

        private static AppActivityDetailResponseDto MapToActivityDto(AppRootActivityProjection ra, Dictionary<long, List<AppSubActivityProjection>> subActivitiesByParent)
        {
            var dto = new AppActivityDetailResponseDto
            {
                ActivityId = ra.ActivityId,
                ActivityName = ra.ActivityName,
                TargetQuantity = ra.TargetQuantity,
                CurrentQuantity = ra.CurrentQuantity,
                ActivityProgress = ra.ActivityProgress,
                MeasurementUnitName = ra.MeasurementUnitName,
                MeasurementUnitSymbol = ra.MeasurementUnitSymbol,
                ComplexityName = ra.ComplexityName,
                ComplexityWeightFactor = ra.ComplexityWeightFactor,
                HasChildren = ra.HasChildren
            };

            if (ra.HasChildren && subActivitiesByParent.TryGetValue(ra.ActivityId, out var children))
            {
                dto.SubActivities = children.Select(c => MapSubToActivityDto(c, subActivitiesByParent)).ToList();
            }

            return dto;
        }

        private static AppActivityDetailResponseDto MapSubToActivityDto(AppSubActivityProjection sa, Dictionary<long, List<AppSubActivityProjection>> subActivitiesByParent)
        {
            var dto = new AppActivityDetailResponseDto
            {
                ActivityId = sa.ActivityId,
                ParentActivityId = sa.ParentActivityId,
                ActivityName = sa.ActivityName,
                TargetQuantity = sa.TargetQuantity,
                CurrentQuantity = sa.CurrentQuantity,
                ActivityProgress = sa.ActivityProgress,
                MeasurementUnitName = sa.MeasurementUnitName,
                MeasurementUnitSymbol = sa.MeasurementUnitSymbol,
                ComplexityName = sa.ComplexityName,
                ComplexityWeightFactor = sa.ComplexityWeightFactor,
                HasChildren = subActivitiesByParent.ContainsKey(sa.ActivityId)
            };

            if (subActivitiesByParent.TryGetValue(sa.ActivityId, out var children))
            {
                dto.SubActivities = children.Select(c => MapSubToActivityDto(c, subActivitiesByParent)).ToList();
            }

            return dto;
        }
    }
}

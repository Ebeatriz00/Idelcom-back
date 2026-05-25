using Application.DTOs.Operations.OperationsWorkOrderActivity;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderActivity
{
    public class GetAppActivitiesByResponsible(IOperationsWorkOrderActivityRepository repository)
    {
        private readonly IOperationsWorkOrderActivityRepository _repository = repository;

        public async Task<AppActivitiesListResponseDto> ExecuteAsync(long userId, long businessId)
        {
            var flatData = await _repository.GetAppActivitiesByResponsibleAsync(userId, businessId);

            if (flatData == null || !flatData.Any())
            {
                return new AppActivitiesListResponseDto
                {
                    Status = 1,
                    Message = "No se encontraron actividades asignadas para su usuario."
                };
            }

            var groupedData = flatData
                .GroupBy(p => new { p.OperationId, p.OperationName })
                .Select(projectGroup => new AppProjectGroupResponseDto
                {
                    OperationId = projectGroup.Key.OperationId,
                    OperationName = projectGroup.Key.OperationName,
                    WorkOrders = projectGroup
                        .GroupBy(wo => new { wo.WorkOrderId, wo.WorkOrderName, wo.WorkOrderProgress })
                        .Select(woGroup => new AppWorkOrderGroupResponseDto
                        {
                            WorkOrderId = woGroup.Key.WorkOrderId,
                            WorkOrderName = woGroup.Key.WorkOrderName,
                            WorkOrderProgress = woGroup.Key.WorkOrderProgress,
                            Activities = woGroup.Select(a => new AppActivityDetailResponseDto
                            {
                                ActivityId = a.ActivityId,
                                ActivityName = a.ActivityName,
                                TargetQuantity = a.TargetQuantity,
                                CurrentQuantity = a.CurrentQuantity,
                                ActivityProgress = a.ActivityProgress,
                                MeasurementUnitName = a.MeasurementUnitName,
                                MeasurementUnitSymbol = a.MeasurementUnitSymbol,
                                ComplexityName = a.ComplexityName,
                                ComplexityWeightFactor = a.ComplexityWeightFactor
                            }).ToList()
                        }).ToList()
                }).ToList();

            return new AppActivitiesListResponseDto
            {
                Status = 1,
                Message = "Actividades recuperadas exitosamente.",
                Data = groupedData
            };
        }
    }
}

using Application.DTOs.Operations.OperationsWorkOrder;
using AutoMapper;
using Core.Interfaces.Operations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Operations.OperationsWorkOrder
{
    public class GetOperationsWorkOrderProgressReport(IOperationsWorkOrderRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsWorkOrderProgressReportResponseDto> ExecuteAsync(long businessId, long operationsId)
        {
            var (summaries, details) = await _repository.GetProgressReportAsync(businessId, operationsId);

            var summaryDtos = summaries.Select(s => new OperationsWorkOrderSummaryDto
            {
                WorkOrderId = s.WorkOrderId,
                WorkOrderCode = s.WorkOrderCode,
                ProgressPercentage = s.ProgressPercentage,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                ResponsibleName = s.ResponsibleName
            }).ToList();

            var detailDtos = details.Select(d => new OperationsWorkOrderProgressDetailDto
            {
                ProgressId = d.ProgressId,
                ActivityId = d.ActivityId,
                ActivityName = d.ActivityName,
                ReportedDate = d.ReportedDate,
                ReportedQuantity = d.ReportedQuantity,
                WorkerId = d.WorkerId,
                WorkerName = d.WorkerName,
                Observations = d.Observations,
                TargetQuantity = d.TargetQuantity,
                CurrentQuantity = d.CurrentQuantity,
                MeasurementUnitSymbol = d.MeasurementUnitSymbol
            }).ToList();

            return new OperationsWorkOrderProgressReportResponseDto
            {
                Summaries = summaryDtos,
                Details = detailDtos
            };
        }
    }
}

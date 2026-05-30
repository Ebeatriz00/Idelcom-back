using System;
using System.Collections.Generic;

namespace Application.DTOs.Operations.OperationsWorkOrder
{
    public class OperationsWorkOrderSummaryDto
    {
        public long WorkOrderId { get; set; }
        public string? WorkOrderCode { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ResponsibleName { get; set; }
    }

    public class OperationsWorkOrderProgressDetailDto
    {
        public long ProgressId { get; set; }
        public long ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public DateTime ReportedDate { get; set; }
        public decimal ReportedQuantity { get; set; }
        public long? WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public string? Observations { get; set; }
        public decimal? TargetQuantity { get; set; }
        public decimal? CurrentQuantity { get; set; }
        public string? MeasurementUnitSymbol { get; set; }
        public decimal? ActivityProgressPercentage { get; set; }
    }

    public class OperationsWorkOrderProgressReportResponseDto
    {
        public IEnumerable<OperationsWorkOrderSummaryDto> Summaries { get; set; } = new List<OperationsWorkOrderSummaryDto>();
        public IEnumerable<OperationsWorkOrderProgressDetailDto> Details { get; set; } = new List<OperationsWorkOrderProgressDetailDto>();
    }
}

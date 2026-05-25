using System.Collections.Generic;

namespace Application.DTOs.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressResponseDto
    {
        public long ProgressId { get; set; }
        public long ActivityId { get; set; }
        public long BusinessId { get; set; }
        public DateTime ReportedDate { get; set; }
        public decimal ReportedQuantity { get; set; }
        public long? WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public string? ActivityName { get; set; }
        public decimal? TargetQuantity { get; set; }
        public decimal? CurrentQuantity { get; set; }
        public string? Observations { get; set; }
        public List<OperationsWorkOrderProgressPhotoDto> Photos { get; set; } = new();
    }
}
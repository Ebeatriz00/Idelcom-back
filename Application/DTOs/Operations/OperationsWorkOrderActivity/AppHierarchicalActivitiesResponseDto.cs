using SharedKernel;

namespace Application.DTOs.Operations.OperationsWorkOrderActivity
{
    public class AppActivitiesListResponseDto : BaseResponse
    {
        public List<AppProjectGroupResponseDto> Data { get; set; } = [];
    }

    public class AppProjectGroupResponseDto
    {
        public long OperationId { get; set; }
        public string? OperationName { get; set; }
        public List<AppWorkOrderGroupResponseDto> WorkOrders { get; set; } = [];
    }

    public class AppWorkOrderGroupResponseDto
    {
        public long WorkOrderId { get; set; }
        public string? WorkOrderName { get; set; }
        public decimal? WorkOrderProgress { get; set; }
        public List<AppActivityDetailResponseDto> Activities { get; set; } = [];
    }

    public class AppActivityDetailResponseDto
    {
        public long ActivityId { get; set; }
        public long? ParentActivityId { get; set; }
        public string? ActivityName { get; set; }
        public decimal TargetQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal ActivityProgress { get; set; }
        public string? MeasurementUnitName { get; set; }
        public string? MeasurementUnitSymbol { get; set; }
        public string? ComplexityName { get; set; }
        public decimal ComplexityWeightFactor { get; set; }
        public bool HasChildren { get; set; }
        public List<AppActivityDetailResponseDto> SubActivities { get; set; } = [];
    }
}

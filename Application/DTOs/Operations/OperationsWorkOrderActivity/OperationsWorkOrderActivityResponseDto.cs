namespace Application.DTOs.Operations.OperationsWorkOrderActivity
{
    public class OperationsWorkOrderActivityResponseDto
    {
        public long ActivityId { get; set; }
        public long WorkOrderId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public long MeasurementUnitId { get; set; }
        public string? MeasurementUnitName { get; set; }
        public string? MeasurementUnitSymbol { get; set; }
        public int ComplexityId { get; set; }
        public string? ComplexityName { get; set; }
        public decimal WeightFactor { get; set; }
        public decimal TargetQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal ProgressPercentage { get; set; }
        public long? ParentActivityId { get; set; }
    }
}

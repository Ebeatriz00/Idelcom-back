namespace Core.Projections.Operations
{
    public class AppActivityWorkOrderProjection
    {
        public long OperationId { get; set; }
        public string? OperationName { get; set; }
        public long WorkOrderId { get; set; }
        public string? WorkOrderName { get; set; }
        public decimal? WorkOrderProgress { get; set; }
        public long ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public decimal TargetQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal ActivityProgress { get; set; }
        public string? MeasurementUnitName { get; set; }
        public string? MeasurementUnitSymbol { get; set; }
        public string? ComplexityName { get; set; }
        public decimal ComplexityWeightFactor { get; set; }
        public long? ParentActivityId { get; set; }
        public bool HasChildren { get; set; }
    }
}

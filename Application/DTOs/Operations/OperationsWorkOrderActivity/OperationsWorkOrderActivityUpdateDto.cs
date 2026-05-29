namespace Application.DTOs.Operations.OperationsWorkOrderActivity
{
    public class OperationsWorkOrderActivityUpdateDto
    {
        public long ActivityId { get; set; }
        public long WorkOrderId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public long MeasurementUnitId { get; set; }
        public int ComplexityId { get; set; }
        public decimal TargetQuantity { get; set; }
        public long? ParentActivityId { get; set; }
    }
}

namespace Application.DTOs.Operations.OperationsWorkOrderActivity
{
    public class OperationsWorkOrderActivityCreateDto
    {
        public long WorkOrderId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public long MeasurementUnitId { get; set; }
        public int ComplexityId { get; set; }
        public decimal TargetQuantity { get; set; }
    }
}

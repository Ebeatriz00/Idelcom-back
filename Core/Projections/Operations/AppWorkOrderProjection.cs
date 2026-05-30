namespace Core.Projections.Operations
{
    public class AppWorkOrderProjection
    {
        public long WorkOrderId { get; set; }
        public long OperationId { get; set; }
        public string? WorkOrderName { get; set; }
        public decimal? WorkOrderProgress { get; set; }
    }
}
namespace Core.Projections.Operations
{
    public class OperationsWorkOrderActivitySelectItem
    {
        public long ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public long? ParentActivityId { get; set; }
        public long WorkOrderId { get; set; }
    }
}
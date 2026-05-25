namespace Application.DTOs.Operations.OperationsWorkOrder
{
    public class OperationsWorkOrderByIdDto
    {
        public long WorkOrderId { get; set; }
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public string? WorkOrderCode { get; set; }
        public string? WorkOrderName { get; set; }
        public long OrderStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public bool NeedLogistics { get; set; }
        public bool NeedSsoma { get; set; }
        public bool NeedAttendance { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? Status { get; set; }
        public bool IsAdministrative { get; set; }
    }
}

namespace Application.DTOs.Operations.OperationsWorkOrder
{
    public class OperationsWorkOrderUpdateDto
    {
        public long WorkOrderId { get; set; }
        public long OperationsId { get; set; }
        public string? WorkOrderName { get; set; }
        public long OrderStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public bool NeedLogistics { get; set; }
        public bool NeedSsoma { get; set; }
        public bool NeedAttendance { get; set; }
        public bool IsAdministrative { get; set; }
        public long UpdateUser { get; set; }
    }
}

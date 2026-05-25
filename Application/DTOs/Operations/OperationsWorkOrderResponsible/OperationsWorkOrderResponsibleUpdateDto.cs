namespace Application.DTOs.Operations.OperationsWorkOrderResponsible
{
    public class OperationsWorkOrderResponsibleUpdateDto
    {
        public long WorkOrderResponsibleId { get; set; }
        public long WorkOrderId { get; set; }
        public long WorkerId { get; set; }
        public bool? IsMain { get; set; }
    }
}

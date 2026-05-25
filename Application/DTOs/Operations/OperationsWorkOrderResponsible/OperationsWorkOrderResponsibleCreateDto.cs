namespace Application.DTOs.Operations.OperationsWorkOrderResponsible
{
    public class OperationsWorkOrderResponsibleCreateDto
    {
        public long WorkOrderId { get; set; }
        public long WorkerId { get; set; }
        public bool IsMain { get; set; }
    }
}

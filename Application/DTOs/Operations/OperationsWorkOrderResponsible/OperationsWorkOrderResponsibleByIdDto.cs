namespace Application.DTOs.Operations.OperationsWorkOrderResponsible
{
    public class OperationsWorkOrderResponsibleByIdDto
    {
        public long WorkOrderResponsibleId { get; set; }
        public long BusinessId { get; set; }
        public long WorkOrderId { get; set; }
        public long WorkerId { get; set; }
        public bool IsMain { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }
    }
}

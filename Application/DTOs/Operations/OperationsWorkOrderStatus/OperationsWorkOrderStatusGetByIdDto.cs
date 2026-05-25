namespace Application.DTOs.Operations.OperationsWorkOrderStatus
{
    public class OperationsWorkOrderStatusGetByIdDto
    {
        public long WorkOrderStatusId { get; set; }
        public long BusinessId { get; set; }
        public required string StatusDesc { get; set; }
        public required string StatusColor { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdateUser { get; set; }
        public string? Status { get; set; }
    }
}

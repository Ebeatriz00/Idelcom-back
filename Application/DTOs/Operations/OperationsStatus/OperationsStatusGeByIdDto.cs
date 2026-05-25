namespace Application.DTOs.Operations.OperationsStatus
{
    public class OperationsStatusGeByIdDto
    {
        public int OperationsStatusId { get; set; }
        public long BusinessId { get; set; }
        public required string StateDesc { get; set; }
        public required string StateColor { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdateUser { get; set; }
        public string? Status { get; set; }
    }
}

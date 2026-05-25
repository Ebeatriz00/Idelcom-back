namespace Application.DTOs.Operations.AssignmentStatus
{
    public class AssignmentStatusGetByIdDto
    {
        public int AssignmentStatusId { get; set; }
        public long BusinessId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

}

namespace Application.DTOs.Operations.AttendanceStatus
{
    public class AttendanceStatusGetByIdDto
    {
        public int AttendanceStatusId { get; set; }
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

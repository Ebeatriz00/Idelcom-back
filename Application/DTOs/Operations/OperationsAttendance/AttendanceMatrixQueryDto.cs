namespace Application.DTOs.Operations.OperationsAttendance
{
    public class AttendanceMatrixQueryDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? OpporId { get; set; }
        public long? WorkOrderId { get; set; }
        public long? SquadId { get; set; }
        public string? Search { get; set; }
        public int? StatusId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

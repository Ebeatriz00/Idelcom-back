namespace Application.DTOs.Operations.OperationsAttendance
{
    public class AppAttendanceV2CreateDto
    {
        public DateTime AttendanceDate { get; set; }
        public long OperationsId { get; set; }
        public string SessionType { get; set; } = "ENTRADA";
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        public List<AppAttendanceBatchV2DetailDto> Details { get; set; } = [];
    }

    public class AppAttendanceBatchV2DetailDto
    {
        public long? AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public int AttendanceStatusId { get; set; }
        public DateTime CheckTime { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyExitMinutes { get; set; }
        public string? Observation { get; set; }
        public Guid? PhotoUid { get; set; }
    }
}

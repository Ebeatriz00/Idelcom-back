namespace Application.DTOs.Operations.OperationsAttendance
{
    public class AppAttendanceDailyV2ResponseDto
    {
        public string AttendanceDate { get; set; } = string.Empty;
        public List<AppAttendanceOperationV2Dto> Operations { get; set; } = [];
        public List<AppAttendanceStatusDto> AttendanceStatuses { get; set; } = [];
    }

    public class AppAttendanceOperationV2Dto
    {
        public long OperationsId { get; set; }
        public string OperationsDesc { get; set; } = string.Empty;
        public List<AppAttendanceProjectConfigDto> ProjectConfigs { get; set; } = [];
        public AppAttendanceSessionsDto Sessions { get; set; } = new();
        public List<AppAttendanceWorkerV2Dto> Workers { get; set; } = [];
    }

    public class AppAttendanceWorkerV2Dto
    {
        public long? AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public long JobTitleId { get; set; }
        public string JobTitleDesc { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public AppAttendanceDetailDto? Attendance { get; set; }
    }
}

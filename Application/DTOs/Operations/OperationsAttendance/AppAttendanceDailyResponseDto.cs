namespace Application.DTOs.Operations.OperationsAttendance
{
    public class AppAttendanceDailyResponseDto
    {
        public string AttendanceDate { get; set; } = string.Empty;
        public List<AppAttendanceOperationDto> Operations { get; set; } = [];
        public List<AppAttendanceStatusDto> AttendanceStatuses { get; set; } = [];

    }

    public class AppAttendanceOperationDto
    {
        public long OperationsId { get; set; }
        public string OperationsDesc { get; set; } = string.Empty;
        public List<AppAttendanceProjectConfigDto> ProjectConfigs { get; set; } = [];
        public List<AppAttendanceWorkOrderDto> WorkOrders { get; set; } = [];
    }

    public class AppAttendanceWorkOrderDto
    {
        public long WorkOrderId { get; set; }
        public string WorkOrderCode { get; set; } = string.Empty;
        public string WorkOrderName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsAdministrative { get; set; }
        public List<AppAttendanceSquadDto> Squads { get; set; } = [];
    }

    public class AppAttendanceProjectConfigDto
    {
        public long OperationsProjectConfigId { get; set; }
        public string EntryTime { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public bool AllowDelay { get; set; }
        public int MinutesTolerance { get; set; }
        public string BeforeOfficialTime { get; set; } = string.Empty;
        public bool IsRequirePhoto { get; set; }
        public int Shift { get; set; }
    }

    public class AppAttendanceSquadDto
    {
        public long SquadId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public string SquadName { get; set; } = string.Empty;
        public string SquadCategory { get; set; } = string.Empty;
        public AppAttendanceTechLeaderDto TechLeader { get; set; } = new();
        public AppAttendanceSessionsDto Sessions { get; set; } = new();
        public List<AppAttendanceWorkerDto> Workers { get; set; } = [];
    }

    public class AppAttendanceTechLeaderDto
    {
        public long WorkerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
    }

    public class AppAttendanceSessionsDto
    {
        public AppAttendanceSessionInfoDto? Entrada { get; set; }
        public AppAttendanceSessionInfoDto? Salida { get; set; }
    }

    public class AppAttendanceSessionInfoDto
    {
        public long AttendanceSessionId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalWorkers { get; set; }
        public Guid? GroupPhotoUid { get; set; }
    }

    public class AppAttendanceWorkerDto
    {
        public long AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public int JobTitleId { get; set; }
        public string JobTitleDesc { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public AppAttendanceDetailDto? Attendance { get; set; }
    }

    public class AppAttendanceDetailDto
    {
        public long AttendanceId { get; set; }
        public long? CheckInSessionId { get; set; }
        public long? CheckOutSessionId { get; set; }
        public int StatusId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public bool IsLate { get; set; }
        public int LateMinutes { get; set; }
        public int? EarlyExitMinutes { get; set; }
        public string? Observation { get; set; }
        public Guid? CheckInPhotoUid { get; set; }
        public Guid? CheckOutPhotoUid { get; set; }
    }

    public class AppAttendanceStatusDto
    {
        public int StatusId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string StatusType { get; set; } = string.Empty;
    }
}

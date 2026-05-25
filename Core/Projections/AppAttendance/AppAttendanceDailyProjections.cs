namespace Core.Projections.AppAttendance
{
    public class AppAttendanceOperationProjection
    {
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public string OperationsDesc { get; set; } = string.Empty;
        public TimeSpan EntryTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public bool AllowDelay { get; set; }
        public int MinutesTolerance { get; set; }
        public TimeSpan BeforeOfficialTime { get; set; }
        public bool IsRequirePhoto { get; set; }
    }

    public class AppAttendanceWorkOrderProjection
    {
        public long BusinessId { get; set; }
        public long WorkOrderId { get; set; }
        public long OperationsId { get; set; }
        public string WorkOrderCode { get; set; } = string.Empty;
        public string WorkOrderName { get; set; } = string.Empty;
        public int OrderStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool NeedAttendance { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsAdministrative { get; set; }

    }

    public class AppAttendanceSquadProjection
    {
        public long BusinessId { get; set; }
        public long SquadId { get; set; }
        public long WorkOrderId { get; set; }
        public long OperationsId { get; set; }
        public string SquadName { get; set; } = string.Empty;
        public long TechLeaderId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TechLeaderName { get; set; } = string.Empty;
        public string TechLeaderLastName { get; set; } = string.Empty;
        public string TechLeaderDocument { get; set; } = string.Empty;
        public string SquadCategory { get; set; } = string.Empty;
    }

    public class AppAttendanceWorkerProjection
    {
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public long WorkOrderId { get; set; }
        public long AssignmentId { get; set; }
        public long SquadId { get; set; }
        public long WorkerId { get; set; }
        public long AssignedBy { get; set; }
        public DateTime AssignmentDate { get; set; }
        public int AssignmentStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int JobTitleId { get; set; }
        public string JobTitleDesc { get; set; } = string.Empty;
        public string WorkerName { get; set; } = string.Empty;
        public string WorkerLastName { get; set; } = string.Empty;
        public string WorkerDocument { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }

    public class AppAttendanceSessionProjection
    {
        public long AttendanceSessionId { get; set; }
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public long WorkOrderId { get; set; }
        public long SquadId { get; set; }
        public DateTime SessionDate { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalWorkers { get; set; }
        public Guid? GroupPhotoUid { get; set; }
    }

    public class AppAttendanceDetailProjection
    {
        public long AttendanceId { get; set; }
        public long? CheckInSessionId { get; set; }
        public long? CheckOutSessionId { get; set; }
        public long AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public long? RegisteredBy { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int? StatusId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public bool? IsLate { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyExitMinutes { get; set; }
        public string Observation { get; set; } = string.Empty;
        public Guid? CheckInPhotoUid { get; set; }
        public Guid? CheckOutPhotoUid { get; set; }
    }

    public class AppAttendanceStatusProjection
    {
        public int StatusId { get; set; }
        public long BusinessId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusType { get; set; } = string.Empty;

    }

    public class AppAttendanceDailyResult
    {
        public IEnumerable<AppAttendanceOperationProjection> Operations { get; set; } = [];
        public IEnumerable<AppAttendanceWorkOrderProjection> WorkOrders { get; set; } = [];
        public IEnumerable<AppAttendanceSquadProjection> Squads { get; set; } = [];
        public IEnumerable<AppAttendanceWorkerProjection> Workers { get; set; } = [];
        public IEnumerable<AppAttendanceSessionProjection> Sessions { get; set; } = [];
        public IEnumerable<AppAttendanceDetailProjection> Details { get; set; } = [];
        public IEnumerable<AppAttendanceStatusProjection> Statuses { get; set; } = [];
    }
}

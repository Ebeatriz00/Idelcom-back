namespace Core.Projections.AppAttendance
{
    public class AppAttendanceWorkerV2Projection
    {
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public long? AssignmentId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public long WorkerId { get; set; }
        public int JobTitleId { get; set; }
        public string JobTitleDesc { get; set; } = string.Empty;
        public string WorkerName { get; set; } = string.Empty;
        public string WorkerLastName { get; set; } = string.Empty;
        public string WorkerDocument { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class AppAttendanceSessionV2Projection
    {
        public long AttendanceSessionId { get; set; }
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public DateTime SessionDate { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalWorkers { get; set; }
        public Guid? GroupPhotoUid { get; set; }
    }

    public class AppAttendanceDailyV2Result
    {
        public IEnumerable<AppAttendanceOperationProjection> Operations { get; set; } = [];
        public IEnumerable<AppAttendanceProjectConfigProjection> Configs { get; set; } = [];
        public IEnumerable<AppAttendanceWorkerV2Projection> Workers { get; set; } = [];
        public IEnumerable<AppAttendanceSessionV2Projection> Sessions { get; set; } = [];
        public IEnumerable<AppAttendanceDetailProjection> Details { get; set; } = [];
        public IEnumerable<AppAttendanceStatusProjection> Statuses { get; set; } = [];
    }
}

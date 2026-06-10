namespace Core.Requests
{
    public class AppAttendanceBatchV2Request
    {
        public long BusinessId { get; set; }
        public long OperationsId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public long UserId { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        public Guid? GroupPhotoUid { get; set; }
        public List<AppAttendanceBatchV2DetailRequest> Details { get; set; } = [];

        public class AppAttendanceBatchV2DetailRequest
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
}

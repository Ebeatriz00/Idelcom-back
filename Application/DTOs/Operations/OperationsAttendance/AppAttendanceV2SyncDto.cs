using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Operations.OperationsAttendance
{
    public class AppAttendanceV2SyncDto
    {
        public DateTime AttendanceDate { get; set; }
        public long OperationsId { get; set; }
        public string SessionType { get; set; } = "ENTRADA";
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }
        public IFormFile? GroupPhoto { get; set; }
        public List<AppAttendanceBatchDetailV2SyncDto> Details { get; set; } = [];
    }

    public class AppAttendanceBatchDetailV2SyncDto
    {
        public long? AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public int AttendanceStatusId { get; set; }
        public DateTime CheckTime { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyExitMinutes { get; set; }
        public string? Observation { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }
}

using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Operations.OperationsAttendance
{
    public class AppAttendanceSyncDto
    {
        public DateTime AttendanceDate { get; set; }
        public long WorkOrderId { get; set; }
        public long SquadId { get; set; }
        public string SessionType { get; set; } = "ENTRADA";
        public DateTime SessionStartTime { get; set; }
        public DateTime SessionEndTime { get; set; }

        // Foto grupal (Opcional a nivel de asignación)
        public IFormFile? GroupPhoto { get; set; }

        public List<AppAttendanceBatchDetailSyncDto> Details { get; set; } = [];
    }

    public class AppAttendanceBatchDetailSyncDto
    {
        public long AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public int AttendanceStatusId { get; set; }
        public DateTime CheckTime { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyExitMinutes { get; set; }
        public string? Observation { get; set; }
        
        // Foto individual (Opcional a nivel de detalle)
        public IFormFile? PhotoFile { get; set; }
    }
}

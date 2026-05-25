namespace Core.Projections.Operations
{
    public class AttendanceMatrixResult
    {
        public IEnumerable<AttendanceMatrixProjectProjection> Projects { get; set; } = [];
        public IEnumerable<AttendanceMatrixWorkOrderProjection> WorkOrders { get; set; } = [];
        public IEnumerable<AttendanceMatrixSquadProjection> Squads { get; set; } = [];
        public IEnumerable<AttendanceMatrixDetailProjection> Details { get; set; } = [];
        public int TotalWorkers { get; set; }
    }

    public class AttendanceMatrixProjectProjection
    {
        public long OpporId { get; set; }
        public string OpporDesc { get; set; } = string.Empty;
        public string ClientsName { get; set; } = string.Empty;
    }

    public class AttendanceMatrixWorkOrderProjection
    {
        public long OpporId { get; set; }
        public long WorkOrderId { get; set; }
        public string WorkOrderCode { get; set; } = string.Empty;
        public string WorkOrderName { get; set; } = string.Empty;
    }

    public class AttendanceMatrixSquadProjection
    {
        public long WorkOrderId { get; set; }
        public long SquadId { get; set; }
        public string SquadName { get; set; } = string.Empty;
    }

    public class AttendanceMatrixDetailProjection
    {
        public long? CheckInId { get; set; }
        public long? CheckOutId { get; set; }
        public long? CheckInSessionId { get; set; }
        public long? CheckOutSessionId { get; set; }
        public long OpporId { get; set; }
        public long WorkOrderId { get; set; }
        public long SquadId { get; set; }
        public long AssignmentId { get; set; }
        public long WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public string WorkerDocument { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int AttendanceStatusId { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public bool? IsLate { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyExitMinutes { get; set; }
        public string Observation { get; set; } = string.Empty;
        public string ClientsName { get; set; } = string.Empty;
        public Guid? CheckInGroupPhotoUid { get; set; }
        public Guid? CheckOutGroupPhotoUid { get; set; }
        public Guid? CheckInPhotoUid { get; set; }
        public Guid? CheckOutPhotoUid { get; set; }
    }
}

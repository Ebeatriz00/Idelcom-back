using System;

namespace Application.DTOs.OperationsPersonnelMovement
{
    public class OperationPersonnelMovementResponseDto
    {
        public long MovementId { get; set; }
        public long BusinessId { get; set; }
        public long WorkerId { get; set; }
        public string WorkerName { get; set; }
        public long? FromOperationsId { get; set; }
        public string? FromOperationsName { get; set; }
        public long? ToOperationsId { get; set; }
        public string? ToOperationsName { get; set; }
        public long? FromSquadId { get; set; }
        public string? FromSquadName { get; set; }
        public long? ToSquadId { get; set; }
        public string? ToSquadName { get; set; }
        public DateTime MovementDate { get; set; }
        public DateTime ReleaseTime { get; set; }
        public DateTime? ReassignmentTime { get; set; }
        public int MovementStatusId { get; set; }
        public string? MovementReason { get; set; }
        public long? AuthorizedBy { get; set; }
        public long RegisteredBy { get; set; }
        public long? RegularizedBy { get; set; }
        public DateTime? RegularizedDate { get; set; }
        public string? Observation { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

using System;

namespace Application.DTOs.OperationsPersonnelAssignment
{
    public class OperationsPersonnelAssignmentResponseDto
    {
        public long AssignmentId { get; set; }
        public long BusinessId { get; set; }
        public long SquadId { get; set; }
        public string? SquadName { get; set; }
        public long WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public int AssignmentStatusId { get; set; }
        public string? AssignmentStatusName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string? Notes { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }
    }
}

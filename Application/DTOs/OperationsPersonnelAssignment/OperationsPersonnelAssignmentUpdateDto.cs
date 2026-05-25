using System;

namespace Application.DTOs.OperationsPersonnelAssignment
{
    public class OperationsPersonnelAssignmentUpdateDto
    {
        public long AssignmentId { get; set; }
        public long SquadId { get; set; }
        public long WorkerId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public int AssignmentStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string? Notes { get; set; }
        public long? UpdateUser { get; set; }
    }
}

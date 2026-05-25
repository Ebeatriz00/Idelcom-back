using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.OperationsPersonnelAssignment
{
    public class OperationPersonnelAssignmentProjection : BaseAuditableEntity
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
    }
}

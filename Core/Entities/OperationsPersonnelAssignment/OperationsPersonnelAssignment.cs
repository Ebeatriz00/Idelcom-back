using Core.Attributes;
using SharedKernel;
using System;

namespace Core.Entities.OperationsPersonnelAssignment
{
    public class OperationPersonnelAssignment : BaseAuditableEntity
    {
        public long AssignmentId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Id de la Cuadrilla")]
        public long SquadId { get; set; }
        public string? SquadName { get; set; }

        [AuditField("Id Trabajador")]
        public long WorkerId { get; set; }
        public string? WorkerName { get; set; }

        [AuditField("Fecha de Asignacin")]
        public DateTime AssignmentDate { get; set; }

        [AuditField("Id Status de Asignacin")]
        public int AssignmentStatusId { get; set; }
        public string? AssignmentStatusName { get; set; }

        [AuditField("Fecha de Inicio")]
        public DateTime StartDate { get; set; }

        [AuditField("Fecha de Fin")]
        public DateTime? FinishDate { get; set; }

        [AuditField("Notas")]
        public string? Notes { get; set; }
    }
}

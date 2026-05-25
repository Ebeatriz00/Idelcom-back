using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OperationsPersonnelMovement
{
    public class OperationPersonnelMovement : BaseAuditableEntity
    {
        public long MovementId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Nombre del Trabajador")]
        public long WorkerId { get; set; }

        [AuditField("De esta Operación")]
        public long? FromOperationsId { get; set; }

        [AuditField("A esta Operación")]
        public long? ToOperationsId { get; set; }

        [AuditField("De esta Cuadrilla")]
        public long? FromSquadId { get; set; }

        [AuditField("A esta Cudrilla")]
        public long? ToSquadId { get; set; }

        [AuditField("Fecha de Movimiento")]
        public DateTime MovementDate { get; set; }

        [AuditField("Hora de Salida")]
        public DateTime ReleaseTime { get; set; }

        [AuditField("Hora de Reasignación")]
        public DateTime? ReassignmentTime { get; set; }

        [AuditField("Id del Estado de Movimiento")]
        public int MovementStatusId { get; set; }

        [AuditField("Razón del Movimiento")]
        public string? MovementReason { get; set; }

        [AuditField("Autorizado Por")]
        public long? AuthorizedBy { get; set; }

        [AuditField("Registrado Por")]
        public long RegisteredBy { get; set; }

        [AuditField("Regularizado Por")]
        public long? RegularizedBy { get; set; }

        [AuditField("Fecha de Regularización")]
        public DateTime? RegularizedDate { get; set; }

        [AuditField("Observación")]
        public string? Observation { get; set; }
    }
}

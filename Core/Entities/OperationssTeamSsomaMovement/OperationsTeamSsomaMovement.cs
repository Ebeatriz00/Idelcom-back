using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OperationsTeamSsomaMovement
{
    public class OperationsTeamSsomaMovement : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long OperationsSsomaMovementId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Id SSOMA")]
        public long OperationsTeamSsomaId { get; set; }

        [AuditField("Tipo de movimiento SSOMA")]
        public long SssomaMovementTypeId { get; set; }

        [AuditField("Fecha de movimiento")]
        public DateTime MovementDate { get; set; }

        [AuditField("De procesos SSOMA")]
        public long? FromSsomaProcessId {  get; set; }

        [AuditField("A procesos SSOMA")]
        public long? ToSsomaProcessId { get; set; }

        [AuditField("Razón de movimiento")]
        public string? Description { get; set; }
    }
}

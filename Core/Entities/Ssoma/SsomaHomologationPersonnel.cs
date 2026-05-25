             using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Ssoma
{
    public class SsomaHomologationPersonnel : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long HomologationPersonnelId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("General/Proyecto")]
        public long HomologationScopeId { get; set; }

        [AuditField("Operación")]
        public long? OperationsId { get; set; }

        [AuditField("Trabajador")]
        public long WorkerId { get; set; } = 0;

        [AuditField("Estado Trabajador")]
        public long WorkerStatusId { get; set; }

        [AuditField("Aptitud Médica")]
        public long MedicalAptitudeId { get; set; }

        [AuditField("Válido Desde")]
        public DateTime ValidFrom { get; set; }

        [AuditField("Válido Hasta")]
        public DateTime ValidTo { get; set; }

        [AuditField("Aprobado por Ssoma")]
        public bool SsomaApproved { get; set; }

        [AuditField("Notas")]
        public string Notes { get; set; } = null!;
    }
}

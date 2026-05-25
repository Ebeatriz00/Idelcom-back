
using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Ssoma
{
    public class SsomaProcess : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long SsomaProcessId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Indentificador de operaciones")]
        public long OperationsId { get; set; }

        [AuditField("Es requerido homogolación de empresa")]
        public bool RequiresCompanyHomologation { get; set; }

        [AuditField("Es requerido equipo ssoma")]
        public bool RequieresOperationTeamSsoma { get; set; }

        [AuditField("Estado Actual")]
        public int CurrentStatusId { get; set; }

        [AuditField("Fecha de solicitud")]
        public DateTime? RequestDate { get; set; }

        [AuditField("Fecha de presentación")]
        public DateTime? SubmissionsDate { get; set; }

        [AuditField("Fecha de aprobación")]
        public DateTime? ApprovalDate { get; set; }

        [AuditField("Fecha de inicio")]
        public DateTime? StartDate { get; set; }

        [AuditField("Fecha fin")]
        public DateTime? EndDate { get; set; }

        [AuditField("Obs general")]
        public string? GeneralObservation { get; set; }


    }
}

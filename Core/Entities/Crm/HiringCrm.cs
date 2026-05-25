using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Crm
{
    public class HiringCrm : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long LicReqId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Oportunidad")]
        public long OpporId { get; set; }

        [AuditField("Tipo de proceso")]
        public int? ProcessType { get; set; }

        [AuditField("Estado de licitacion")]
        public long? LicStatusId { get; set; }

        [AuditField("Nota de solicitud")]
        public string? RequestNote { get; set; }

        [AuditField("Solicitado por")]
        public long? RequestedBy { get; set; }

        [AuditField("Fecha de solicitud")]
        public DateTime? RequestedAt { get; set; }

        [AuditField("Licitador")]
        public long LicitatorId { get; set; }

        [AuditField("Fecha de revision")]
        public DateTime? ReviewedAt { get; set; }

        [AuditField("Ejercicio")]
        public int? Exercises { get; set; }
    }
}

using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Ssoma
{
    public class SsomaOperationsRequirement : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long SsomaOperationsRequirementId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Operación")]
        public  long OperationsId { get; set; }

        [AuditField("Requerimiento")]
        public int RequirementId { get; set; }

        [AuditField("Obligatorio")]
        public bool IsMandatory { get; set; }

        [AuditField("Días Válidos")]
        public int? ValidDays { get; set; } 

    }
}

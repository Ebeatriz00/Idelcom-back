using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Operations
{
    public class WorkDayStatus : BaseAuditableEntity
    {
        [AuditField("Id del Status de la Jornada")]
        public long WorkDayStatusId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Descripción Estado")]
        public string StatusDesc { get; set; }

        [AuditField("Color Estado")]
        public string StatusColor { get; set; }
    }
}

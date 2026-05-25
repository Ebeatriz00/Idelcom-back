using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class ProductTypes : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long ProductTypesId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Tipo de Producto")]
        public string Description { get; set; } = null!;

        [AuditField("Consumible?")]
        public bool IsConsumable { get; set; }

        [AuditField("Retornable?")]
        public bool IsReturnable { get; set; }

        [AuditField("Requiere Serial?")]
        public bool RequiresSerial { get; set; }

    }
}

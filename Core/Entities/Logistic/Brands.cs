using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class Brands : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long BrandsId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Marca")]
        public string Description { get; set; } = null!;
    }
}

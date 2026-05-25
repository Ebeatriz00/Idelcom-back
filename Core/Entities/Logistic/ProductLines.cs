using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class ProductLines : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long ProductLinesId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Línea de productos")]
        public string Description { get; set; } = null!;

        [AuditField("Categoría")]
        public long CategoriesId { get; set; }
    }
}

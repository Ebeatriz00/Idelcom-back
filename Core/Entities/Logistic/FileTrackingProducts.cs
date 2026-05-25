using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class FileTrackingProducts : BaseAuditableEntity
    {
            [AuditField("Id")]
            public long FileTrackingProductsId { get; set; }

            [AuditField("Empresa")]
            public long BusinessId { get; set; }

            [AuditField("Producto")]
            public long ProductsId { get; set; }

            [AuditField("Nombre de Archivo")]
            public string FileTitle { get; set; } = null!;

            [AuditField("Ruta")]
            public string FileUrl { get; set; } = null!;

            [AuditField("Ruta Relativa")]
            public string RelativePath { get; set; } = null!;

    }
}

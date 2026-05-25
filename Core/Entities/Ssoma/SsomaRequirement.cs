using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Ssoma
{
    public class SsomaRequirement : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long RequirementId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Requerimiento")]
        public string Name { get; set; } = null!;

        [AuditField("Descripción")]
        public string Description { get; set; } = null!;

        [AuditField("Duración")]
        public int? Duration { get; set; }

        [AuditField("General/Proyecto")]
        public int ScopeId { get; set; }

        [AuditField("Tiene vencimiento")]
        public bool HasExpiration { get; set; }

        [AuditField("Requiere archivos")]
        public bool RequiresFile { get; set; }

        [AuditField("Requiere vencimiento")]
        public bool RequiresExpiration { get; set; }

        [AuditField("Peso maximo")]
        public int MaxFileSize { get; set; }

        [AuditField("Extensión permitida")]
        public string AllowedExtensions { get; set; } = null!;

        [AuditField("Permitir reutilización")]
        public bool AllowInternalReuse { get; set; }

        [AuditField("Es activo")]
        public bool IsActive { get; set; }

        
    }
}

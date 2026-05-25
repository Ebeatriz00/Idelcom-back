using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class Support : BaseAuditableEntity
    {
        [AuditField("Id Soporte")]
        public long SupportId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Proveedor")]
        public string? Provider { get; set; }

        [AuditField("Servicio")]
        public string? Service { get; set; }

        [AuditField("URL")]
        public string? Url { get; set; }

        [AuditField("Acceso")]
        public string? Access { get; set; }

        [AuditField("Email")]
        public string? Email { get; set; }

        [AuditField("Usuario")]
        public string? Username { get; set; }

        [AuditField("Contraseña")]
        public string? Password { get; set; }

        [AuditField("Estado de Apoyo")]
        public int? SupportState { get; set; }

        [AuditField("Fecha Inicio")]
        public DateTime? StartDate { get; set; }

        [AuditField("Fecha Expiración")]
        public DateTime? ExpirationDate { get; set; }

        [AuditField("Comentarios")]
        public string? Comments { get; set; }

        [AuditField("Observaciones")]
        public string? Remarks { get; set; }
    }
}

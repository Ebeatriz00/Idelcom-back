using Core.Attributes;
using SharedKernel;

namespace Core.Entities
{
    public class SupportState : BaseAuditableEntity
    {
        [AuditField("Id Estado de Soporte")]
        public int SupportStateId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Descripción")]
        public string? StatusDesc { get; set; }

        [AuditField("Color")]
        public string? StatusColor { get; set; }
    }
}

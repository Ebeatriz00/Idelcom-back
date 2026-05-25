using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class AssignmentStatus : BaseAuditableEntity
    {
        [AuditField("Id Estado Asignación")]
        public int AssignmentStatusId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Descripción Estado")]
        public string StatusDesc { get; set; } = string.Empty;

        [AuditField("Color Estado")]
        public string StatusColor { get; set; } = string.Empty;
    }
}

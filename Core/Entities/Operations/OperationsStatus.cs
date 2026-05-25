using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationsStatus : BaseAuditableEntity
    {
        [AuditField("OperationsStatusId")]
        public int OperationsStatusId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Descripción del estado")]
        public required string StateDesc { get; set; }

        [AuditField("Color del estado")]
        public required string StateColor { get; set; }
    }
}

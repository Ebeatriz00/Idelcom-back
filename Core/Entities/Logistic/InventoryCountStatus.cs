using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Logistic
{
    public class InventoryCountStatus : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long InventoryCountStatusId { get; set; }

        [AuditField("Código")]
        public string Code { get; set; } = null!;

        [AuditField("Descripción")]
        public string Description { get; set; } = null!;
    }
}

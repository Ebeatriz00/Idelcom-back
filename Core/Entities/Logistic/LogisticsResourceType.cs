using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Logistic
{
    public class LogisticsResourceType : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long LogisticsResourceTypeId { get; set; }

        [AuditField("Codigo")]
        public string Code { get; set; } = string.Empty;

        [AuditField("Descripcion")]
        public string Description { get; set; } = string.Empty;
    }
}

using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationWorkOrderStatus : BaseAuditableEntity
    {
        [AuditField("WorkOrderStatusId")]
        public long WorkOrderStatusId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Descripción Estado")]
        public required string StatusDesc { get; set; }

        [AuditField("Color Estado")]
        public required string StatusColor { get; set; }
    }
}

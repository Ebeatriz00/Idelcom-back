using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    namespace Core.Entities
    {
        public class OperationWorkOrderResponsible : BaseAuditableEntity
        {
            [AuditField("WorkOrderResponsibleId")]
            public long WorkOrderResponsibleId { get; set; }

            [AuditField("BusinessId")]
            public long BusinessId { get; set; }

            [AuditField("Orden de trabajo")]
            public long WorkOrderId { get; set; }

            [AuditField("Trabajador")]
            public long WorkerId { get; set; }

            [AuditField("Responsable principal")]
            public bool IsMain { get; set; }

            public string? WorkerName { get; set; }
        }
    }
}

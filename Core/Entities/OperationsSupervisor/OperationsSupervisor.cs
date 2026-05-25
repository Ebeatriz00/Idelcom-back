using Core.Attributes;
using SharedKernel;

namespace Core.Entities.OperationsSupervisor
{
    public class OperationSupervisor : BaseAuditableEntity
    {
        public long SupervisorId { get; set; }

        [AuditField("Id de Operaciones")]
        public long OperationsId { get; set; }

        [AuditField("Id del Supervisor")]
        public long WorkerId { get; set; }

        [AuditField("¿Es Supervisor Principal?")]
        public bool IsMain { get; set; }

    }
}

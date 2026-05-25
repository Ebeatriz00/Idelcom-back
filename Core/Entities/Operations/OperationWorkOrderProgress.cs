using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationWorkOrderProgress : BaseAuditableEntity
    {
        [AuditField("ProgressId")]
        public long ProgressId { get; set; }

        [AuditField("Actividad")]
        public long ActivityId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Fecha reportada")]
        public DateTime ReportedDate { get; set; }

        [AuditField("Cantidad reportada")]
        public decimal ReportedQuantity { get; set; }

        [AuditField("Trabajador")]
        public long? WorkerId { get; set; }

        public string? WorkerName { get; set; }

        public string? ActivityName { get; set; }

        public decimal? TargetQuantity { get; set; }

        public decimal? CurrentQuantity { get; set; }

        [AuditField("Observaciones")]
        public string? Observations { get; set; }
    }
}

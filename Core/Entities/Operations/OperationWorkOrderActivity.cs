using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationWorkOrderActivity : BaseAuditableEntity
    {
        [AuditField("ActivityId")]
        public long ActivityId { get; set; }

        [AuditField("Orden de trabajo")]
        public long WorkOrderId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Actividad")]
        public required string ActivityName { get; set; }

        [AuditField("Unidad de medida")]
        public long MeasurementUnitId { get; set; }

        [AuditField("Nombre de unidad de medida")]
        public string? MeasurementUnitName { get; set; }

        [AuditField("Simbolo de unidad de medida")]
        public string? MeasurementUnitSymbol { get; set; }

        [AuditField("Complejidad")]
        public int ComplexityId { get; set; }

        [AuditField("Nombre de complejidad")]
        public string? ComplexityName { get; set; }

        [AuditField("Factor de peso")]
        public decimal WeightFactor { get; set; }

        [AuditField("Cantidad meta")]
        public decimal TargetQuantity { get; set; }

        [AuditField("Cantidad actual")]
        public decimal CurrentQuantity { get; set; }

        [AuditField("Porcentaje de avance")]
        public decimal ProgressPercentage { get; set; }
    }
}

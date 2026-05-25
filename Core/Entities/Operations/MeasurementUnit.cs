using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    /// <summary>
    /// Entidad que representa una unidad de medida utilizada en el sistema.
    /// </summary>
    public class MeasurementUnit : BaseAuditableEntity
    {
        [AuditField("MeasurementUnitId")]
        public long MeasurementUnitId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Nombre de unidad")]
        public required string UnitName { get; set; }

        [AuditField("Simbolo")]
        public string? Symbol { get; set; }
    }
}

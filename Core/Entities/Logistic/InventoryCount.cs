using Core.Attributes;
using SharedKernel;
using System;

namespace Core.Entities.Logistic
{
    public class InventoryCount : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long InventoryCountId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Almacén")]
        public long WarehouseId { get; set; }

        [AuditField("N° de conteo")]
        public string CountNumber { get; set; } = null!;

        [AuditField("Fecha de conteo")]
        public DateTime CountDate { get; set; }

        [AuditField("Estado de inventario")]
        public long InventoryCountStatusId { get; set; }

        [AuditField("Observación")]
        public string? Observation { get; set; }
    }
}

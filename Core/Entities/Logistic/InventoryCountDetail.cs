using Core.Attributes;
using SharedKernel;
using System;

namespace Core.Entities.Logistic
{
    public class InventoryCountDetail : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long InventoryCountDetailId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Conteo de inventario")]
        public long InventoryCountId { get; set; }

        [AuditField("Producto")]
        public long ProductsId { get; set; }

        [AuditField("Cantidad sistema")]
        public decimal SystemQuantity { get; set; }

        [AuditField("Cantidad contada")]
        public decimal CountedQuantity { get; set; }

        [AuditField("Diferencia de cantidad")]
        public decimal DifferenceQuantity { get; set; }

        [AuditField("Costo unitario")]
        public decimal UnitCost { get; set; }

        [AuditField("Costo total de diferencia")]
        public decimal TotalDifferenceCost { get; set; }

        [AuditField("N° de lote")]
        public string? LotNumber { get; set; }

        [AuditField("N° de serie")]
        public string? SerialNumber { get; set; }

        [AuditField("Fecha de vencimiento")]
        public DateTime? ExpirationDate { get; set; }

        [AuditField("Movimiento de ajuste")]
        public long? AdjustmentMovementId { get; set; }

        [AuditField("Observación")]
        public string? Observation { get; set; }
    }
}

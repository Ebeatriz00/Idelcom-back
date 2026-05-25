using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class WarehouseMovementDetail : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long WarehouseMovementDetailId { get; set; }

        [AuditField("Movimiento")]
        public long WarehouseMovementId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Producto")]
        public long ProductsId { get; set; }

        [AuditField("Cantidad")]
        public decimal Quantity { get; set; }

        [AuditField("Costo unitario")]
        public decimal UnitCost { get; set; }

        [AuditField("Costo Total")]
        public decimal TotalCost { get; set; }

        [AuditField("N° de Lote")]
        public string? LotNumber { get; set; }

        [AuditField("N° de serie")]
        public string? SerialNumber { get; set; }

        [AuditField("Fecha de caducidad")]
        public DateTime? ExpirationDate { get; set; }

        [AuditField("observacion")]
        public string? Observation { get; set; }

        [AuditField("Detalle OC")]
        public long? PurchaseOrderDetailId { get; set; }

        [AuditField("Detalle de oc recibido")]
        public decimal? PurchaseReceiptDetailId { get; set; }

        
    }
}

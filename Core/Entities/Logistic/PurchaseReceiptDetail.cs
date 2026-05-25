using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class PurchaseReceiptDetail : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long PurchaseReceiptDetailId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Recepcion")]
        public long PurchaseReceiptId { get; set; }

        [AuditField("Id")]
        public long? PurchaseOrderDetailId { get; set; }

        [AuditField("Productos")]
        public long ProductsId { get; set; }

        [AuditField("Unidad de medida")]
        public long? UomId { get; set; }

        [AuditField("Cantidad de pedida")]
        public decimal OrderedQuantity { get; set; }

        [AuditField("Cantidad de recivida")]
        public decimal ReceivedQuantity { get; set; }

        [AuditField("Costo Unitario")]
        public decimal UnitCost { get; set; }

        [AuditField("Costo total")]
        public decimal TotalCost { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }

    }
}

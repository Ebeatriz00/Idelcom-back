using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Logistic
{
    public class PurchaseOrderDetail : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long PurchaseOrderDetailId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Orden de compra")]
        public long PurchaseOrderId { get; set; }

        [AuditField("Producto")]
        public long ProductsId { get; set; }

        [AuditField("Unidad de medida")]
        public long? UomId { get; set; }

        [AuditField("Cantidad")]
        public decimal Quantity { get; set; }

        [AuditField("Precio Unitario")]
        public decimal UnitPrice { get; set; }

        [AuditField("Porcentaje de descuento")]
        public decimal DiscountPercent { get; set; }

        [AuditField("Monto de descuento")]
        public decimal DiscountAmount { get; set; }

        [AuditField("Impuesto")]
        public long? TaxesId { get; set; }

        [AuditField("Precio incluye impuesto")]
        public bool PriceIncludesTax { get; set; }

        [AuditField("IGV Porcentaje")]
        public decimal IgvPercent { get; set; }

        [AuditField("IGV Total")]
        public decimal IgvAmount { get; set; }

        [AuditField("SubTotal")]
        public decimal Subtotal { get; set; }

        [AuditField("Total")]
        public decimal Total { get; set; }

        [AuditField("Cantidad recibida")]
        public decimal ReceivedQuantity { get; set; }

        [AuditField("Cantidad pendiente")]
        public decimal PendingQuantity { get; set; }

        [AuditField("Estado detalle")]
        public long? DetailStatusId { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }
    }
}

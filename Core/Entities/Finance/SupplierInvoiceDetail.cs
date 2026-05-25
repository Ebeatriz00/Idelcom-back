using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Finance
{
    public class SupplierInvoiceDetail : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long SupplierInvoiceDetailId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Factura Proveedor")]
        public long SupplierInvoiceId { get; set; }


        [AuditField("Detalle OC")]
        public long? PurchaseOrderDetailId { get; set; }

        [AuditField("Detalle de recepcion OC")]
        public long? PurchaseReceiptDetailId { get; set; }

        [AuditField("Producto")]
        public long ProductsId { get; set; }

        [AuditField("Unidad de medida")]
        public long UomId { get; set; }

        [AuditField("Precio unitario")]
        public decimal UnitPrice { get; set; }

        [AuditField("Porcentaje de descuento")]
        public decimal DiscountPercent { get; set; }

        [AuditField("Importe de descuento")]
        public decimal DiscountAmount { get; set; }

        [AuditField("Impuesto")]
        public long? TaxesId { get; set; }

        [AuditField("Procentaje Igv")]
        public decimal IgvPorcent { get; set; }

        [AuditField("Importe Igv")]
        public decimal IgvAmount { get; set; }

        [AuditField("SubTotal")]
        public decimal SubTotal { get; set; }

        [AuditField("Total")]
        public decimal Total { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }
    }
}

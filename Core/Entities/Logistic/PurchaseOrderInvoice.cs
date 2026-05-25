using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Logistic
{
    public class PurchaseOrderInvoice : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long PurchaseOrderInvoiceId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Orden de compra")]
        public long PurchaseOrderId { get; set; }

        [AuditField("Factura proveedor")]
        public long SupplierInvoiceId { get; set; }

        [AuditField("Observacion")]
        public string Observation { get; set; } = string.Empty;

        [AuditField("Estado")]
        public new string Status { get; set; } = "1";
    }
}

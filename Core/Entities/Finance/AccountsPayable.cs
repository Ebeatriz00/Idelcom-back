using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Finance
{
    public class AccountsPayable : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long AccountPayableId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Factura Proveedor")]
        public long SupplierInvoiceId { get; set; }

        [AuditField("Proveedor")]
        public long SuppliersId { get; set; }

        [AuditField("Fecha emisión")]
        public DateTime IssueDate { get; set; }

        [AuditField("Fecha vencimiento")]
        public DateTime? DueDate { get; set; }

        [AuditField("Moneda")]
        public long CurrencyId { get; set; }

        [AuditField("Tipo de cambio")]
        public decimal? ExchangeRate { get; set; }

        [AuditField("Monto Original")]
        public decimal OriginalAmount { get; set; }

        [AuditField("Monto Pagado")]
        public decimal PaidAmount { get; set; }

        [AuditField("Monto de detraccion")]
        public decimal? DetractionAmount { get; set; }

        [AuditField("Monton de retencion")]
        public decimal? RetentionAmount { get; set; }

        [AuditField("Monto de percepcion")]
        public decimal? PerceptionAmount { get; set; }

        [AuditField("Estados pagos")]
        public long PayableStatusId { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }
    }
}

using Core.Attributes;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Finance
{
    public class SupplierInvoice : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long SupplierInvoiceId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Proveedor")]
        public long SuppliersId { get; set; }

        [AuditField("Recepcion de compra")]
        public long? PurchaseReceiptId { get; set; }

        [AuditField("Tipo de documento")]
        public long DocumentTypeId { get; set; }

        [AuditField("Series")]
        public string Series { get; set; } = null!;

        [AuditField("Correlativo")]
        public string Correlative { get; set; } = null!;

        [AuditField("Fecha de asunto")]
        public DateTime IssueDate { get; set; }

        [AuditField("Fecha de vencimiento")]
        public DateTime? DueDate { get; set; }

        [AuditField("Moneda")]
        public long CurrencyId { get; set; }

        [AuditField("Tipo de cambio")]
        public decimal? ExchangeRate { get; set; }

        [AuditField("SubTotal")]
        public decimal Subtotal { get; set; }

        [AuditField("Importe de descuento")]
        public decimal DiscountAmount { get; set; }

        [AuditField("Monto de impusto")]
        public decimal TaxAmount { get; set; }

        [AuditField("Total")]
        public decimal Total { get; set; }

        [AuditField("Aplica detracciones")]
        public bool DetractionApplies { get; set; }

        [AuditField("Detracción porcentaje")]
        public decimal? DetractionPercent { get; set; }

        [AuditField("Total detracción")]
        public decimal? DetractionAmount { get; set; }

        [AuditField("Aplica percepción")]
        public bool PerceptionApplies { get; set; }

        [AuditField("Total percepcion")]
        public decimal? PerceptionAmount { get; set; }

        [AuditField("Aplica retención")]
        public bool RetentionApplies { get; set; }

        [AuditField("Total retención")]
        public decimal? RetentionAmount { get; set; }

        [AuditField("Estado sunat")]
        public long? SunatStatusId { get; set; }

        [AuditField("Estado factura")]
        public long InvoiceStatusId { get; set; }

        [AuditField("Documento XML")]
        public string? XmlPath { get; set; }

        [AuditField("Documento CDR")]
        public string? CdrPath { get; set; }

        [AuditField("Documento PDF")]
        public string? PdfPath { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }

        [AuditField("Activo")]
        public bool IsActive { get; set; }
    }
}

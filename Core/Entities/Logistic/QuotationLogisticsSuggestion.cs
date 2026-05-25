using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Logistic
{
    public class QuotationLogisticsSuggestion : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long QuotationLogisticsSuggestionId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Cotizacion")]
        public long QuotationId { get; set; }

        [AuditField("Version de cotizacion")]
        public long QuotationVerId { get; set; }

        [AuditField("Linea de cotizacion")]
        public long? QuotationVerLinId { get; set; }

        [AuditField("Regla de sugerencia")]
        public long? LogisticsSuggestionRuleId { get; set; }

        [AuditField("Tipo de recurso logistico")]
        public long LogisticsResourceTypeId { get; set; }

        [AuditField("Producto")]
        public long? ProductsId { get; set; }

        [AuditField("Descripcion")]
        public string Description { get; set; } = string.Empty;

        [AuditField("Cantidad sugerida")]
        public decimal SuggestedQuantity { get; set; }

        [AuditField("Cantidad aprobada")]
        public decimal? ApprovedQuantity { get; set; }

        [AuditField("Seleccionado")]
        public bool IsSelected { get; set; }

        [AuditField("Manual")]
        public bool IsManual { get; set; }

        [AuditField("Duplicado")]
        public bool IsDuplicated { get; set; }

        [AuditField("Motivo de sugerencia")]
        public string? SuggestionReason { get; set; }

        [AuditField("Observacion de oficina")]
        public string? OfficeObservation { get; set; }

        [AuditField("Revisado por")]
        public long? ReviewedBy { get; set; }

        [AuditField("Fecha de revision")]
        public DateTime? ReviewedDate { get; set; }
    }
}

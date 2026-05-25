using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Logistic
{
    public class LogisticsSuggestionRule : BaseAuditableEntity
    {
        [AuditField("Id")]
        public long LogisticsSuggestionRuleId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Nombre de regla")]
        public string RuleName { get; set; } = string.Empty;

        [AuditField("Keyword")]
        public string? Keyword { get; set; }

        [AuditField("Tipo de producto")]
        public long? ProductsTypeId { get; set; }

        [AuditField("Sistema")]
        public long? SystemId { get; set; }

        [AuditField("Tipo de linea")]
        public string? LineType { get; set; }

        [AuditField("Tipo de recurso logistico")]
        public long LogisticsResourceTypeId { get; set; }

        [AuditField("Producto sugerido")]
        public long? SuggestedProductsId { get; set; }

        [AuditField("Descripcion sugerida")]
        public string SuggestedDescription { get; set; } = string.Empty;

        [AuditField("Cantidad por defecto")]
        public decimal DefaultQuantity { get; set; }

        [AuditField("Factor de cantidad")]
        public decimal QuantityFactor { get; set; }

        [AuditField("Obligatorio")]
        public bool IsRequired { get; set; }

        [AuditField("Requiere revision")]
        public bool RequiresReview { get; set; }

        [AuditField("Observacion")]
        public string? Observation { get; set; }
    }
}

using Core.Attributes;
using SharedKernel;

namespace Core.Entities.Operations
{
    public class ActivityComplexity : BaseAuditableEntity
    {
        [AuditField("ComplexityId")]
        public int ComplexityId { get; set; }

        [AuditField("Empresa")]
        public long BusinessId { get; set; }

        [AuditField("Nombre de complejidad")]
        public required string ComplexityName { get; set; }

        [AuditField("Factor de peso")]
        public decimal WeightFactor { get; set; }
    }
}

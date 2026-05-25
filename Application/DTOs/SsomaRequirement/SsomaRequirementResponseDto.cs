using SharedKernel;

namespace Application.DTOs.SsomaRequirement
{
    public class SsomaRequirementResponseDto : BaseAuditableEntity
    {
        public long RequirementId { get; set; }
        public long BusinessId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ScopeId { get; set; }
        public string ScopeName { get; set; } = null!;
        public bool HasExpiration { get; set; }
        public bool RequiresFile { get; set; }
        public bool RequiresExpiration { get; set; }
        public int MaxFileSize { get; set; }
        public string AllowedExtensions { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}

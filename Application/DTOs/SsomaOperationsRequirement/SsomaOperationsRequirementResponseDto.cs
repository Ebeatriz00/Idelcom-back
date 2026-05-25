namespace Application.DTOs.SsomaOperationsRequirement
{
    public class SsomaOperationsRequirementResponseDto
    {
        public long OperationsRequirementId { get; set; }
        public int RequirementId { get; set; }
        public string RequirementName { get; set; } = null!;
        public string RequirementDescription { get; set; } = null!;
        public bool IsMandatory { get; set; }
        public bool RequiresFile { get; set; }
        public bool RequiresExpiration { get; set; }
        public int MaxFileSize { get; set; }
        public string AllowedExtensions { get; set; } = null!;
    }
}

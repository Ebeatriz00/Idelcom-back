
namespace Application.DTOs.SsomaRequirement
{
    public class SsomaRequirementItemDto
    {
        public string RequirementId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? Duration { get; set; }
        public string ScopeName { get; set; } = null!;
        public string HasExpiration { get; set; } = null!;
        public string RequiresFile { get; set; } = null!;
        public string MaxFileSize { get; set; } = null!;
        public string AllowedExtensions { get; set; } = null!;
        public string AllowInternalReuse { get; set; } = null!;
        public string IsActive { get; set; } = null!;
    }

}

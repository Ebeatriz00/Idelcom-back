namespace Application.DTOs.SsomaRequirement
{
    public class SsomaRequirementCreateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? Duration { get; set; }
        public int ScopeId { get; set; }
        public bool HasExpiration { get; set; }
        public bool RequiresFile { get; set; }
        public bool RequiresExpiration { get; set; }
        public int MaxFileSize { get; set; }
        public string AllowedExtensions { get; set; } = null!;
        public bool AllowInternalReuse { get; set; }
    }
}

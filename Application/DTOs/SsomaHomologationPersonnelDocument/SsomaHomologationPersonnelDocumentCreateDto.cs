using Core.Attributes;
using System.Text.Json.Serialization;

namespace Application.DTOs.SsomaHomologationPersonnelDocument
{
    public class SsomaHomologationPersonnelDocumentCreateDto
    {
        [JsonIgnore]
        public long HomologationPersonnelId { get; set; }

        public int RequirementId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        [JsonIgnore]
        public int ValidationStatusId { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string Observation { get; set; } = null!;

        [JsonIgnore]
        public long? ReplacedDocumentId { get; set; }
        [JsonIgnore]
        public int? DocumentVersion { get; set; }
        [JsonIgnore]
        public string? ReplacementReason { get; set; }
    }
}

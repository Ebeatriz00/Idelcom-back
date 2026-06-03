using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.DTOs.SsomaHomologationPersonnelDocument
{
    public class SsomaHomologationPersonnelDocumentUpdateDto
    {
        public long SsomaHomologationPersonnelDocumentId { get; set; }
        public long HomologationPersonnelId { get; set; }
        public int RequirementId { get; set; }
        public IFormFile? File { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FilePath { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ValidationStatusId { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? Observation { get; set; }
        [JsonIgnore]
        public long? ReplacedDocumentId { get; set; }
        [JsonIgnore]
        public int? DocumentVersion { get; set; }
        [JsonIgnore]
        public string? ReplacementReason { get; set; }
    }
}

namespace Application.DTOs.SsomaHomologationPersonnelDocument
{
    public class SsomaHomologationPersonnelDocumentResponseDto
    {
        public long SsomaHomologationPersonnelDocumentId { get; set; }
        public long BusinessId { get; set; }
        public long HomologationPersonnelId { get; set; }
        public int RequirementId { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FilePath { get; set; }
        public Guid? FileUid { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ValidationStatusId { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Observation { get; set; } = null!;
        public long? ClinicId { get; set; }
    }
}

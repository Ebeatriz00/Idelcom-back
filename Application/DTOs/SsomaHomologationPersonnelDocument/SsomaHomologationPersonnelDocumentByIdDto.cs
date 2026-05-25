using SharedKernel;

namespace Application.DTOs.SsomaHomologationPersonnelDocument
{
    public class SsomaHomologationPersonnelDocumentByIdDto : BaseAuditableEntity
    {
        public long SsomaHomologationPersonnelDocumentId { get; set; }
        public long BusinessId { get; set; }
        public long HomologationPersonnelId { get; set; }
        public int RequirementId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ValidationStatusId { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Observation { get; set; } = null!;
    }
}

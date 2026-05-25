using SharedKernel;

namespace Application.DTOs.SsomaHomologationPersonnel
{
    public class SsomaHomologationPersonnelByIdDto : BaseAuditableEntity
    {
        public long HomologationPersonnelId { get; set; }
        public long BusinessId { get; set; }
        public long HomologationScopeId { get; set; }
        public long? OperationsId { get; set; }
        public long WorkerId { get; set; }
        public long WorkerStatusId { get; set; }
        public long MedicalAptitudeId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool SsomaApproved { get; set; }
        public string Notes { get; set; } = null!;
    }
}

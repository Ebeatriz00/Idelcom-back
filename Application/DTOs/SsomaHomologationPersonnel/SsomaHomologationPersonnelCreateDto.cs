using System.Text.Json.Serialization;

namespace Application.DTOs.SsomaHomologationPersonnel
{
    public class SsomaHomologationPersonnelCreateDto
    {
        public long HomologationScopeId { get; set; }
        public long? OperationsId { get; set; }
        public long WorkerId { get; set; }
        [JsonIgnore]
        public long WorkerStatusId { get; set; }
        public long MedicalAptitudeId { get; set; }
        public DateTime? ValidFrom { get; set; }
        [JsonIgnore]
        public DateTime? ValidTo { get; set; }
        public bool SsomaApproved { get; set; }
        public string Notes { get; set; } = null!;
    }
}

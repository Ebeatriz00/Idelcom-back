namespace Application.DTOs.SsomaOperationsRequirement
{
    public class ValidateSsomaOperationsRequirementByWorkerResponseDto
    {
        public int? IsReady { get; set; }
        public int? TotalMandatory { get; set; }
        public int? TotalCoveredMandatory { get; set; }
        public int? TotalMissingMandatory { get; set; }
    }
}

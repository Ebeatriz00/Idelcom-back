namespace Application.DTOs.SsomaOperationsRequirement
{
    public class SsomaOperationsRequirementUpdateDto
    {
        public long SsomaOperationsRequirementId { get; set; }
        public long OperationsId { get; set; }
        public int RequirementId { get; set; }
        public bool IsMandatory { get; set; }
        public int? ValidDays { get; set; }
    }
}

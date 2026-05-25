using SharedKernel;

namespace Application.DTOs.OperationsTeamSsoma
{
    public class OperationsTeamSsomaGetByIdDto : BaseAuditableEntity
    {
        public long? OperationsTeamSsomaId { get; set; }
        public long BusinessId { get; set; }
        public long SsomaProcessId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public List<OperationsTeamSsomaAssignmentDto> TeamSsoma { get; set; } = new();
        public int? AssignmentId { get; set; }
        public bool IsActive { get; set; }
        public string? ReasonChange { get; set; }
        public long? ReplacedAssignmentId { get; set; }
        public int? ClientApprovalStatusId { get; set; }
        public DateOnly? ClientApprovalDate { get; set; }
        public string? Comments { get; set; }
    }
}

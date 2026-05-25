using Core.Attributes;

namespace Core.Projections.Operations
{
    public class OperationsTeamSsomaDetailProjection
    {
        public long? OperationsTeamSsomaId { get; set; }

        public long BusinessId { get; set; }

        public long SsomaProcessId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public int? AssignmentId { get; set; }

        public bool IsActive { get; set; }
        public string? ReasonChange { get; set; }

        public long? ReplacedAssignmentId { get; set; }

        public int? ClientApprovalStatusId { get; set; }

        public DateTime? ClientApprovalDate { get; set; }

        public string? Comments { get; set; }

        public List<OperationsTeamSsomaAssignmentItem> TeamSsoma { get; set; } = new();
    }
}

using System;

namespace Core.Projections.Operations
{
    public class OperationsTeamSsomaListItemProjection
    {
        public long OperationsTeamSsomaId { get; set; }
        public long SsomaProcessId { get; set; }
        public int? AssignmentId { get; set; }
        public long? WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public int SsomaRoleId { get; set; }
        public string? SsomaRoleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public long? ReplacedAssignmentId { get; set; }
        public int? ClientApprovalStatusId { get; set; }
        public string? ClientApprovalStatusName { get; set; }
        public string? Comments { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}

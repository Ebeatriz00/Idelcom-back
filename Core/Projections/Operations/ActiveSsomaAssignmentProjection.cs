using System;

namespace Core.Projections.Operations
{
    public class ActiveSsomaAssignmentProjection
    {
        public long OperationsTeamSsomaId { get; set; }
        public long SsomaProcessId { get; set; }
        public long OperationsId { get; set; }
        public long OpporId { get; set; }
        public string? OpportunityName { get; set; }
        public long WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public int SsomaRoleId { get; set; }
        public string? SsomaRoleName { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}

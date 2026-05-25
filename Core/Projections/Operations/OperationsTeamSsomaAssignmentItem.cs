using Core.Attributes;

namespace Core.Projections.Operations
{
    public class OperationsTeamSsomaAssignmentItem
    {
        public long? OperationsTeamSsomaId { get; set; }
        public long? WorkerId { get; set; }
        public int SsomaRoleId { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public bool IsPrimary { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}

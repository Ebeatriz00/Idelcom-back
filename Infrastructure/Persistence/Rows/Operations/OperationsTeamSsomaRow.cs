
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Rows.Operations
{
    public class OperationsTeamSsomaRow
    {
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

        public long? OperationsTeamSsomaId { get; set; }
        public long WorkerId { get; set; }
        public int SsomaRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int IsPrimary { get; set; }
    }

    public class OperationsTeamSsomaInsertedRow
    {
        public long OperationsTeamSsomaId { get; set; }
        public long WorkerId { get; set; }
        public int SsomaRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPrimary { get; set; }
    }
}

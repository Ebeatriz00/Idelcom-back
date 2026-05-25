using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contexts.OperationsTeamSsoma
{
    public class OperationsTeamSsomaAuditSnapshot
    {
        public long BusinessId { get; set; }
        public long SsomaProcessId { get; set; }
        public int? AssignmentId { get; set; }
        public bool IsActive { get; set; }
        public string? ReasonChange { get; set; }
        public long? ReplacedAssignmentId { get; set; }
        public int? ClientApprovalStatusId { get; set; }
        public DateTime? ClientApprovalDate { get; set; }
        public string? Comments { get; set; }
    }
}

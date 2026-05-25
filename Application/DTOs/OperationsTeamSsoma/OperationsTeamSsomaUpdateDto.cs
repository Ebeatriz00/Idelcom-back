using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsTeamSsoma
{
    public class OperationsTeamSsomaUpdateDto
    {
        public long BusinessId { get; set; }
        public long SsomaProcessId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public List<OperationsTeamSsomaAssignmentUpdateDto> TeamSsoma { get; set; } = new();
        public long? UpdateUser { get; set; }
    }
    public class OperationsTeamSsomaAssignmentUpdateDto
    {
        public long OperationsTeamSsomaId { get; set; }
        public long? WorkerId { get; set; }
        public int SsomaRoleId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsPrimary { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}

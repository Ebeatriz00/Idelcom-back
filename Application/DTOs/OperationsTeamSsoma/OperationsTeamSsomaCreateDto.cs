using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OperationsTeamSsoma
{
    public class OperationsTeamSsomaCreateDto
    {
        public long BusinessId { get; set; }
        public long SsomaProcessId { get; set; }
        public long? OperationsProjectConfigId { get; set; }
        public List<OperationsTeamSsomaAssignmentDto> TeamSsoma { get; set; } = new();
        public long CreateUser { get; set; }
    }
    public class OperationsTeamSsomaAssignmentDto
    {
        public long? WorkerId { get; set; }
        public int SsomaRoleId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsPrimary { get; set; }
        public long? OperationsProjectConfigId { get; set; }
    }
}

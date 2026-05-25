using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Projections.Ssoma
{
    public class SsomaProcessListItem
    {
        public long SsomaProcessId { get; set; }
        public long BusinessId { get; set; }
        public string? OperationsName { get; set; }
        public bool RequiresCompanyHomologation { get; set; }
        public bool RequieresOperationTeamSsoma { get; set; }
        public string? CurrentDesc { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}

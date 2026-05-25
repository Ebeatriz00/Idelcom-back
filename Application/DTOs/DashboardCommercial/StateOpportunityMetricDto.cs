using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Dashboard
{
    public class StateOpportunityMetricDto
    {
        public long StateOpportunityId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateColor { get; set; } = string.Empty;
        public long Quantity { get; set; }
    }
}

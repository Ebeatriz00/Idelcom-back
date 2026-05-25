using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StateOpportunityMetric
    {
        public long StateOpportunityId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateColor { get; set; } = string.Empty;
        public long Quantity { get; set; }
    }


    public class ClientMetric
    {
        public int Quantity { get; set; }
    }

    public class QuarterMetric
    {
        public int QuarterNum { get; set; }
        public int Quantity { get; set; }
    }

    public class CombinedMetric
    {
        public int QuarterNum { get; set; }
        public string StateName { get; set; }
        public string StateColor { get; set; }
        public int Quantity { get; set; }
    }
}

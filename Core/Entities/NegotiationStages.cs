using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class NegotiationStages
    {
        public long NegotiationStagesId { get; set; }
        public long BusinessId { get; set; }
        public string NegitiationDesc { get; set; }
        public string status { get; set; }
    }

}

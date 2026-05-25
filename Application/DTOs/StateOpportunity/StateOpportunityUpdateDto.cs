using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StateOpportunity
{
    public class StateOpportunityUpdateDto
    {
        public long StateOpportunityId { get; set; }
        public long BusinessId { get; set; }
        public string StateColor { get; set; }
        public string StateDesc { get; set; }
        public int NumPercPro { get; set; }
        public int NumOrder { get; set; }
        public long UsersBy { get; set; }
    }
}

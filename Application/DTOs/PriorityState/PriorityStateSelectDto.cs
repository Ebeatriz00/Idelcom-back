using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PriorityState
{
    public class PriorityStateSelectDto
    {
        public string LinkToken { get; set; }
        public int PriorityStateId { get; set; }
        public string PriorityDesc { get; set; }
        public string Color { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StateTask
{
    public class StateTaskSelectDto
    {
        public string LineToken { get; set; }
        public long StateTaskId { get; set; }
        public string StateColor { get; set; }
        public string StateDesc { get; set; }
        public int NumPercPro { get; set; }
    }
}

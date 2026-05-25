using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StateTask
{
    public class StateTaskByIdDto
    {
        public long StateTaskId { get; set; }
        public long BusinessId { get; set; }
        public string StateColor { get; set; }
        public string StateDesc { get; set; }
        public int NumPercPro { get; set; }
        public int NumOrder { get; set; }
    }
}

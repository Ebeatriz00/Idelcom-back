using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StateTask
    {
        public long StateTaskId { get; set; }
        public string LineToken { get; set; }
        public long BusinessId { get; set; }
        public string StateColor { get; set; }
        public string StateDesc { get; set; }
        public int NumPercPro { get; set; }
        public int NumOrder { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";

    }
}

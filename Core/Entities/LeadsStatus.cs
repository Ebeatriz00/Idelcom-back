using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class LeadsStatus
    {
        public long LeadsStatusId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int LeadsStatusCount { get; set; }
        public long UsersBy { get; set; }
    }
}

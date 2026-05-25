using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class WorkerStatus
    {
        public long WorkerStatusId { get; set; }
        public long BusinessId { get; set; }
        public string WorkerStatusDesc { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}

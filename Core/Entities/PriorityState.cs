using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PriorityState
    {
        public string LinkToken { get; set; } = default!;
        public long PriorityStateId { get; set; }
        public long BusinessId { get; set; }
        public string PriorityDesc { get; set; }
        public string Color { get; set; }
        public long UsersBy {  get; set; }
        public string Status { get; set; } = "1";
    }
}

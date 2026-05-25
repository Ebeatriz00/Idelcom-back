using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PMCondition
    {
        public long PMConditionId { get; set; }
        public long BussinessId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "1";
        public long UsersBy {  get; set; }
        public int PmConditionCount { get; set; }
    }
}

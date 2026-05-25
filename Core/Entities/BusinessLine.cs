using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class BusinessLine
    {
        public int BusinessLineId { get; set; }
        public long BusinessId { get; set; }
        public string DescLine { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}

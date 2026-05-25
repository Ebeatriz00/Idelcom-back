using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MovOper
    {
        public long MovOperId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "1";
        public long UsersBy { get; set; }
        public int MovOperCount { get; set; }
    }
}

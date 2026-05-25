using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Area
    {
        public long AreaId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UsersBy { get; set; }
        public int AreaCount { get; set; }
        public string Status { get; set; } = "1";
    }
}

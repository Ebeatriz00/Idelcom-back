using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Sector
    {
        public long SectorId { get; set; }
        public long BusinessId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int SectorCount { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
    }
}

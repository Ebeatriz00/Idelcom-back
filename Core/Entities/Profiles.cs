using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Profiles
    {
        public long ProfilesId { get; set; }
        public long BusinessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public long UsersCount { get; set; }
        public int UsersBy { get; set; }
    }
}

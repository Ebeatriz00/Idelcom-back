using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ActivityType
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string? ActivityIcon { get; set; }
        public string? ActivityDesc { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; }
    }
}

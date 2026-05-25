using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Activity
{
    public class ActivityPriorityOpporDto
    {
        public long BusinessId { get; set; }
        public string LinkToken { get; set; } = default!;
        public long UsersBy { get; set; }
        public string Status { get; set; }
    }
}

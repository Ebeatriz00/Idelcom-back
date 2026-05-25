using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActivityState
{
    public class ActivityStateStatusToggleDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; }
    }
}

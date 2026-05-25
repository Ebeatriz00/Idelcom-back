using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActivityState
{
    public class ActivityStateCreateDto
    {
        public long BusinessId { get; set; }
        public string? StateIcon { get; set; }
        public string? StateColor { get; set; }
        public string? StateDesc { get; set; }
        public int? OrderState { get; set; }
        public long UsersBy { get; set; }

    }
}

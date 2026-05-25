using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActivityState
{
    public class ActivityStateSelectDto
    {
        public string LinkToken { get; set; } = default!;
        public string? StateIcon { get; set; }
        public string? StateColor { get; set; }
        public string? StateDesc { get; set; }
    }
}

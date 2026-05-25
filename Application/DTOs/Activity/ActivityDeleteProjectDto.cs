using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Activity
{
    public class ActivityDeleteProjectDto
    {
        public string LinkToken { get; set; } = default!;
        public string ProjectToken { get; set; } = default!;
    }
}

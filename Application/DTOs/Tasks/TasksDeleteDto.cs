using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tasks
{
    public class TaksOpporDeleteDto
    {
        public string LinkToken { get; set; } = default!;
        public string OpporToken { get; set; } = default!;
    }


    public class TasksProjectDeleteDto
    {
        public string LinkToken {  set; get; } = default!;
        public string ProjectToken{ get; set; } = default!;
    }
}

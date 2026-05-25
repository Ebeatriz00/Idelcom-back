using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tasks
{
    public class TasksSelectDto
    {
        public long TasksId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tasks
{
    public class TasksCompletedDto
    {
        public string linkToken { get; set; }
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }
    }
}

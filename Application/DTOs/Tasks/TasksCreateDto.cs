using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tasks
{
    public class TasksCreateDto
    {
        public long BusinessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly? EndDate { get; set; }
        public int PriorityStateId { get; set; }
        public TimeOnly? Time { get; set; }
        public string? ProjectToken { get; set; }
        public string? OpporToken { get; set; }
        public long StateTaskId { get; set; }
        public long WorkerId { get; set; }
        public long UsersBy { get; set; }
    }
}

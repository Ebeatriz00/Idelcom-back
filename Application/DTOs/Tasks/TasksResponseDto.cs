using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tasks
{
    public class TasksResponseDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly? EndDate { get; set; }
        public TimeOnly? Time { get; set; }
        public string Status { get; set; } = "1";
        public string? OpporDescription { get; set; } = string.Empty;
        public string? StateTaskDescription { get; set; } = string.Empty;
        public string WprkerDescription { get; set; } = string.Empty;
        public string PriorityStateDescription {  get; set; } = string.Empty;
        public int TaskCount { get; set; }
    }
}

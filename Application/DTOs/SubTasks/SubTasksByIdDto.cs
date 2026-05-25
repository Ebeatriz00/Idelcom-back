using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubTasks
{
    public class SubTasksByIdDto
    {
        public long SubTasksId { get; set; }
        public long TasksId { get; set; }
        public string LinkToken { get; set; }
        public long BusinessId { get; set; }
        public long? WorkerId { get; set; }
        public long? AreaId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly? EndDate { get; set; }
        public TimeOnly? Time { get; set; }
        public long? StateTaskId { get; set; }
        public int? PriorityStateId { get; set; }
        public string Status { get; set; }
    }
}

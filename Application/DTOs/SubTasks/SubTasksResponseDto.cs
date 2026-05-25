using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubTasks
{
    public class SubTasksResponseDto
    {
        public string LinkToken { get; set; }
        public string TaskToken { get; set; }
        public long BusinessId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly? Time { get; set; }
        public string Status { get; set; }

        public string WorkerDescription { get; set; }
        public string AreaDescription { get; set; }
        public string StateTaskDescription { get; set; }
        public string PriorityStateDescription { get; set; }

        public long? WorkerId { get; set; }
        public long? AreaId { get; set; }
        public long? StateTaskId { get; set; }
        public int? PriorityStateId { get; set; }
    }
}

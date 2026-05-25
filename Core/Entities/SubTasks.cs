using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SubTasks
    {
        public long SubTasksId { get; set; }
        public long TasksId { get; set; }
        public string TaskToken { get; set; }
        public string LinkToken { get; set; }
        public long BusinessId { get; set; }
        public long? WorkerId { get; set; }
        public long? AreaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly? Time { get; set; }
        public long? StateTaskId { get; set; }
        public int? PriorityStateId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";

        public string WorkerDescription { get; set; }
        public string AreaDescription { get; set; }
        public string StateTaskDescription { get; set; }
        public string PriorityStateDescription { get; set; }
    }
}

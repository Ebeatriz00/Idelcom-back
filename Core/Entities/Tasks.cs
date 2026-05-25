using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Tasks
    {
        public long TasksId { get; set; }
        public string linkToken { get; set; }
        public long BusinessId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly? EndDate { get; set; }
        public int PriorityStateId { get; set; }
        public long DeliverableId { get; set; }
        public string Priority { get; set; }
        public TimeOnly? Time { get; set; }
        public long? OpporId { get; set; }
        public long? ProjectId { get; set; }
        public string OpporToken { get; set; }
        public string ProjectToken { get; set; }
        public long StateTaskId { get; set; }
        public long WorkerId { get; set; }
        public long UsersBy {  get; set; }
        public string Status { get; set; } = "1";
        public int TaskCount { get; set; }  
        public string ProjectDescription { get; set; }
        public string OpporDescription { get; set; }
        public string StateTaskDescription { get; set; }
        public string WprkerDescription { get; set; }
        public string PriorityStateDescription { get; set; }
    }
}

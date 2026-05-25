using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubTasks
{
    using System;

    namespace Application.DTOs.SubTasks
    {
        public class SubTasksUpdateDto
        {
            public string LinkToken { get; set; }
            public long BusinessId { get; set; }
            public long? WorkerId { get; set; }

            public string Title { get; set; }
            public string Description { get; set; }
            public DateOnly? EndDate { get; set; }
            public TimeOnly? Time { get; set; }
            public long? StateTaskId { get; set; }
            public int? PriorityStateId { get; set; }
            public long UsersBy { get; set; }
        }
    }
}

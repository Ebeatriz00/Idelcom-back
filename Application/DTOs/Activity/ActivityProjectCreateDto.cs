using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Activity
{
    public class ActivityProjectCreateDto
    {
        public long BusinessId { get; set; }
        public string ProjectToken { get; set; } = default!;
        public long WorkerOwnerId { get; set; }
        public long WorkerSenderId { get; set; }
        public int ActivityState { get; set; }
        public int ActivityType { get; set; }
        public int ActivityPriority { get; set; }
        public string? ActivityMessage { get; set; }
        public string? MessageAddition { get; set; }
        public DateTime? MessageDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public long UsersBy { get; set; }
    }
}

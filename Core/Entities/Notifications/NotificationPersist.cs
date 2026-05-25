using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Notifications
{
    public class NotificationPersist
    {
        public long NotificationId { get; init; }
        public long BusinessId { get; init; }
        public long UsersId { get; init; }
        public string Title { get; init; } = "";
        public string Message { get; init; } = "";
        public string Module { get; init; } = "";
        public string Entity { get; init; } = "";
        public string EntityId { get; init; }
        public string LinkUrl { get; init; } = "";
        public string Type { get; init; } = "";
        public DateTime CreatedAt { get; init; }
        public DateTime? ReadAt { get; init; }
        public long CreatedBy { get; init; }
        public string CreatedByName { get; init; }
        public DateTime? FinishDate { get; init; }
    }
}

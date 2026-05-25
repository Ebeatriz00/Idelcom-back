using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Notifications
{
    public class NotificationsMarkAllDto
    {

        public long BusinessId { get; init; }
        public long UsersId { get; init; }
    }
    public class NotificationsMarkDto
    {
        public long NotificationId { get; init; }
        public long BusinessId { get; init; }
        public long UsersId { get; init; }
    }
}

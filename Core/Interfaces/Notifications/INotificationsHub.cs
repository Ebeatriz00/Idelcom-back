using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public interface INotificationsHub
    {
        Task Notify(object notification);
        Task UserNotification(object notification);
        Task BusinessNotification(object notification);
    }
}

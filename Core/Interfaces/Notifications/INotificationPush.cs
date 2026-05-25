using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public interface INotificationPush
    {
        Task PushToUsersAsync(IEnumerable<long> userIds, object payload, CancellationToken ct);
        Task PushToBusinessAsync(long businessId, object payload, CancellationToken ct);
    }
}

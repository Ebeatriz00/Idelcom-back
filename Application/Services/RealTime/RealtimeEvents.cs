using Core.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.RealTime
{
    public static class RealtimeEvents
    {
        public static Task InvalidateBusinessAsync(
            INotificationPush push,
            long businessId,
            params string[] keys)
        {
            return push.PushToBusinessAsync(
                businessId,
                new
                {
                    code = "CACHE_INVALIDATE",
                    keys,
                    createdAt = DateTime.UtcNow
                },
                CancellationToken.None
            );
        }
    }

}

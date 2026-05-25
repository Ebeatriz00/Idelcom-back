using Core.Interfaces.Notifications;
using Infrastructure.Hubs; 
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public sealed class SignalRNotificationPush : INotificationPush
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public SignalRNotificationPush(IHubContext<NotificationsHub> hub) => _hub = hub;

        public Task PushToUsersAsync(IEnumerable<long> userIds, object payload, CancellationToken ct)
            => Task.WhenAll(userIds.Select(id =>
                _hub.Clients.User(id.ToString()).SendAsync("notify", payload, ct)));

        public Task PushToBusinessAsync(long businessId, object payload, CancellationToken ct)
            => _hub.Clients.Group($"biz:{businessId}").SendAsync("notify", payload, ct);
    }
}
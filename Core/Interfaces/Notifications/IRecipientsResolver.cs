using Core.Entities.Notifications;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public interface IRecipientsResolver
    {
        Task AddManyAsync(IEnumerable<NotificationPersist> items, CancellationToken ct);
        Task<PagedResult<NotificationPersist>> ListForUserAsync(long businessId, long userId, string? search, int page, int pageSize, CancellationToken ct);
        Task<IReadOnlyList<long>> ResolveAsync(NotificationEvent ev, CancellationToken ct);
        Task MarkAllRead(long businessId, long usersId, CancellationToken ct);
        Task MarkRead(long businessId, long usersId, long notificationId, CancellationToken ct);
        Task AlertResolve(long businessId, long usersId, long notificationId, CancellationToken ct);
        Task AlertSnooze(long businessId, long usersId, long notificationId, DateTime snoozeUntil, string comment, CancellationToken ct);
    }
}

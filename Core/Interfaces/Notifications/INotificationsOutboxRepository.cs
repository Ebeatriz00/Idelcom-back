using Core.Entities.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public interface INotificationsOutboxRepository
    {
        Task EnqueueAsync(NotificationPersist item, CancellationToken ct);

        Task<IReadOnlyList<NotificationOutboxRow>> GetPendingAsync(int top, CancellationToken ct);
        Task MarkProcessedAsync(IEnumerable<long> outboxIds, CancellationToken ct);
    }
}

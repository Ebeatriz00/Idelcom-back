using Core.Entities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public interface IEmailOutboxRepository
    {
        Task<List<EmailOutboxItem>> GetPendingBatchAsync(int take, CancellationToken ct);
        Task<bool> MarkProcessingAsync(long outboxId, CancellationToken ct);
        Task MarkSentAsync(long outboxId, CancellationToken ct);
        Task MarkFailedAsync(long outboxId, string error, int nextMinutes, CancellationToken ct);

    }
}

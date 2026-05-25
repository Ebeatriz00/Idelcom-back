using Core.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Notifications
{
    public class AlertSnooze
    {
        private readonly IRecipientsResolver _resolver;

        public AlertSnooze(IRecipientsResolver resolver)
        {
            _resolver = resolver;
        }
        public async Task HandleAsync(long businessId, long usersId, long notificationId, DateTime snoozeUntil, string comment, CancellationToken ct)
        {
            await _resolver.AlertSnooze(businessId, usersId, notificationId, snoozeUntil, comment, ct);
        }
    }
}

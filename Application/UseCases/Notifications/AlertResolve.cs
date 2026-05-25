using Core.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Notifications
{
    public class AlertResolve
    {
        private readonly IRecipientsResolver _resolver;

        public AlertResolve(IRecipientsResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task HandleAsync(long businessId, long usersId, long notificationId, CancellationToken ct)
        {
            await _resolver.AlertResolve(businessId, usersId, notificationId,ct);
        }
    }
}

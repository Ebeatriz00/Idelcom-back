using Core.Entities.Notifications;
using Core.Entities.paginations;
using Core.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Notifications
{
    public class ListNotifications
    {
        private readonly IRecipientsResolver _resolver;

        public ListNotifications(IRecipientsResolver resolver)
        {
            _resolver = resolver;
        }

        public Task<PagedResult<NotificationPersist>> HandleAsync(
            long businessId,
            long userId,
           string? search, int page, int pageSize,
            CancellationToken ct)
        {
            return _resolver.ListForUserAsync(businessId, userId, search, page, pageSize, ct);
        }
    }
}

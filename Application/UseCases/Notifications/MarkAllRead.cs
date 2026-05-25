using Application.DTOs.Notifications;
using AutoMapper;
using Core.Entities.Notifications;
using Core.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Notifications
{
    public class MarkAllRead
    {
        private readonly IRecipientsResolver _resolver;
        public MarkAllRead(IRecipientsResolver resolver)
        {
            _resolver = resolver;
        }
        public async Task HandleAsync(NotificationsMarkAllDto notificationPersist, CancellationToken ct)
        {

            
            await _resolver.MarkAllRead(notificationPersist.BusinessId, notificationPersist.UsersId, ct);
        }
    }
}

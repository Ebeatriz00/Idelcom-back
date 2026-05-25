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
    public class MarkRead
    {
        private readonly IRecipientsResolver _resolver;
        public MarkRead(IRecipientsResolver resolver)
        {
            _resolver = resolver;
            
        }
        public async Task HandleAsync(NotificationsMarkDto notificationPersist, CancellationToken ct)
        {
            await _resolver.MarkRead(notificationPersist.BusinessId, notificationPersist.UsersId, notificationPersist.NotificationId, ct);
        }
    }
}

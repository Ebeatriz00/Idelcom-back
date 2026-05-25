using Core.Entities.Notifications;
using Core.Interfaces.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.Notifications
{
    public sealed class PushNotificationUseCase
    {
        private readonly IRecipientsResolver _resolver;
        private readonly INotificationPush _push;
        private readonly INotificationsOutboxRepository _outbox;

        public PushNotificationUseCase(
            IRecipientsResolver resolver,
            INotificationPush push,
            INotificationsOutboxRepository outbox)
        {
            _resolver = resolver;
            _push = push;
            _outbox = outbox;
        }

        public async Task HandleAsync(NotificationEvent ev, CancellationToken ct)
        {
            var recipients = await _resolver.ResolveAsync(ev, ct);
            if (recipients.Count == 0) return;

            var persistItems = ev switch
            {
                NewComment nc => recipients.Select(uid => new NotificationPersist
                {
                    NotificationId = 0,                
                    BusinessId = nc.BusinessId,
                    UsersId = uid,
                    Title = "Nuevo comentario",
                    Message = nc.CommentPreview ?? "Se agregó un comentario en la oportunidad.",
                    Module = "CRM",
                    Entity = "OPPORTUNITY",
                    EntityId = nc.OpporId,
                    LinkUrl = $"/crm/opportunity/detail/{nc.OpporId}",
                    Type = nc.Type,
                    CreatedBy = nc.CreatedBy,
                    CreatedByName = "Usuario de prueba",
                    CreatedAt = DateTime.UtcNow,
                    ReadAt = null
                }),

                OpportunityStateChanged sc => recipients.Select(uid => new NotificationPersist
                {
                    NotificationId = 0,
                    BusinessId = sc.BusinessId,
                    UsersId = uid,
                    Title = "Oportunidad actualizada",
                    Message = $"La oportunidad cambió a estado: {sc.NewState}",
                    Module = "CRM",
                    Entity = "OPPORTUNITY",
                    EntityId = sc.OpporId,
                    LinkUrl = $"/crm/opportunity/detail/{sc.OpporId}",
                    Type = sc.Type,
                    CreatedBy = sc.ChangedBy,          // acá es ChangedBy
                    CreatedAt = DateTime.UtcNow,
                    ReadAt = null
                }),

                _ => Enumerable.Empty<NotificationPersist>()
            };
            var itemsList = persistItems.ToList();
            if (itemsList.Count == 0) return;

            await _resolver.AddManyAsync(itemsList, ct);

            foreach (var item in itemsList)
            {
                await _outbox.EnqueueAsync(item, ct);
            }

            await _push.PushToUsersAsync(recipients, itemsList, ct);
        }
    }
}
using Core.Entities.Notifications;
using Core.Interfaces.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Notifications
{
    public sealed class NotificationsOutboxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationsOutboxWorker> _logger;

        private const int BatchSize = 50;

        public NotificationsOutboxWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationsOutboxWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var outbox = scope.ServiceProvider.GetRequiredService<INotificationsOutboxRepository>();
                    var history = scope.ServiceProvider.GetRequiredService<IRecipientsResolver>();
                    var push = scope.ServiceProvider.GetRequiredService<INotificationPush>();

                    var pending = await outbox.GetPendingAsync(BatchSize, stoppingToken);

                    if (pending.Count == 0)
                    {
                        await Task.Delay(10_000, stoppingToken);
                        continue;
                    }

                    // 1) Mapear
                    var persistItems = pending.Select(p => new NotificationPersist
                    {
                        NotificationId = 0,
                        BusinessId = p.BusinessId,
                        UsersId = p.UsersId,
                        Title = p.Title,
                        Message = p.Message,
                        Module = p.Module,
                        Entity = p.Entity,
                        EntityId = p.EntityId.ToString(),
                        LinkUrl = p.LinkUrl ?? "",
                        Type = p.Type,
                        CreatedBy = p.CreatedBy,
                        CreatedAt = p.CreatedAt,
                        ReadAt = null,
                        CreatedByName = ""
                    }).ToList();

                    // 2) Insertar en NOTIFICATIONS
                    await history.AddManyAsync(persistItems, stoppingToken);

                    // 3) Push vía SignalR
                    foreach (var row in pending)
                    {
                        await push.PushToUsersAsync(
                            new[] { row.UsersId },
                            new
                            {
                                row.Title,
                                row.Message,
                                row.Module,
                                row.Entity,
                                row.EntityId,
                                row.LinkUrl,
                                row.Type,
                                row.CreatedAt
                            },
                            stoppingToken);
                    }

                    // 4) Marcar OUTBOX como procesado
                    await outbox.MarkProcessedAsync(
                        pending.Select(p => p.OutboxId),
                        stoppingToken);

                    await Task.Delay(500, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error crítico en NotificationsOutboxWorker. Reintentando en 30s.");
                    await DelayOrStopAsync(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
        }

        private static async Task DelayOrStopAsync(TimeSpan delay, CancellationToken ct)
        {
            try
            {
                await Task.Delay(delay, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
        }
    }

}

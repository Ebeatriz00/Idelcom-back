using Application.UseCases.Notifications;
using Core.Interfaces.Notifications;
using Infrastructure.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.ServiceExtensions
{
    public static class NotificationsDI
    {
        public static IServiceCollection AddNotificationServices(this IServiceCollection services)
        {
            // Registrar SignalR
            services.AddScoped<INotificationPush, SignalRNotificationPush>();
            services.AddScoped<IRecipientsResolver, DbRecipientsResolver>();
            services.AddScoped<INotificationsOutboxRepository, DbNotificationsOutboxRepository>();
            
            services.AddScoped<PushNotificationUseCase>();
            services.AddScoped<ListNotifications>();
            services.AddScoped<MarkAllRead>();
            services.AddScoped<MarkRead>();
            services.AddScoped<AlertResolve>();
            services.AddScoped<AlertSnooze>();

            services.AddHostedService<NotificationsOutboxWorker>();
            services.AddHostedService<OpportunityAlertsWorker>();
            services.AddHostedService<SupportAlertsWorker>();
            return services;
        }
    }
}

using Core.Entities.Email;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using Infrastructure.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.ServiceExtensions
{
    public static class EmailInjections
    {
        public static IServiceCollection AddNotificationsEmailServices(this IServiceCollection services, IConfiguration cfg)
        {
            services.Configure<SmtpSettings>(cfg.GetSection("Smtp"));


            services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();

            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddSingleton<IEmailTemplateRenderer, EmailTemplateRenderer>();
            services.AddHostedService<EmailOutboxWorker>();


            return services;
        }
    }
}

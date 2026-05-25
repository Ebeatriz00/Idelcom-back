using Application.Services.Audit;
using Core.Interfaces.Audit;
using Infrastructure.Repositories.Audit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.Audit
{
    public static class AuditInjection
    {
        public static IServiceCollection AddAuditInjection(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IAuditLogFactory, AuditLogFactory>();
            services.AddScoped<IAuditService, AuditService>();

            return services;
        }
    }
}

using Application.UseCases.LicStatus;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class LicStatusInjection
    {
        public static IServiceCollection AddLicStatusServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectLicStatus>();

            services.AddScoped<ILicStatusRepository, LicStatusRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

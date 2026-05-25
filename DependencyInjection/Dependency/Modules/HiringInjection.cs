using Application.UseCases.Hiring;
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
    public static class HiringInjection
    {
        public static IServiceCollection AddHiringServices(this IServiceCollection services)
        {
            services.AddScoped<GetAllHiring>();
            services.AddScoped<UpdateStatusHiring>();
            services.AddScoped<MarkFilesRead>();

            services.AddScoped<IHiringRepository, HiringRepository>();

            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

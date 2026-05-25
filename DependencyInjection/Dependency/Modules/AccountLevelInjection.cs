using Application.UseCases.AccountLevel;
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
    public static class AccountLevelInjection
    {
        public static IServiceCollection AddAccountLevelServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectAccountLevel>();
            services.AddScoped<IAccountLevelRepository, AccountLevelRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

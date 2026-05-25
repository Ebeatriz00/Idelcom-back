using Application.UseCases.SsomaMovementTypes;
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
    public static class SsomaMovementTypeInjection
    {
        public static IServiceCollection AddSsomaMovementTypeServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaMovementType>();
            services.AddScoped<GetAllSsomaMovementType>();
            services.AddScoped<GetByIdSsomaMovementType>();
            services.AddScoped<UpdateSsomaMovementType>();
            services.AddScoped<PatchSsomaMovementType>();
            services.AddScoped<GetSelectSsomaMovementType>();

            services.AddScoped<ISsomaMovementTypeRepository, SsomaMovementTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

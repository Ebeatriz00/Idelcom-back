using Application.UseCases.AuxiliarType;
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
    public static class AuxiliarTypeInjection
    {
        public static IServiceCollection AddAuxiliarTypeServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectAuxiliarType>();
            services.AddScoped<IAuxiliarTypeRepository, AuxiliarTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

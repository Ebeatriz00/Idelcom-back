using Application.UseCases.NegotiationStages;
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
    public static class NegotiationStagesInjection
    {
        public static IServiceCollection AddNegotiationStagesServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectNegotiationStages>();

            services.AddScoped<INegotiationStagesRepository, NegotiationStagesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

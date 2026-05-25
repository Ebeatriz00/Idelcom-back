using Application.DTOs.OpporViability;
using Application.UseCases.OpporViability;
using Application.Validators.OpporViability;
using Core.Interfaces;
using FluentValidation;
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
    public static class OpporViabilityInjection
    {
        public static IServiceCollection AddOpporViabilityServices(this IServiceCollection services)
        {
            services.AddScoped<GetAllOpporViability>();
            services.AddScoped<PatchOpporViability>();
            services.AddScoped<ConvertPreOpportunity>();

            services.AddTransient<IValidator<OpporViabilityStatusToggleDto>, OpporViabilityStatusToggleValidato>();

            services.AddScoped<IOpporViabilityRepository, OpporViabilityRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

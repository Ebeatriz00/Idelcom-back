using Application.DTOs.Viability;
using Application.UseCases.Viability;
using Application.Validators.Viability;
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
    public static class ViabilityInjection
    {
        public static IServiceCollection AddViabilityServices(this IServiceCollection services)
        {
            services.AddScoped<CreateViability>();
            services.AddScoped<UpdateViability>();
            services.AddScoped<GetByIdViability>();
            services.AddScoped<GetAllViability>();
            services.AddScoped<PatchViabilityStatus>();
            services.AddScoped<GetSelectViability>();


            services.AddTransient<IValidator<ViabilityCreateDto>, ViabilityCreateValidator>();
            services.AddTransient<IValidator<ViabilityUpdateDto>, ViabilityUpdateValidator>();
            services.AddTransient<IValidator<ViabilityStatusToggleDto>, ViabilityStatusToggleValidator>();

            services.AddScoped<IViabilityRepository, ViabilityRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>(); 

            return services;
        }
    }
}

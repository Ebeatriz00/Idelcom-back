using Application.DTOs.Area;
using Application.UseCases.Area;
using Application.Validators.Area;
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
    public static class AreaInjection
    {
        public static IServiceCollection AddAreaServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateArea>();
            services.AddScoped<GetAllArea>();
            services.AddScoped<GetSelectArea>();
            services.AddScoped<GetByIdArea>();
            services.AddScoped<UpdateArea>();
            services.AddScoped<PatchAreaStatus>();

            // Validators
            services.AddTransient<IValidator<AreaCreateDto>, AreaCreateValidator>();
            services.AddTransient<IValidator<AreaUpdateDto>, AreaUpdateValidator>();
            services.AddTransient<IValidator<AreaStatusToggleDto>, AreaStatusToggleValidator>();

            // Infra
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

using Application.DTOs.Uom;
using Application.UseCases.Uom;
using Application.Validators.Uom;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class UomInjection
    {
        public static IServiceCollection AddUomServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateUom>();
            services.AddScoped<GetAllUom>();
            services.AddScoped<GetSelectUom>();
            services.AddScoped<GetByIdUom>();
            services.AddScoped<UpdateUom>();
            services.AddScoped<PatchUomStatus>();

            // Validators
            services.AddTransient<IValidator<UomCreateDto>, UomCreateValidator>();
            services.AddTransient<IValidator<UomUpdateDto>, UomUpdateValidator>();
            services.AddTransient<IValidator<UomStatusToggleDto>, UomStatusToggleValidator>();

            // Infraestructura (repositorio y conexión)
            services.AddScoped<IUomRepository, UomRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

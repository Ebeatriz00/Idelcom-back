using Application.DTOs.Modules;
using Application.UseCases.Modules;
using Application.Validators.Modules;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class ModulesInjection
    {
        public static IServiceCollection AddModulesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateModules>();
            services.AddScoped<GetAllModules>();
            services.AddScoped<GetSelectModules>();
            services.AddScoped<GetByIdModules>();
            services.AddScoped<UpdateModules>();
            services.AddScoped<PatchModulesStatus>();

            // Validators
            services.AddTransient<IValidator<ModulesCreateDto>, ModulesCreateValidator>();
            services.AddTransient<IValidator<ModulesUpdateDto>, ModulesUpdateValidator>();
            services.AddTransient<IValidator<ModulesStatusToogleDto>, ModulesStatusToggleValidator>();

            // Infra
            services.AddScoped<IModulesRepository, ModulesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.ModulePermission;
using Application.UseCases.ModulePermission;
using Application.Validators.ModulePermission;
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
    public static class ModulesPermissionsInjection
    {
        public static IServiceCollection AddModulesPermissionsServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateModulesPermissions>();
            services.AddScoped<GetAllModulesPermissions>();
            services.AddScoped<GetByIdModulesPermissions>();
            services.AddScoped<UpdateModulesPermissions>();
            services.AddScoped<PatchModulesPermissionsStatus>();

            // Validators
            services.AddTransient<IValidator<ModulesPermissionsCreateDto>, ModulesPermissionsCreateValidator>();
            services.AddTransient<IValidator<ModulesPermissionsUpdateDto>, ModulesPermissionsUpdateValidator>();
            services.AddTransient<IValidator<ModulesPermissionsStatusToggleDto>, ModulePermissionStatusToggleValidator>();

            // Infra
            services.AddScoped<IModulesPermissionsRepository, ModulesPermissionsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

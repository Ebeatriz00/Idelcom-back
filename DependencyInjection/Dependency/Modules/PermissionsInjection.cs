using Application.DTOs.Permissions;
using Application.UseCases.Permissions;
using Application.Validators.Permissions;
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
    public static class PermissionsInjection
    {
            public static IServiceCollection AddPermissionsServices(this IServiceCollection services)
            {
                services.AddScoped<CreatePermissions>();
                services.AddScoped<GetAllPermissions>();
                services.AddScoped<GetSelectPermissions>();
                services.AddScoped<GetByIdPermissions>();
                services.AddScoped<UpdatePermissions>();
                services.AddScoped<PatchPermissionsStatus>();

                services.AddTransient<IValidator<PermissionsCreateDto>, PermissionsCreateValidator>();
                services.AddTransient<IValidator<PermissionsUpdateDto>, PermissionsUpdateValidator>();
                services.AddTransient<IValidator<PermissionsStatusToggleDto>, PermissionsStatusToggleValidator>();

                services.AddScoped<IPermissionsRepository, PermissionsRepository>();
                services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

                return services;
            }
    }

}


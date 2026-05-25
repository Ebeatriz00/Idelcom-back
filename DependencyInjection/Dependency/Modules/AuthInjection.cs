using Application.DTOs.Auth;
using Application.Services;
using Application.Services.InterfacesServices;
using Application.UseCases.Auth;
using Application.Validators.Auth;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.UseCases.Auth.GetAllowedModules;
using static Application.UseCases.Auth.GetEffectivePermissions;

namespace DependencyInjection.Dependency.Modules
{
    public static class AuthInjection
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<GetAuth>();

            services.AddScoped<GetAllowedModules.GetAllowedModulesUseCase>();
            services.AddScoped<GetEffectivePermissions.GetEffectivePermissionsUseCase>();
            services.AddScoped<GetAuthBootstrapUseCase>();
            services.AddScoped<InvalidateAuthCacheUseCase>();
            // Validators
            services.AddTransient<IValidator<AuthRequestDto>, AuthValidator>();
            // Infra
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthPermisionRepository, AuthPermisionRepository>();
            services.AddScoped<IAuthPermissionService, PermissionService>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

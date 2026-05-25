using Application.DTOs.AppAuth;
using Application.UseCases.AppAuth;
using Application.Validators.AppAuth;
using Core.Interfaces.Security;
using FluentValidation;
using Infrastructure.Repositories.Security;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.AppAuth
{
    public static class AppAuthInjection
    {
        public static IServiceCollection AddAppAuthServices(this IServiceCollection services)
        {
            // 1. Use Cases
            services.AddScoped<AppLoginUseCase>();
            services.AddScoped<AppLogoutUseCase>();
            // 2. Validators
            services.AddTransient<IValidator<AppLoginRequestDto>, AppLoginRequestValidator>();
            // 3. Repositories
            services.AddScoped<IAppDeviceSessionRepository, AppDeviceSessionRepository>();
            return services;
        }
    }
}

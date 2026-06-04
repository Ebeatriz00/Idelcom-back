using Application.DTOs.MobileAppVersion;
using Application.UseCases.MobileAppVersion;
using Application.Validators.MobileAppVersion;
using Core.Interfaces.MobileAppVersion;
using FluentValidation;
using Infrastructure.Repositories.MobileAppVersion;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class MobileAppVersionInjection
    {
        public static IServiceCollection AddMobileAppVersionServices(this IServiceCollection services)
        {
            services.AddScoped<GetMobileAppVersionConfig>();
            services.AddScoped<UpsertMobileAppVersionConfig>();
            services.AddTransient<IValidator<MobileAppVersionQueryDto>, MobileAppVersionQueryValidator>();
            services.AddTransient<IValidator<MobileAppVersionUpsertDto>, MobileAppVersionUpsertValidator>();
            services.AddScoped<IMobileAppVersionConfigRepository, MobileAppVersionConfigRepository>();

            return services;
        }
    }
}

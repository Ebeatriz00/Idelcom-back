using Application.DTOs.SsomaRequirement;
using Application.UseCases.SsomaRequirement;
using Application.Validators.SsomaRequirement;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Ssoma;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class SsomaRequirementInjection
    {
        public static IServiceCollection AddSsomaRequirementInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaRequirement>();
            services.AddScoped<GetAllSsomaRequirement>();
            services.AddScoped<GetByIdSsomaRequirement>();
            services.AddScoped<GetGeneralRequirementById>();
            services.AddScoped<UpdateSsomaRequirement>();
            services.AddScoped<DeleteSsomaRequirement>();
            services.AddScoped<GetAllSsomaRequirement>();
            services.AddScoped<GetAllSsomaRequirementItem>();
            services.AddScoped<GetSelectSsomaRequirement>();

            services.AddTransient<IValidator<SsomaRequirementCreateDto>, SsomaRequirementCreateValidator>();
            services.AddTransient<IValidator<SsomaRequirementUpdateDto>, SsomaRequirementUpdateValidator>();

            services.AddScoped<ISsomaRequirementRepository, SsomaRequirementRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.SsomaOperationsRequirement;
using Application.UseCases.SsomaOperationsRequirement;
using Application.Validators.SsomaOperationsRequirement;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Ssoma;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class SsomaOperationsRequirementInjection
    {
        public static IServiceCollection AddSsomaOperationsRequirementInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaOperationsRequirement>();
            services.AddScoped<GetAllSsomaOperationsRequirement>();
            services.AddScoped<GetListSsomaOperationsRequirementByWorker>();
            services.AddScoped<ValidateSsomaOperationsRequirementByWorker>();
            services.AddScoped<GetMissingSsomaOperationsRequirementByWorker>();
            services.AddScoped<GetSelectOperationsSsomaOperationsRequirement>();
            services.AddScoped<GetByIdSsomaOperationsRequirement>();
            services.AddScoped<UpdateSsomaOperationsRequirement>();
            services.AddScoped<DeleteSsomaOperationsRequirement>();

            services.AddTransient<IValidator<SsomaOperationsRequirementCreateDto>, SsomaOperationsRequirementCreateValidator>();
            services.AddTransient<IValidator<SsomaOperationsRequirementUpdateDto>, SsomaOperationsRequirementUpdateValidator>();

            services.AddScoped<ISsomaOperationsRequirementRepository, SsomaOperationsRequirementRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

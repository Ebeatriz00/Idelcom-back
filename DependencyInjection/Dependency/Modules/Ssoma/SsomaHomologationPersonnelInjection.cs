using Application.DTOs.SsomaHomologationPersonnel;
using Application.UseCases.SsomaHomologationPersonnel;
using Application.UseCases.SsomaHomologationPersonnel.services;
using Application.Validators.SsomaHomologationPersonnel;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Ssoma;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class SsomaHomologationPersonnelInjection
    {
        public static IServiceCollection AddSsomaHomologationPersonnelInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaHomologationPersonnel>();
            services.AddScoped<CreateSsomaHomologationPersonnelOrchestrated>();
            services.AddScoped<GetAllSsomaHomologationPersonnel>();
            services.AddScoped<GetListAllPersonnelOperations>();
            services.AddScoped<GetDetailPersonnelOperations>();
            services.AddScoped<GetByIdSsomaHomologationPersonnel>();
            services.AddScoped<UpdateSsomaHomologationPersonnel>();
            services.AddScoped<DeleteSsomaHomologationPersonnel>();
            services.AddScoped<SsomaHomologationPersonnelBusinessRules>();
            services.AddScoped<ISsomaHomologationCalculator, SsomaHomologationCalculator>();

            services.AddTransient<IValidator<SsomaHomologationPersonnelCreateDto>, SsomaHomologationPersonnelCreateValidator>();
            services.AddTransient<IValidator<SsomaHomologationPersonnelCreateOrchestratedDto>, SsomaHomologationPersonnelCreateOrchestratedValidator>();
            services.AddTransient<IValidator<SsomaHomologationPersonnelUpdateDto>, SsomaHomologationPersonnelUpdateValidator>();

            services.AddScoped<ISsomaHomologationPersonnelRepository, SsomaHomologationPersonnelRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

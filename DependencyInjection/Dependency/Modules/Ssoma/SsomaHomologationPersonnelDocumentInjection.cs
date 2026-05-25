using Application.DTOs.SsomaHomologationPersonnelDocument;
using Application.UseCases.SsomaHomologationPersonnelDocument;
using Application.Validators.SsomaHomologationPersonnelDocument;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Ssoma;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class SsomaHomologationPersonnelDocumentInjection
    {
        public static IServiceCollection AddSsomaHomologationPersonnelDocumentInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaHomologationPersonnelDocument>();
            services.AddScoped<GetAllSsomaHomologationPersonnelDocument>();
            services.AddScoped<GetByIdSsomaHomologationPersonnelDocument>();
            services.AddScoped<UpdateSsomaHomologationPersonnelDocument>();
            services.AddScoped<ReplaceSsomaHomologationPersonnelDocument>();
            services.AddScoped<DeleteSsomaHomologationPersonnelDocument>();
            services.AddScoped<SsomaHomologationPersonnelDocumentBusinessRules>();

            services.AddTransient<IValidator<SsomaHomologationPersonnelDocumentCreateDto>, SsomaHomologationPersonnelDocumentCreateValidator>();
            services.AddTransient<IValidator<SsomaHomologationPersonnelDocumentUpdateDto>, SsomaHomologationPersonnelDocumentUpdateValidator>();

            services.AddScoped<ISsomaHomologationPersonnelDocumentRepository, SsomaHomologationPersonnelDocumentRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

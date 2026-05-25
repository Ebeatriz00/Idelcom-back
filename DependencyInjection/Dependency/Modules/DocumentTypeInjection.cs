
using Application.DTOs.DocumentType;
using Application.UseCases.DocumentType;
using Application.Validators.DocumentType;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependency.Modules
{
    public static class DocumentTypeInjection
    {
        public static IServiceCollection AddDocumentTypeServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateDocumentType>();
            services.AddScoped<GetAllDocumentTypes>();
            services.AddScoped<GetSelectDocumentType>();
            services.AddScoped<GetByIdDocumentTypes>();
            services.AddScoped<UpdateDocumentType>();
            services.AddScoped<PatchDocumentTypeStatus>();

            // Validators
            services.AddTransient<IValidator<DocumentTypeCreateDto>, DocumentTypeCreateValidator>();
            services.AddTransient<IValidator<DocumentTypeUpdateDto>, DocumentTypeUpdateValidator>();
            services.AddTransient<IValidator<DocumentTypeStatusToggleDto>, DocumentTypeStatusToggleValidator>();

            // Infra
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

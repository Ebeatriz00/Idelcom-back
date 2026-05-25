using Application.DTOs.ConceptType;
using Application.UseCases.ConceptType;
using Application.Validators.ConceptType;
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
    public static class ConceptTypeInjection
    {
        public static IServiceCollection AddConceptTypeServices(this IServiceCollection services)
        {
            services.AddScoped<CreateConceptType>();
            services.AddScoped<GetAllConceptType>();
            services.AddScoped<GetSelectConceptType>();
            services.AddScoped<GetByIdConceptType>();
            services.AddScoped<UpdateConceptType>();
            services.AddScoped<PatchConceptTypeStatus>();

            services.AddTransient<IValidator<ConceptTypeCreateDto>, ConceptTypeCreateValidator>();
            services.AddTransient<IValidator<ConceptTypeUpdateDto>, ConceptTypeUpdateValidator>();
            services.AddTransient<IValidator<ConceptTypeStatusToggleDto>, ConceptTypeStatusToggleValidator>();

            services.AddScoped<IConceptTypeRepository, ConceptTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

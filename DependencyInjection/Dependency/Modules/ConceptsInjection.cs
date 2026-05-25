using Application.DTOs.Concepts;
using Application.UseCases.Concepts;
using Application.Validators.Concepts;
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
    public static class ConceptsInjection
    {
        public static IServiceCollection AddConceptsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateConcepts>();
            services.AddScoped<GetAllConcepts>();
            services.AddScoped<GetConceptsById>();
            services.AddScoped<UpdateConcepts>();
            services.AddScoped<PatchConceptsStatus>();
            services.AddScoped<GetSelectConcepts>();
           

            services.AddTransient<IValidator<ConceptsCreateDto>, ConceptsCreateValidator>();
            services.AddTransient<IValidator<ConceptsUpdateDto>, ConceptsUpdateValidator>();
            services.AddTransient<IValidator<ConceptsStatusToggleDto>, ConceptsStatusToggleValidator>();

            services.AddScoped<IConceptsRepository, ConceptsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

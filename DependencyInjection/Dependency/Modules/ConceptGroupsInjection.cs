using Application.DTOs.ConceptGroups;
using Application.UseCases.ConceptGroups;
using Application.Validators.ConceptGroups;
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
    public static class ConceptGroupsInjection
    {
        public static IServiceCollection AddConceptGroupsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateConceptGroups>();
            services.AddScoped<GetAllConceptGroups>();
            services.AddScoped<GetConceptGroupsById>();
            services.AddScoped<UpdateConceptGroups>();
            services.AddScoped<PatchConceptGroupsStatus>();
            services.AddScoped<GetSelectConceptGroups>();

            services.AddTransient<IValidator<ConceptGroupsCreateDto>, ConceptGroupsCreateValidator>();
            services.AddTransient<IValidator<ConceptGroupsUpdateDto>, ConceptGroupsUpdateValidator>();
            services.AddTransient<IValidator<ConceptGroupsStatusToggleDto>, ConceptGroupsStatusToggleValidator>();

            services.AddScoped<IConceptGroupsRepository, ConceptGroupsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

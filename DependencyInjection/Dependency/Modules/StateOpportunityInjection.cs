
using Application.DTOs.StateOpportunity;
using Application.UseCases.StateOpportunity;
using Application.UseCases.StatePreSale;
using Application.Validators.StateOpportunity;
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
    public static class StateOpportunityInjection
    {
        public static IServiceCollection AddStateOpportunityServices(this IServiceCollection services) {

            services.AddScoped<CreateStateOpportunity>();
            services.AddScoped<GetAllStateOpportunity>();
            services.AddScoped<GetSelectStateOpportunity>();
            services.AddScoped<GetByIdStateOpportunity>();
            services.AddScoped<UpdateStateOpportunity>();
            services.AddScoped<PatchStateOpportunity>();
            // Validators
            services.AddTransient<IValidator<StateOpportunityCreateDto>, StateOpportunityCreateValidator>();
            services.AddTransient<IValidator<StateOpportunityUpdateDto>, StateOpportunityUpdateValidator>();
            services.AddTransient<IValidator<StateOpportunityStatusToggleDto>, StateOpportunityStatusToggleValidator>();
            // Infra
            services.AddScoped<IStateOpportunityRepository, StateOpportunityRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

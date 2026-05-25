using Application.DTOs.LeadsSources;
using Application.UseCases.LeadsSources;
using Application.Validators.LeadsSources;
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
    public static class LeadsSourcesInjection
    {
        public static IServiceCollection AddLeadsSourcesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateLeadsSources>();
            services.AddScoped<GetAllLeadsSources>();
            services.AddScoped<GetLeadsSourcesById>();
            services.AddScoped<UpdateLeadsSources>();
            services.AddScoped<PatchLeadsSourcesStatus>();
            services.AddScoped<GetSelectLeadsSources>();

            services.AddTransient<IValidator<LeadsSourcesCreateDto>, LeadsSourcesCreateValidator>();
            services.AddTransient<IValidator<LeadsSourcesUpdateDto>, LeadsSourcesUpdateValidator>();
            services.AddTransient<IValidator<LeadsSourcesStatusToggleDto>, LeadsSourcesStatusToggleValidator>();


            services.AddScoped<ILeadsSourcesRepository, LeadsSourcesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

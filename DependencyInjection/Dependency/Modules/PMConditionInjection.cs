using Application.DTOs.PMCondition;
using Application.UseCases.PMCondition;
using Application.Validators.PMCondition;
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
    public static class PMConditionInjection
    {
        public static IServiceCollection AddPMConditionServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePMCondition>();
            services.AddScoped<GetAllPMCondition>();
            services.AddScoped<GetPMConditionById>();
            services.AddScoped<UpdatePMCondition>();
            services.AddScoped<PatchPMConditionStatus>();
            services.AddScoped<GetSelectPMCondition>();

            services.AddTransient<IValidator<PMConditionCreateDto>, PMConditionCreateValidator>();
            services.AddTransient<IValidator<PMConditionUpdateDto>, PMConditionUpdateValidator>();
            services.AddTransient<IValidator<PMConditionStatusToggleDto>, PMConditionStatusToggleValidator>();

            services.AddScoped<IPMConditionRepository, PMConditionRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.ProcessType;
using Application.UseCases.ProcessType;
using Application.Validators.ProcessType;
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
    public static class ProcessTypeInjection
    {
        public static IServiceCollection AddProcessTypeServices(this IServiceCollection services)
        {

            services.AddScoped<CreateProcessType>();
            services.AddScoped<GetAllProcessType>();
            services.AddScoped<GetSelectProcessType>();
            services.AddScoped<GetByIdProcessType>();
            services.AddScoped<UpdateProcessType>();
            services.AddScoped<PatchProcessType>();
            // Validators
            services.AddTransient<IValidator<ProcessTypeCreateDto>, ProcessTypeCreateValidator>();
            services.AddTransient<IValidator<ProcessTypeUpdateDto>, ProcessTypeUpdateValidator>();
            services.AddTransient<IValidator<ProcessTypeStatusToggleDto>, ProcessTypeStatusToggleValidator>();
            // Infra
            services.AddScoped<IProcessTypeReporsitory, ProcessTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

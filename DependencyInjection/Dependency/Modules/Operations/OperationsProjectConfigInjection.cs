using Application.DTOs.OperationsProjectConfing;
using Application.UseCases.OperationsProjectConfig;
using Application.Validators.OperationsProjectConfig;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsProjectConfigInjection
    {
        public static IServiceCollection AddOperationsProjectConfigInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsProjectConfig>();
            services.AddScoped<GetAllOperationsProjectConfig>();
            services.AddScoped<GetByIdOperationsProjectConfig>();
            services.AddScoped<UpdateOperationsProjectConfig>();

            services.AddTransient<IValidator<OperationsProjectConfigCreateDto>, OperationsProjectConfigCreateValidator>();
            services.AddTransient<IValidator<OperationsProjectConfigUpdateDto>, OperationsProjectConfigUpdateValidator>();

            services.AddScoped<IOperationsProjectConfigRepository, OperationsProjectConfigRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

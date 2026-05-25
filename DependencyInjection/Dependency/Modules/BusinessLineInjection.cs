using Application.DTOs.BusinessLine;
using Application.UseCases.BusinessLine;
using Application.Validators.BusinessLine;
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
    public static class BusinessLineInjection
    {
        public static IServiceCollection AddBusinessLineServices(this IServiceCollection services)
        {

            services.AddScoped<CreateBusinessLine>();
            services.AddScoped<GetAllBusinessLine>();
            services.AddScoped<GetSelectBusinessLine>();
            services.AddScoped<GetByIdBusinessLine>();
            services.AddScoped<UpdateBusinessLine>();
            services.AddScoped<PatchBusinessLine>();
            // Validators
            services.AddTransient<IValidator<BusinessLineCreateDto>, BusinessLineCreateValidator>();
            services.AddTransient<IValidator<BusinessLineUpdateDto>, BusinessLineUpdateValidator>();
            services.AddTransient<IValidator<BusinessLineStatusToggleDto>, BusinessLineStatusToggleValidator>();
            // Infra
            services.AddScoped<IBusinessLineRepository, BusinessLineRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

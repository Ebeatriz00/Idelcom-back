using Application.DTOs.CommercialParameters;
using Application.UseCases.CommercialParameters;
using Application.Validators.CommercialParameters;
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
    public static class CommercialParametersInjection
    {
        public static IServiceCollection AddCommercialParametersServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateCommercialParameters>();
            services.AddScoped<GetAllCommercialParameters>();
            services.AddScoped<GetByIdCommercialParameters>();
            services.AddScoped<UpdateCommercialParameters>();
            services.AddScoped<PatchCommercialParameters>();

            // Validators
            services.AddTransient<IValidator<CreateCommercialParametersDto>, CommercialParametersCreateValidator>();
            services.AddTransient<IValidator<UpdateCommercialParametersDto>, CommercialParametersUpdateValidator>();
            services.AddTransient<IValidator<CommercialParametersStatusToggleDto>, CommercialParametersStatusToggleValidator>();
            // Infra
            services.AddScoped<ICommercialParametersRepository, CommercialParametersRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

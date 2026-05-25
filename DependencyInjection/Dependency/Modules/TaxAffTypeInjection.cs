using Application.DTOs.TaxAffType;
using Application.UseCases.TaxAffType;
using Application.Validators.TaxAffType;
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
    public static class TaxAffTypeInjection
    {
        public static IServiceCollection AddTaxAffTypeServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateTaxAffType>();
            services.AddScoped<GetAllTaxAffType>();
            services.AddScoped<GetSelectTaxAffType>();
            services.AddScoped<GetByIdTaxAffType>();
            services.AddScoped<UpdateTaxAffType>();
            services.AddScoped<PatchTaxAffType>();
            // Validators
            services.AddTransient<IValidator<TaxAffTypeCreateDto>, TaxAffTypeCreateValidator>();
            services.AddTransient<IValidator<TaxAffTypeUpdateDto>, TaxAffTypeUpdateValidator>();
            services.AddTransient<IValidator<TaxAffTypeStatusToggleDto>, TaxAffTypeStatusToggleValidator>();
            // Infra
            services.AddScoped<ITaxAffTypeRepository, TaxAffTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

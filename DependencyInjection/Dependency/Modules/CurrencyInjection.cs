using Application.DTOs.Currency;
using Application.UseCases.Currency;
using Application.Validators.Currency;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class CurrencyInjection
    {
        public static IServiceCollection AddCurrencyServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateCurrency>();
            services.AddScoped<GetAllCurrency>();
            services.AddScoped<GetSelectCurrency>();
            services.AddScoped<GetByIdCurrency>();
            services.AddScoped<UpdateCurrency>();
            services.AddScoped<PatchCurrencyStatus>();
            // Validators
            services.AddTransient<IValidator<CurrencyCreateDto>, CurrencyCreateValidator>();
            services.AddTransient<IValidator<CurrencyUpdateDto>, CurrencyUpdateValidator>();
            services.AddTransient<IValidator<CurrencyStatusToggleDto>, CurrencyStatusToggleValidator>();
            // Infra
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

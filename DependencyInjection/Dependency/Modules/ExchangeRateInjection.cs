using Application.DTOs.ExchangeRate;
using Application.UseCases.ExchangeRate;
using Application.Validators.ExchangeRate;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistance.Repositories;
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
    public static class ExchangeRateInjection
    {
        public static IServiceCollection AddExchangeRateServices(this IServiceCollection services)
        {
            services.AddScoped<CreateExchangeRate>();
            services.AddScoped<UpdateExchangeRate>();
            services.AddScoped<PatchExchangeRateStatus>();
            services.AddScoped<GetAllExchangeRate>();
            services.AddScoped<GetByIdExchangeRate>();
            services.AddScoped<GetSelectExchangeRate>();

            services.AddTransient<IValidator<ExchangeRateCreateDto>, ExchangeRateCreateValidator>();
            services.AddTransient<IValidator<ExchangeRateUpdateDto>, ExchangeRateUpdateValidator>();
            services.AddTransient<IValidator<ExchangeRateStatusToggleDto>, ExchangeRateStatusToggleValidator>();

            services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

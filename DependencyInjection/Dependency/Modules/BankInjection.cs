
using Application.DTOs.Bank;
using Application.DTOs.ConceptType;
using Application.UseCases.Bank;
using Application.Validators.Bank;
using Application.Validators.ConceptType;
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
    public static class BankInjection
    {
        public static IServiceCollection AddBankServices(this IServiceCollection services)
        {
            services.AddScoped<CreateBank>();
            services.AddScoped<GetAllBank>();
            services.AddScoped<GetSelectBank>();
            services.AddScoped<GetByIdBank>();
            services.AddScoped<UpdateBank>();
            services.AddScoped<PatchBankStatus>();

            services.AddTransient<IValidator<BankCreateDto>, BankCreateValidator>();
            services.AddTransient<IValidator<BankUpdateDto>, BankUpdateValidator>();
            services.AddTransient<IValidator<BankStatusToggleDto>, BankStatusToggleValidator>();

            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

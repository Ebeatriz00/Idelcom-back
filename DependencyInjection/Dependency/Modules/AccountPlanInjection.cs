using Application.DTOs.AccountPlan;
using Application.UseCases.AccountPlan;
using Application.Validators.AccountPlan;
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
    public static class AccountPlanInjection
    {
        public static IServiceCollection AddAccountPlanServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateAccountPlan>();
            services.AddScoped<GetAllAccountPlan>();
            services.AddScoped<GetSelectAccountPlan>();
            services.AddScoped<GetByIdAccountPlan>();
            services.AddScoped<UpdateAccountPlan>();
            services.AddScoped<PatchAccountPlan>();
            // Validators
            services.AddTransient<IValidator<AccountPlanCreateDto>, AccountPlanCreateValidator>();
            services.AddTransient<IValidator<AccountPlanUpdateDto>, AccountPlanUpdateValidator>();
            services.AddTransient<IValidator<AccountPlanStatusToggleDto>, AccountPlanStatusToggleValidator>();
            // Infra
            services.AddScoped<IAccountPlanRepository, AccountPlanRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

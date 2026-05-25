using Application.DTOs.Account;
using Application.UseCases.Account;
using Application.Validators.Account;
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
    public static class AccountInjection
    {
        public static IServiceCollection AddAccountServices(this IServiceCollection services)
        {
            services.AddScoped<CreateAccount>();
            services.AddScoped<GetAllAccount>();
            services.AddScoped<GetAccountById>();
            services.AddScoped<UpdateAccount>();
            services.AddScoped<PatchAccountStatus>();
            services.AddScoped<GetSelectAccount>();

            services.AddTransient<IValidator<AccountCreateDto>, AccountCreateValidator>();
            services.AddTransient<IValidator<AccountUpdateDto>, AccountUpdateValidator>();
            services.AddTransient<IValidator<AccountStatusToggleDto>, AccountStatusToggleValidator>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

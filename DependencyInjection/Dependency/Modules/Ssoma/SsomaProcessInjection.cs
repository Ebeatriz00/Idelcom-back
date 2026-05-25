using Application.DTOs.OperationsTeamSsoma;
using Application.DTOs.SsomaProcess;
using Application.UseCases.SsomaProcess;
using Application.Validators.OperationsTeamSsoma;
using Application.Validators.SsomaProcess;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Ssoma;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class SsomaProcessInjection
    {
        public static IServiceCollection AddSsomaProcessInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaProcess>();
            services.AddScoped<UpdateSsomaProcess>();
            services.AddScoped<GetByIdSsomaProcess>();
            services.AddScoped<GetAllSsomaProcess>();
            services.AddScoped<DeleteSsomaProcess>();

            services.AddTransient<IValidator<SsomaProcessCreateDto>, SsomaProcessCreateValidator>();
            services.AddTransient<IValidator<SsomaProcessUpdateDto>, SsomaProcessUpdateValidator>();

            services.AddScoped<ISsomaProcessRepository, SsomaProcessRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

using Application.DTOs.Ssoma;
using Application.DTOs.SsomaAssignmanetType;
using Application.UseCases.SsomaAssignmanetType;
using Application.Validators.SsomaAsiggnmanetType;
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
    public static class SsomaAsiggnmanetTypeInjection
    {
        public static IServiceCollection AddSsomaAsiggnmanetTypeServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaAssignmanetType>();
            services.AddScoped<GetAllSsomaAssignmanetType>();
            services.AddScoped<GetSelectSsomaAssignmanetType>();
            services.AddScoped<GetByIdSsomaAssignmanetType>();
            services.AddScoped<UpdateSsomaAssignmanetType>();
            services.AddScoped<PatchSsomaAssignmanetType>();

            services.AddTransient<IValidator<SsomaAssignmanetTypeCreateDto>, SsomaAsiggnmanetTypeCreateValidator>();
            services.AddTransient<IValidator<SsomaAsiggnmanetTypeUpdateDto>, SsomaAsiggnmanetTypeUpdateValidator>();
            services.AddTransient<IValidator<SsomaAssignmanetTypeStatusToggleDto>, SsomaAsiggnmanetTypeStatusToggleValidator>();

            services.AddScoped<ISsomaAssignmanetTypeRepository, SsomaAssignmanetTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

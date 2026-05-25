using Application.DTOs.Periods;
using Application.UseCases.Periods;
using Application.Validators.Periods;
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
    public static class PeriodsInjection
    {
        public static IServiceCollection AddPeriodsServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePeriods>();
            services.AddScoped<GetAllPeriods>();
            services.AddScoped<GetPeriodsById>();
            services.AddScoped<UpdatePeriods>();
            services.AddScoped<PatchPeriodsStatus>();
            services.AddScoped<GetSelectPeriods>();
            services.AddScoped<ToggleBlockPeriods>();


            services.AddTransient<IValidator<PeriodsCreateDto>, PeriodsCreateValidator>();
            services.AddTransient<IValidator<PeriodsUpdateDto>, PeriodsUpdateValidator>();
            services.AddTransient<IValidator<PeriodsStatusToggleDto>, PeriodsStatusToggleValidator>();
            services.AddTransient<IValidator<PeriodsBlockToggleDto>, PeriodsBlockToggleValidator>();

            services.AddScoped<IPeriodsRepository, PeriodsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

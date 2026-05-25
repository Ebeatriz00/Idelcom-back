using Application.DTOs.PriorityState;
using Application.UseCases.PriorityState;
using Application.Validators.PriorityState;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class PriorityStateInjection
    {
        public static IServiceCollection AddPriorityStateServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePriorityState>();
            services.AddScoped<GetAllPriorityState>();
            services.AddScoped<GetSelectPriorityState>();
            services.AddScoped<GetSelectNormalPriorityState>();
            services.AddScoped<GetByIdPriorityState>();
            services.AddScoped<UpdatePriorityState>();
            services.AddScoped<PatchPriorityState>();

            services.AddTransient<IValidator<PriorityStateCreateDto>, PriorityStateCreateValidator>();
            services.AddTransient<IValidator<PriorityStateUpdateDto>, PriorityStateUpdateValidator>();
            services.AddTransient<IValidator<PriorityStateStatusToggleDto>, PriorityStateStatusToggleValidator>();

            services.AddScoped<IPriorityStateRepository, PriorityStateRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>(); 

            return services;
        }
    }
}

using Application.DTOs.StateOpportunity;
using Application.DTOs.StatePreSale;
using Application.UseCases.StateOpportunity;
using Application.UseCases.StatePreSale;
using Application.Validators.StateOpportunity;
using Application.Validators.StatePreSale;
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
    public static class StatePreSaleInjection
    {
        public static IServiceCollection AddStatePreSaleServices(this IServiceCollection services)
        {

            services.AddScoped<CreateStatePreSale>();
            services.AddScoped<GetAllStatePreSale>();
            services.AddScoped<GetSelectStatePreSale>();
            services.AddScoped<GetStatePreSaleById>();
            services.AddScoped<UpdateStatePreSale>();
            services.AddScoped<PatchStatePreSaleStatus>();

            services.AddTransient<IValidator<StatePreSaleCreateDto>, StatePreSaleCreateValidator>();
            services.AddTransient<IValidator<StatePreSaleUpdateDto>, StatePreSaleUpdateValidator>();
            services.AddTransient<IValidator<StatePreSaleStatusToggleDto>, StatePreSaleStatusToggleValidator>();

            services.AddScoped<IStatePreSaleRepository, StatePreSaleRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

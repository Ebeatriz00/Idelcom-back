using Application.DTOs.CostCenters;
using Application.UseCases.CostCenters;
using Application.Validators.CostCenters;
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
    public static class CostCentersInjection
    {
        public static IServiceCollection AddCostCentersServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateCostCenters>();
            services.AddScoped<GetAllCostCenters>();
            services.AddScoped<GetSelectCostCenters>();
            services.AddScoped<GetByIdCostCenters>();
            services.AddScoped<UpdateCostCenters>();
            services.AddScoped<PatchCostCentersStatus>();

            // Validators
            services.AddTransient<IValidator<CostCentersCreateDto>, CostCentersCreateValidator>();
            services.AddTransient<IValidator<CostCentersUpdateDto>, CostCentersUpdateValidator>();
            services.AddTransient<IValidator<CostCentersStatusToggleDto>, CostCentersStatusToggleValidator>();

            // Infra
            services.AddScoped<ICostCentersRepository, CostCentersRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.PreSaleProyects;
using Application.UseCases.PreSaleProyects;
using Application.UseCases.PreSaleProyects.Application.UseCases.PreSaleProyects;
using Application.Validators.PreSaleProyects;
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
    public static class PreSaleProyectsInjection
    {
        public static IServiceCollection AddPreSaleProyectsServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePreSaleProyects>();
            services.AddScoped<GetAllPreSaleProyects>();
            services.AddScoped<GetPreSaleProyectsById>();
            services.AddScoped<UpdatePreSaleProyects>();
            services.AddScoped<PatchPreSaleProyectsStatus>();
            services.AddScoped<GetSelectPreSaleProyects>();
            services.AddScoped<GetNumPreSaleProyects>();
            services.AddScoped<GetDetailPreSaleProyects>();
            services.AddScoped<UpdateResponsiblePreSaleProject>();
            services.AddScoped<UpdateStatePreSaleProyects>();

            services.AddTransient<IValidator<PreSaleProyectsCreateDto>, PreSaleProyectsCreateValidator>();
            services.AddTransient<IValidator<PreSaleProyectsUpdateDto>, PreSaleProyectsUpdateValidator>();
            services.AddTransient<IValidator<PreSaleProyectsStatusToggleDto>, PreSaleProyectsStatusToggleValidator>();
            services.AddTransient<IValidator<PreSaleProjectsUpdateStateDto>, PreSaleProyectsUpdateStateValidator>();

            services.AddScoped<IPreSaleProyectsRepository, PreSaleProyectsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

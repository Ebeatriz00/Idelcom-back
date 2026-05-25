using Application.DTOs.Opportunities;
using Application.UseCases.Opportunities;
using Application.Validators.Opportunities;
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
    public static class OpportunitiesInjection
    {
        public static IServiceCollection AddOpportunitiesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateOpportunities>();
            services.AddScoped<UpdateOpportunities>();
            services.AddScoped<UpdateStateOpportunities>();
            services.AddScoped<UpdateClientsOpportunities>();
            services.AddScoped<GetByIdClientsOpportunities>();
            services.AddScoped<GetByIdOpportunities>();
            services.AddScoped<GetByIdStateOpportunities>();
            services.AddScoped<OpportunitiesStateUpdateDto>();

            services.AddScoped<UploadNewVerOpportunities>();


            services.AddScoped<GetAllOpportunities>();
            services.AddScoped<GetNumOpportunities>();
            services.AddScoped<GetDetailOpportunities>();
            services.AddScoped<PatchOpportunities>();
            services.AddScoped<GetSelectOpportunities>();
            services.AddScoped<GetSelectVerQuo>();
            services.AddScoped<GetSelectDeliverables>();
            services.AddScoped<GetSelectDeliverablesHiring>();
            services.AddScoped<GetSelectFlowType>();
            services.AddScoped<UpdateDeliverablesOpportunities>();
            services.AddScoped<AttachHiringFiles>();

            services.AddTransient<IValidator<OpportunitiesCreateDto>, OpportunitiesCreateValidator>();
            services.AddTransient<IValidator<OpportunitiesUpdateDto>, OpportunitiesUpdateValidator>();
            services.AddTransient<IValidator<OpportunitiesStateUpdateDto>, OpportunitiesStateUpdateValidator>();
            services.AddTransient<IValidator<OpportunitiesClientsUpdateDto>, OpportunitiesClientsUpdateValidator>();
            services.AddTransient<IValidator<OpportunitiesStatusToggleDto>, OpportunitiesStatusToggleValidator>();
            services.AddTransient<IValidator<OpportunitiesUploadNewVerDto>, OpportunitiesUploadNewVerValidator>();

            services.AddScoped<IOpportunitiesRepository, OpportunitiesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

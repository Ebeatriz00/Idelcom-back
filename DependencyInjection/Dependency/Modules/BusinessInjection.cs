using Application.DTOs.Business;
using Application.UseCases.Business;
using Application.Validators.Business;
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
    public static class BusinessInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateBusiness>();
            //services.AddScoped<GetAllBusiness>();
            //services.AddScoped<GetByIdBusiness>();
            services.AddScoped<GetViewBusiness>();
            //services.AddScoped<UpdateBusiness>();
            //services.AddScoped<PatchBusinessStatus>();
            // Validators
            services.AddTransient<IValidator<BusinessCreateDto>,BusinessCreateValidator>();
            //services.AddTransient<IValidator<BusinessUpdateDto>,usinessUpdateValidator>();
            //services.AddTransient<IValidator<BusinessStatusToggleDto>,usinessStatusToggleValidator>();
            //// Infra
            services.AddScoped<IBusinessRepository,BusinessRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

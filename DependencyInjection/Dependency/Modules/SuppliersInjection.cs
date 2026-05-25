using Application.DTOs.Suppliers;
using Application.UseCases.Suppliers;
using Application.Validators.Suppliers;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class SuppliersInjection
    {
        public static IServiceCollection AddSuppliersServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSuppliers>();
            services.AddScoped<GetAllSuppliers>();
            services.AddScoped<GetSuppliersById>();
            services.AddScoped<UpdateSuppliers>();
            services.AddScoped<PatchSuppliersStatus>();
            services.AddScoped<GetSelectSuppliers>();

            services.AddTransient<IValidator<SuppliersCreateDto>, SuppliersCreateValidator>();
            services.AddTransient<IValidator<SuppliersUpdateDto>, SuppliersUpdateValidator>();
            services.AddTransient<IValidator<SuppliersStatusToggleDto>, SuppliersStatusToggleValidator>();

            services.AddScoped<ISuppliersRepository, SuppliersRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.SupplierGroups;
using Application.UseCases.SupplierGroups;
using Application.Validators.SupplierGroups;
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
    public static class SupplierGroupsInjection
    {
        public static IServiceCollection AddSupplierGroupsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSupplierGroups>();
            services.AddScoped<GetAllSupplierGroups>();
            services.AddScoped<GetSelectSupplierGroups>();
            services.AddScoped<GetByIdSupplierGroups>();
            services.AddScoped<UpdateSupplierGroups>();
            services.AddScoped<PatchSupplierGroupsStatus>();

            services.AddTransient<IValidator<SupplierGroupsCreateDto>, SupplierGroupsCreateValidator>();
            services.AddTransient<IValidator<SupplierGroupsUpdateDto>, SupplierGroupsUpdateValidator>();
            services.AddTransient<IValidator<SupplierGroupsStatusToggleDto>, SupplierGroupsStatusToggleValidator>();

            services.AddScoped<ISupplierGroupsRepository, SupplierGroupsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

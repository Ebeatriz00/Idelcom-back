using Application.DTOs.Brands;
using Application.UseCases.Brands;
using Application.Validators.Brands;
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
    public static class BrandsInjection
    {
        public static IServiceCollection AddBrandsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateBrands>();
            services.AddScoped<GetAllBrands>();
            services.AddScoped<GetBrandsById>();
            services.AddScoped<UpdateBrands>();
            services.AddScoped<PatchBrandsStatus>();
            services.AddScoped<GetSelectBrands>();

            services.AddTransient<IValidator<BrandsCreateDto>, BrandsCreateValidator>();
            services.AddTransient<IValidator<BrandsUpdateDto>, BrandsUpdateValidator>();
            services.AddTransient<IValidator<BrandsStatusToggleDto>, BrandsStatusToggleValidator>();

            services.AddScoped<IBrandsRepository, BrandsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.Products;
using Application.UseCases.Products;
using Application.Validators.Products;
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
    public static class ProductsInjection
    {
        public static IServiceCollection AddProductsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateProducts>();
            services.AddScoped<UpdateProducts>();
            services.AddScoped<GetAllProducts>();
            services.AddScoped<GetByIdProducts>();
            services.AddScoped<PatchProducts>();
            services.AddScoped<GetSelectProducts>();

            services.AddTransient<IValidator<ProductsCreateDto>, ProductsCreateValidator>();
            services.AddTransient<IValidator<ProductsUpdateDto>, ProductsUpdateValidator>();
            services.AddTransient<IValidator<ProductsStatusToggleDto>, ProductsStatusToggleValidator>();

            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

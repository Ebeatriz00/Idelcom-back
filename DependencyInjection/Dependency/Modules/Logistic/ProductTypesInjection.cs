using Application.DTOs.ProductTypes;
using Application.UseCases.ProductTypes;
using Application.Validators.ProductTypes;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.Logistic
{
    public static class ProductTypesInjection
    {
        public static IServiceCollection AddProductTypesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateProductTypes>();
            services.AddScoped<GetAllProductTypes>();
            services.AddScoped<GetProductTypesById>();
            services.AddScoped<UpdateProductTypes>();
            services.AddScoped<PatchProductTypesStatus>();
            services.AddScoped<GetSelectProductTypes>();

            services.AddTransient<IValidator<ProductTypesCreateDto>, ProductTypesCreateValidator>();
            services.AddTransient<IValidator<ProductTypesUpdateDto>, ProductTypesUpdateValidator>();
            services.AddTransient<IValidator<ProductTypesStatusToggleDto>, ProductTypesStatusToggleValidator>();

            services.AddScoped<IProductTypesRepository, ProductTypesRepository>();

            return services;
        }
    }
}

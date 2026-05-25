using Application.DTOs.ProductLines;
using Application.UseCases.ProductLines;
using Application.Validators.ProductLines;
using Core.Interfaces.logistic;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.Logistic
{
    public static class ProductLinesInjection
    {
        public static IServiceCollection AddProductLinesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateProductLines>();
            services.AddScoped<GetAllProductLines>();
            services.AddScoped<GetProductLinesById>();
            services.AddScoped<UpdateProductLines>();
            services.AddScoped<PatchProductLinesStatus>();
            services.AddScoped<GetSelectProductLines>();

            services.AddTransient<IValidator<ProductLinesCreateDto>, ProductLinesCreateValidator>();
            services.AddTransient<IValidator<ProductLinesUpdateDto>, ProductLinesUpdateValidator>();
            services.AddTransient<IValidator<ProductLinesStatusToggleDto>, ProductLinesStatusToggleValidator>();

            services.AddScoped<IProductLinesRepository, ProductLinesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.FileTrackingProducts;
using Application.UseCases.FileTrackingProducts;
using Application.Validators.FileTrackingProducts;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class FileTrackingProductsInjection
    {
        public static IServiceCollection AddFileTrackingProductsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateProductFile>();
            services.AddScoped<GetAllProductFiles>();
            services.AddScoped<DeleteProductFile>();

            services.AddTransient<IValidator<FileTrackingProductsCreateDto>, FileTrackingProductsCreateValidator>();
            services.AddTransient<IValidator<FileTrackingProductsDeleteDto>, FileTrackingProductsDeleteValidator>();

            services.AddScoped<IFileTrackingProductsRepository, FileTrackingProductsRepository>();

            return services;
        }
    }
}

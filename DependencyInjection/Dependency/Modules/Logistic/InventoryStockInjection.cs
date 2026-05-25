using Application.DTOs.InventoryStock;
using Application.UseCases.InventoryStock;
using Application.Validators.InventoryStock;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Logistic
{
    public static class InventoryStockInjection
    {
        public static IServiceCollection AddInventoryStockServices(this IServiceCollection services)
        {
            services.AddScoped<CreateInventoryStock>();
            services.AddScoped<InventoryStockBusinessRules>();

            services.AddTransient<IValidator<InventoryStockCreateDto>, InventoryStockCreateValidator>();

            services.AddScoped<IInventoryStockRepository, InventoryStockRepository>();
            services.AddScoped<IInventoryKardexRepository, InventoryKardexRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

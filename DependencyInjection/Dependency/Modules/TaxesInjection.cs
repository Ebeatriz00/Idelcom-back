using Application.UseCases.Taxes;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class TaxesInjection
    {
        public static IServiceCollection AddTaxesServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectTaxes>();

            services.AddScoped<ITaxesRepository, TaxesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

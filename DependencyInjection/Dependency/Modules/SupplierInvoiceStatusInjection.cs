using Application.UseCases.SupplierInvoiceStatus;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class SupplierInvoiceStatusInjection
    {
        public static IServiceCollection AddSupplierInvoiceStatusServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectSupplierInvoiceStatus>();
            services.AddScoped<ISupplierInvoiceStatusRepository, SupplierInvoiceStatusRepository>();

            return services;
        }
    }
}

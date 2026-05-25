using Application.UseCases.PurchaseReceiptStatus;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class PurchaseReceiptStatusInjection
    {
        public static IServiceCollection AddPurchaseReceiptStatusServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectPurchaseReceiptStatus>();
            services.AddScoped<IPurchaseReceiptStatusRepository, PurchaseReceiptStatusRepository>();

            return services;
        }
    }
}

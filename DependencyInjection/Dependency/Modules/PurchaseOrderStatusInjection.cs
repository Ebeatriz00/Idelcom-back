using Application.UseCases.PurchaseOrderStatus;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class PurchaseOrderStatusInjection
    {
        public static IServiceCollection AddPurchaseOrderStatusServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectPurchaseOrderStatus>();
            services.AddScoped<IPurchaseOrderStatusRepository, PurchaseOrderStatusRepository>();

            return services;
        }
    }
}

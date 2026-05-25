using Application.UseCases.PurchaseOrderDetailStatus;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class PurchaseOrderDetailStatusInjection
    {
        public static IServiceCollection AddPurchaseOrderDetailStatusServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectPurchaseOrderDetailStatus>();
            services.AddScoped<IPurchaseOrderDetailStatusRepository, PurchaseOrderDetailStatusRepository>();

            return services;
        }
    }
}

using Application.UseCases.Operations.OperationsWorkOrderStatus;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsWorkOrderStatusInkection
    {
        public static IServiceCollection AddOperationsWorkOrderStatusInkection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectOperationsWorkOrderStatus>();
            services.AddScoped<GetByIdOperationsWorkOrderStatus>();

            services.AddScoped<IOperationsWorkOrderStatusRepository, OperationsWorkOrderStatusRepository>();

            return services;
        }
    }

}

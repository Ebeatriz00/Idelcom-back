using Application.UseCases.Operations.OperationsStatus;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsStatusInjection
    {
        public static IServiceCollection AddOperationsStatusInjection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectOperationsStatus>();
            services.AddScoped<GetByIdOperationsStatus>();

            services.AddScoped<IOperationsStatusRepository, OperationsStatusRepository>();
            return services;
        }
    }
}

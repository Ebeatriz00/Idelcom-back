using Application.UseCases.OperationsPersonnelMovement;
using Core.Interfaces.OperationsPersonnelMovement;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class OperationPersonnelMovementInjection
    {
        public static IServiceCollection AddOperationPersonnelMovementServices(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationPersonnelMovement>();
            services.AddScoped<GetAllOperationPersonnelMovement>();
            services.AddScoped<IOperationsPersonnelMovementRepository, OperationsPersonnelMovementRepository>();
            return services;
        }
    }
}

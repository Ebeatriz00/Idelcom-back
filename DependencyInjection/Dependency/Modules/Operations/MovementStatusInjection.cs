using Application.UseCases.Operations.MovementStatus;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class MovementStatusInjection
    {
        public static IServiceCollection AddMovementStatusInjection(this IServiceCollection services)
        {
            services.AddScoped<GetByIdMovementStatus>();
            services.AddScoped<GetSelectMovementStatus>();

            services.AddScoped<IMovementStatusRepository, MovementStatusRepository>();

            return services;
        }
    }
}

using Application.UseCases.Operations.ActivityComplexity;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class ActivityComplexityInjection
    {
        public static IServiceCollection AddActivityComplexityInjection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectActivityComplexity>();

            services.AddScoped<IActivityComplexityRepository, ActivityComplexityRepository>();

            return services;
        }
    }
}

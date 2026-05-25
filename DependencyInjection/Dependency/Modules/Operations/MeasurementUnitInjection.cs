using Application.UseCases.Operations.MeasurementUnit;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class MeasurementUnitInjection
    {
        public static IServiceCollection AddMeasurementUnitInjection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectMeasurementUnit>();

            services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();

            return services;
        }
    }
}

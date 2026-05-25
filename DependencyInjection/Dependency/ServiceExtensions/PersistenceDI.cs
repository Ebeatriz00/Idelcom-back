using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.ServiceExtensions
{
    public static class PersistenceDI
    {
        public static IServiceCollection AddPersistenceDI(this IServiceCollection services)
        {
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IDapperHelper, DapperHelper>();

            return services;
        }
    }
}

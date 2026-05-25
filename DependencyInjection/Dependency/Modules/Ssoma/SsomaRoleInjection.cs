using Application.UseCases.Ssoma;
using Core.Interfaces.Ssoma;
using Infrastructure.Repositories.Ssoma;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class SsomaRoleInjection
    {
        public static IServiceCollection AddSsomaRoleInjection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectSsomaRole>();

            services.AddScoped<ISsomaRoleRepository, SsomaRoleRepository>();

            return services;
        }
    }
}

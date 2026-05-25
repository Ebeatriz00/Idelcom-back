using Application.UseCases.SupportState;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class SupportStateInjection
    {
        public static IServiceCollection AddSupportStateInjection(this IServiceCollection services)
        {
            services.AddScoped<GetSelectSupportState>();
            services.AddScoped<GetByIdSupportState>();
            services.AddScoped<ISupportStateRepository, SupportStateRepository>();

            return services;
        }
    }
}

using Application.UseCases.Operations.Support;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class SupportInjection
    {
        public static IServiceCollection AddSupportServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSupport>();
            services.AddScoped<GetAllSupport>();
            services.AddScoped<GetByIdSupport>();
            services.AddScoped<UpdateSupport>();
            services.AddScoped<DeleteSupport>();

            services.AddScoped<ISupportRepository, SupportRepository>();

            return services;
        }
    }
}

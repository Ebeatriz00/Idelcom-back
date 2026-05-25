using Application.UseCases.PayableStatus;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class PayableStatusInjection
    {
        public static IServiceCollection AddPayableStatusServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectPayableStatus>();
            services.AddScoped<IPayableStatusRepository, PayableStatusRepository>();

            return services;
        }
    }
}

using Application.UseCases.Operations.AssignmentStatus;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class AssignmentStatusInjection
    {
        public static IServiceCollection AddAssignmentStatusInjection(this IServiceCollection services)
        {
            services.AddScoped<GetByIdAssignmentStatus>();
            services.AddScoped<GetSelectAssignmentStatus>();

            services.AddScoped<IAssignmentStatusRepository, AssignmentStatusRepository>();

            return services;
        }
    }

}

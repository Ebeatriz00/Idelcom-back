using Application.UseCases.Operations.AttendanceStatus;
using Core.Interfaces.Operations;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class AttendanceStatusInjection
    {
        public static IServiceCollection AddAttendanceStatusInjection(this IServiceCollection services)
        {

            services.AddScoped<GetByIdAttendanceStatus>();
            services.AddScoped<GetSelectAttendanceStatus>();

            services.AddScoped<IAttendanceStatusRepository, AttendanceStatusRepository>();

            return services;
        }
    }

}

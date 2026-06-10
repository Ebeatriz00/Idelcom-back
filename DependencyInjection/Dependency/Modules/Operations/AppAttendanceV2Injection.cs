using Application.DTOs.Operations.OperationsAttendance;
using Application.UseCases.Operations.OperationsAttendance;
using Application.Validators.Operations.OperationsAttendance;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class AppAttendanceV2Injection
    {
        public static IServiceCollection AddAppAttendanceV2Injection(this IServiceCollection services)
        {
            services.AddScoped<GetAppAttendanceDailyV2UseCase>();
            services.AddScoped<CreateAppAttendanceBatchV2UseCase>();
            services.AddScoped<SyncAppAttendanceBatchV2UseCase>();

            services.AddTransient<IValidator<AppAttendanceV2CreateDto>, AppAttendanceV2CreateValidator>();
            services.AddTransient<IValidator<AppAttendanceV2SyncDto>, SyncAppAttendanceBatchV2Validator>();

            services.AddScoped<IOperationsAttendanceV2Repository, OperationsAttendanceV2Repository>();

            return services;
        }
    }
}

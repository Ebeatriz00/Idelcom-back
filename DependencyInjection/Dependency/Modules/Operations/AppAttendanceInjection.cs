using Application.DTOs.Operations.OperationsAttendance;
using Application.UseCases.Operations.OperationsAttendance;
using Application.Validators.Operations.OperationsAttendance;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class AppAttendanceInjection
    {
        public static IServiceCollection AddAppAttendanceInjection(this IServiceCollection services)
        {
            // Casos de Uso
            services.AddScoped<GetAppAttendanceDailyUseCase>();
            services.AddScoped<CreateAppAttendanceBatchUseCase>();
            services.AddScoped<SyncAppAttendanceBatchUseCase>();
            services.AddScoped<GetAllAttendanceMatrixUseCase>();

            // Validadores
            services.AddTransient<IValidator<AppAttendanceCreateDto>, AppAttendanceCreateValidator>();
            services.AddTransient<IValidator<AppAttendanceSyncDto>, SyncAppAttendanceBatchValidator>();
            services.AddTransient<IValidator<AttendanceMatrixQueryDto>, AttendanceMatrixQueryValidator>();

            // Repositorio
            services.AddScoped<IOperationsAttendanceRepository, OperationsAttendanceRepository>();

            return services;
        }
    }
}

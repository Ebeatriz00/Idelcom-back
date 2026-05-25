using Application.DTOs.Operations.OperationsWorkOrderProgress;
using Application.UseCases.Operations.OperationsWorkOrderProgress;
using Application.Validators.Operations.OperationsWorkOrderProgress;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsWorkOrderProgressInjection
    {
        public static IServiceCollection AddOperationsWorkOrderProgressInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateAppOperationsWorkOrderProgress>();
            services.AddScoped<SyncAppOperationsWorkOrderProgress>();
            services.AddScoped<GetOperationsWorkOrderProgressList>();
            services.AddScoped<GetAppOperationsWorkOrderProgressPhotos>();

            services.AddTransient<IValidator<OperationsWorkOrderProgressCreateDto>, OperationsWorkOrderProgressCreateValidator>();
            services.AddTransient<IValidator<OperationsWorkOrderProgressSyncDto>, OperationsWorkOrderProgressSyncValidator>();

            services.AddScoped<IOperationsWorkOrderProgressRepository, OperationsWorkOrderProgressRepository>();
            services.AddScoped<IOperationsWorkOrderProgressPhotoRepository, OperationsWorkOrderProgressPhotoRepository>();

            return services;
        }
    }
}

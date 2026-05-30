using Application.DTOs.Operations.OperationsWorkOrderActivity;
using Application.UseCases.Operations.OperationsWorkOrderActivity;
using Application.Validators.Operations.OperationsWorkOrderActivity;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsWorkOrderActivityInjection
    {
        public static IServiceCollection AddOperationsWorkOrderActivityInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsWorkOrderActivity>();
            services.AddScoped<UpdateOperationsWorkOrderActivity>();
            services.AddScoped<GetAllOperationsWorkOrderActivity>();
            services.AddScoped<GetSelectOperationsWorkOrderActivity>();
            services.AddScoped<DeleteOperationsWorkOrderActivity>();
            services.AddScoped<GetAppActivitiesByResponsible>();
            services.AddScoped<CloneOperationsWorkOrderActivity>();

            services.AddTransient<IValidator<OperationsWorkOrderActivityCreateDto>, OperationsWorkOrderActivityCreateValidator>();
            services.AddTransient<IValidator<OperationsWorkOrderActivityUpdateDto>, OperationsWorkOrderActivityUpdateValidator>();

            services.AddScoped<IOperationsWorkOrderActivityRepository, OperationsWorkOrderActivityRepository>();

            return services;
        }
    }
}

using Application.DTOs.Operations.OperationsWorkOrder;
using Application.UseCases.Operations.OperationsWorkOrder;
using Application.Validators.Operations.OperationsWorkOrder;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsWorkOrderInjection
    {
        public static IServiceCollection AddOperationsWorkOrderInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsWorkOrder>();
            services.AddScoped<UpdateOperationsWorkOrder>();
            services.AddScoped<GetAllOperationsWorkOrder>();
            services.AddScoped<GetByIdOperationsWorkOrder>();
            services.AddScoped<DeleteOperationsWorkOrder>();
            services.AddScoped<GetOperationsWorkOrderProgressReport>();

            services.AddTransient<IValidator<OperationsWorkOrderCreateDto>, OperationsWorkOrderCreateValidator>();
            services.AddTransient<IValidator<OperationsWorkOrderUpdateDto>, OperationsWorkOrderUpdateValidator>();

            services.AddScoped<IOperationsWorkOrderRepository, OperationsWorkOrderRepository>();
            return services;
        }
    }
}

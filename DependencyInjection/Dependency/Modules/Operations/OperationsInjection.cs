using Application.DTOs.Operations.Operations;
using Application.UseCases.Operations.Operations;
using Application.Validators.Operations.Operations;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsInjection
    {
        public static IServiceCollection AddOperationsInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateOperations>();
            services.AddScoped<UpdateOperations>();
            services.AddScoped<GetAllOperations>();
            services.AddScoped<GetByIdOperations>();
            services.AddScoped<DeleteOperations>();

            services.AddTransient<IValidator<OperationsCreateDto>, OperationsCreateValidator>();
            services.AddTransient<IValidator<OperationsUpdateDto>, OperationsUpdateValidator>();

            services.AddScoped<IOperationsRepository, OperationsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

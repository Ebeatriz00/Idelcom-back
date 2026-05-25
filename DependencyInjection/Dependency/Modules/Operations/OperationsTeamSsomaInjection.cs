using Application.DTOs.OperationsTeamSsoma;
using Application.UseCases.OperationsTeamSsoma;
using Application.Validators.OperationsTeamSsoma;
using Core.Interfaces.Operations;
using Core.Interfaces.OperationsSsomaMovement;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Operations;
using Infrastructure.Repositories.OperationsSsomaMovement;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Operations
{
    public static class OperationsTeamSsomaInjection
    {
        public static IServiceCollection AddOperationsTeamSsomaInjection(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsTeamSsoma>();
            services.AddScoped<UpdateOperationsTeamSsoma>();
            services.AddScoped<GetByIdOperationsTeamSsoma>();
            services.AddScoped<GetListByProcessIdOperationsTeamSsoma>();
            services.AddScoped<ProcessSsomaAssignmentChange>();
            services.AddScoped<DeleteSsomaAssignment>();

            services.AddTransient<IValidator<OperationsTeamSsomaCreateDto>, OperationsTeamSsomaCreateValidator>();
            services.AddTransient<IValidator<OperationsTeamSsomaUpdateDto>, OperationsTeamSsomaUpdateValidator>();
            services.AddTransient<IValidator<ProcessSsomaAssignmentChangeDto>, ProcessSsomaAssignmentChangeValidator>();

            services.AddScoped<IOperationsTeamSsomaRepository, OperationsTeamSsomaRepository>();
            services.AddScoped<IOperationsTeamSsomaMovementRepository, OperationsTeamSsomaMovementRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

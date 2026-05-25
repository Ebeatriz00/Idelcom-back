using Application.UseCases.OperationsPersonnelAssignment;
using Core.Interfaces.OperationsPersonnelAssignment;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class OperationsPersonnelAssignmentInjection
    {
        public static IServiceCollection AddOperationsPersonnelAssignmentServices(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsPersonnelAssignment>();
            services.AddScoped<GetAllOperationsPersonnelAssignment>();
            services.AddScoped<GetByIdOperationsPersonnelAssignment>();
            services.AddScoped<UpdateOperationsPersonnelAssignment>();
            services.AddScoped<DeleteOperationsPersonnelAssignment>();
            services.AddScoped<IOperationsPersonnelAssignmentRepository, OperationsPersonnelAssignmentRepository>();
            return services;
        }
    }
}

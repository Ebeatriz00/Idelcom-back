using Application.UseCases.OperationsSupervisor;
using Core.Interfaces.OperationsSupervisor;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class OperationsSupervisorInjection
    {
        public static IServiceCollection AddOperationsSupervisorServices(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationsSupervisor>();
            services.AddScoped<GetAllOperationsSupervisor>();
            services.AddScoped<GetByIdOperationsSupervisor>();
            services.AddScoped<UpdateOperationsSupervisor>();
            services.AddScoped<DeleteOperationsSupervisor>();

            services.AddScoped<IOperationsSupervisorRepository, OperationsSupervisorRepository>();
            return services;
        }
    }
}

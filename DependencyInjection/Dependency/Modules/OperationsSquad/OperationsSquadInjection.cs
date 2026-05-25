using Application.UseCases.OperationsSquad;
using Application.UseCases.OperationsSquad.Application.UseCases.OperationsSquad;
using Core.Interfaces.OperationsSquad;
using Infrastructure.Persistence;
using Infrastructure.Repositories.OperationsSquad;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules.OperationsSquad
{
    public static class OperationsSquadInjection
    {
        public static IServiceCollection AddOperationsSquadServices(this IServiceCollection services)
        {
            services.AddScoped<CreateOperationSquad>();
            services.AddScoped<GetAllOperationsSquad>();
            services.AddScoped<GetByIdOperationsSquad>();
            services.AddScoped<UpdateOperationsSquad>();
            services.AddScoped<DeleteOperationsSquad>();

            services.AddScoped<IOperationsSquadRepository, OperationsSquadRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

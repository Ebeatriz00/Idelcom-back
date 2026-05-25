using Application.UseCases.WorkerStatus;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class WorkerStatusInjection
    {
        public static IServiceCollection AddWorkerStatusServices(this IServiceCollection services)
        {
            services.AddScoped<CreateWorkerStatus>();
            services.AddScoped<GetAllWorkerStatus>();
            services.AddScoped<GetByIdWorkerStatus>();
            services.AddScoped<UpdateWorkerStatus>();
            services.AddScoped<PatchWorkerStatus>();
            services.AddScoped<GetSelectWorkerStatus>();

            services.AddScoped<IWorkerStatusRepository, WorkerStatusRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

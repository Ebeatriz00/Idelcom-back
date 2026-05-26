using Application.DTOs.Worker;
using Application.UseCases.Worker;
using Application.Validators.Worker;
using Core.Interfaces;
using FluentValidation;
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
    public static class WorkerInjection
    {
        public static IServiceCollection AddWorkerServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateWorker>();
            services.AddScoped<GetAllWorker>();
            services.AddScoped<GetByIdWorker>();
            services.AddScoped<UpdateWorker>();
            services.AddScoped<PatchWorkerStatus>();
            services.AddScoped<GetSelectWorker>();
            services.AddScoped<GetSelectSalesWorker>();
            services.AddScoped<GetSelectProyectWorker>();
            services.AddScoped<GetSelectOperationsWorker>();
            services.AddScoped<GetSelectSquadWorker>();

            // Validators
            services.AddTransient<IValidator<WorkerCreateDto>, WorkerCreateValidator>();
            services.AddTransient<IValidator<WorkerUpdateDto>, WorkerUpdateValidator>();
            services.AddTransient<IValidator<WorkerStatusToggleDto>, WorkerStatusToggleValidator>();

            // Infra
            services.AddScoped<IWorkerRepository, WorkerRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}


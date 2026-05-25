using Application.DTOs.Tasks;
using Application.UseCases.Tasks;
using Application.Validators.Tasks;
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
    public static class TasksInjection
    {
        public static IServiceCollection AddTasksServices(this IServiceCollection services)
        {
            services.AddScoped<CreateTasks>();
            services.AddScoped<GetAllTasks>();
            services.AddScoped<GetAllTasksProject>();
            services.AddScoped<GetTasksById>();
            services.AddScoped<UpdateTasks>();
            services.AddScoped<PatchTasksStatus>();
            services.AddScoped<PatchTasksCompleted>();
            services.AddScoped<PatchTasksChangeState>();
            services.AddScoped<PatchTasksPriorityState>();
            services.AddScoped<GetSelectTasks>();
            services.AddScoped<DeleteTask>();
            services.AddScoped<DeleteTasksProject>();


            services.AddTransient<IValidator<TasksCreateDto>, TasksCreateValidator>();
            services.AddTransient<IValidator<TasksUpdateDto>, TasksUpdateValidator>();
            services.AddTransient<IValidator<TasksStatusToggleDto>, TasksStatusToggleValidator>();
            services.AddTransient<IValidator<TasksCompletedDto>, TasksCompletedValidator>();
            services.AddTransient<IValidator<TasksChangeStateDto>, TasksChangeStateValidator>();
            services.AddTransient<IValidator<TaskChangePriorityStateDto>, TasksChangePriorityStateValidator>();
            services.AddTransient<IValidator<TaksOpporDeleteDto>, TasksOpporDeleteValidator>();
            services.AddTransient<IValidator<TasksProjectDeleteDto>, TasksProjectDeleteValidator>();

            services.AddScoped<ITasksRepository, TasksRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.SubTasks;
using Application.DTOs.SubTasks.Application.DTOs.SubTasks;
using Application.UseCases.SubTasks;
using Application.Validators.SubTasks;
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
    public static class SubTasksInjection
    {
        public static IServiceCollection AddSubTasksServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSubTasks>();
            services.AddScoped<UpdateSubTasks>();
            services.AddScoped<GetAllSubTasks>(); 
            services.AddScoped<GetSubTasksById>();
            services.AddScoped<DeleteSubTasks>();

            services.AddTransient<IValidator<SubTasksCreateDto>, SubTasksCreateValidator>();
            services.AddTransient<IValidator<SubTasksUpdateDto>, SubTasksUpdateValidator>();
            services.AddTransient<IValidator<SubTasksDeleteDto>, SubTasksDeleteValidator>();

             
            services.AddScoped<ISubTasksRepository, SubTasksRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>(); 


            return services;
        }
    }
}

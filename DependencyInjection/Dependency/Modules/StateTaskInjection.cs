using Application.DTOs.StateTask;
using Application.UseCases.StateTask;
using Application.Validators.StateTask;
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
    public static class StateTaskInjection
    {
        public static IServiceCollection AddStateTaskServices(this IServiceCollection services)
        {

            services.AddScoped<CreateStateTask>();
            services.AddScoped<GetAllStateTask>();
            services.AddScoped<GetSelectStateTask>();
            services.AddScoped<GetSelectNormalStateTask>();
            services.AddScoped<GetByIdStateTask>();
            services.AddScoped<UpdateStateTask>();
            services.AddScoped<PatchStateTask>();

            services.AddTransient<IValidator<StateTaskCreateDto>, StateTaskCreateValidator>();
            services.AddTransient<IValidator<StateTaskUpdateDto>, StateTaskUpdateValidator>();
            services.AddTransient<IValidator<StateTaskStatusToggleDto>, StateTaskStatusToggleValidator>();

            services.AddScoped<IStateTaskRepository, StateTaskRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

using Application.DTOs.ActivityState;
using Application.UseCases.ActivityState;
using Application.Validators.ActivityState;
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
    public static class ActivityStateInjection
    {
        public static IServiceCollection AddActivityStateServices(this IServiceCollection services)
        {
            // UseCases
             services.AddScoped<CreateActivityState>();
             services.AddScoped<GetAllActivityState>();
             services.AddScoped<GetSelectActivityState>();
             services.AddScoped<GetSelectSearchActivityState>();
             services.AddScoped<GetByIdActivityState>();
             services.AddScoped<UpdateActivityState>();
             services.AddScoped<PatchActivityState>();
            // Validators
             services.AddTransient<IValidator<ActivityStateCreateDto>, ActivityStateCreateValidator>();
             services.AddTransient<IValidator<ActivityStateUpdateDto>, ActivityStateUpdateValidator>();
             services.AddTransient<IValidator<ActivityStateStatusToggleDto>, ActivityStateToggleValidator>();
            // Infra
             services.AddScoped<IActivityStateRepository, ActivityStateRepository>();
             services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

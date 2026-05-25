using Application.DTOs.ActivityType;
using Application.UseCases.ActivityType;
using Application.Validators.ActivityType;
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
    public static class ActivityTypeInjection
    {
        public static IServiceCollection AddActivityTypeServices(this IServiceCollection services)
        {
            // UseCases
             services.AddScoped<CreateActivityType>();
             services.AddScoped<GetAllActivityType>();
             services.AddScoped<GetSelectActivityType>();;
             services.AddScoped<GetByIdActivityType>();
             services.AddScoped<UpdateActivityType>();
             services.AddScoped<PatchActivityType>();
            // Validators
             services.AddTransient<IValidator<ActivityTypeCreateDto>, ActivityTypeCreateValidator>();
             services.AddTransient<IValidator<ActivityTypeUpdateDto>, ActivityTypeUpdateValidator>();
             services.AddTransient<IValidator<ActivityTypeStatusToggleDto>, ActivityTypeToggleValidator>();
            // Infra
             services.AddScoped<IActivityTypeRepository, ActivityTypeRepository>();
             services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

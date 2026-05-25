using Application.DTOs.Activity;
using Application.UseCases.Activity;
using Application.Validators.Activity;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class ActivityInjection 
    {
        public static IServiceCollection AddActivityServices(this IServiceCollection services)
        {
            services.AddScoped<CreateActivityOppor>();
            services.AddScoped<CreateActivityProject>();
            services.AddScoped<DeleteActivityOppor>();
            services.AddScoped<DeleteActivityProject>();

            services.AddScoped<PatchChangeActivityStateOppor>();
            services.AddScoped<PatchChangeActivityPriorityOppor>();

            services.AddTransient<IValidator<ActivityOpporCreateDto>, ActivityOpporCreateValidator>();
            services.AddTransient<IValidator<ActivityOpporDeleteDto>, ActivityOpporDeleteValidator>();
            services.AddTransient<IValidator<ActivityDeleteProjectDto>, ActivityProjectDeleteValidator>();

            services.AddTransient<IValidator<ActivityStateOpporDto>, ActivityOpporChangeStateValidator>();
            services.AddTransient<IValidator<ActivityPriorityOpporDto>, ActivityOpporChangePriorityValidator>();

            services.AddScoped<IActivityRepository, ActivityRepository>();
            return services;
        }
    }
}

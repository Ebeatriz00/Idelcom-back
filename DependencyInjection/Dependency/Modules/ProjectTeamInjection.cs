using Application.DTOs.ProjectTeam;
using Application.UseCases.ProjectTeam;
using Application.Validators.ProjectTeam;
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
    public static class ProjectTeamInjection
    {
        public static IServiceCollection AddProjectTeamServices(this IServiceCollection services)
        {
            services.AddScoped<CreateProjectTeam>();
            services.AddScoped<GetAllProjectTeam>();
            services.AddScoped<DeleteProjectTeam>();
            
            services.AddTransient<IValidator<ProjectTeamCreateDto>, ProjectTeamCreateValidator>();
            services.AddTransient<IValidator<ProjectTeamDeleteDto>, ProjectTeamDeleteValidator>();  
            services.AddScoped<IProjectTeamRepository, ProjectTeamRepository>();


            return services;
        }
    }
}

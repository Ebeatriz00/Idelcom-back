using Application.DTOs.JobTitle;
using Application.UseCases.JobTitle;
using Application.Validators.JobTitle;
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
    public static class JobTitleInjection
    {
        public static IServiceCollection AddJobTitleServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateJobTitle>();
            services.AddScoped<GetAllJobTitle>();
            services.AddScoped<GetJobTitleById>();
            services.AddScoped<UpdateJobTitle>();
            services.AddScoped<PatchJobTitleStatus>();
            services.AddScoped<GetSelectJobTitle>();

            // Validators
            services.AddTransient<IValidator<JobTitleCreateDto>, JobTitleCreateValidator>();
            services.AddTransient<IValidator<JobTitleUpdateDto>, JobTitleUpdateValidator>();
            services.AddTransient<IValidator<JobTitleStatusToggleDto>, JobTitleStatusToggleValidator>();

            // Infrastructure
            services.AddScoped<IJobTitleRepository, JobTitleRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.DTOs.LeadsStatus;
using Application.UseCases.LeadsStatus;
using Application.Validators.LeadsStatus;
using Core.Entities;
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
    public static class LeadsStatusInjection
    {
        public static IServiceCollection AddLeadsStatusServices(this IServiceCollection services)
        {
            services.AddScoped<CreateLeadsStatus>();
            services.AddScoped<GetAllLeadsStatus>();
            services.AddScoped<GetByIdLeadsStatus>();
            services.AddScoped<UpdateLeadsStatus>();
            services.AddScoped<PatchLeadsStatus>();
            services.AddScoped<GetSelectLeadsStatus>();

            services.AddTransient<IValidator<LeadsStatusCreateDto>, LeadsStatusCreateValidator>();
            services.AddTransient<IValidator<LeadsStatusUpdateDto>, LeadsStatusUpdateValidator>();
            services.AddTransient<IValidator<LeadsStatusStatusToggleDto>, LeadsStatusStatusToggleValidator>();


            services.AddScoped<ILeadsStatusRepository, LeadsStatusRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

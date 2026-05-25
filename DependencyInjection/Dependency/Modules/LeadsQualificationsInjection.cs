using Application.DTOs.LeadsQualifications;
using Application.UseCases.LeadsQualifications;
using Application.Validators.LeadsQualifications;
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
    public static class LeadsQualificationsInjection
    {
        public static IServiceCollection AddLeadsQualificationsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateLeadsQualifications>();
            services.AddScoped<GetAllLeadsQualifications>();
            services.AddScoped<GetLeadsQualificationsById>();
            services.AddScoped<UpdateLeadsQualifications>();
            services.AddScoped<PatchLeadsQualificationsStatus>();
            services.AddScoped<GetSelectLeadsQualifications>();

            services.AddTransient<IValidator<LeadsQualificationsCreateDto>, LeadsQualificationsCreateValidator>();
            services.AddTransient<IValidator<LeadsQualificationsUpdateDto>, LeadsQualificationsUpdateValidator>();
            services.AddTransient<IValidator<LeadsQualificationsStatusToggleDto>, LeadsQualificationsStatusToggleValidator>();

            services.AddScoped<ILeadsQualificationsRepository, LeadsQualificationsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

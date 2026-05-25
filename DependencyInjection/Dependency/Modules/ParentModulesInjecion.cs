using Application.DTOs.ParentModules;
using Application.UseCases.ParentModules;
using Application.Validators.ParentModules;
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
    public static class ParentModulesInjecion
    {
        public static IServiceCollection AddParentModulesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateParentModules>();
            services.AddScoped<GetAllParentModules>();
            services.AddScoped<GetByIdParentModules>();
            services.AddScoped<UpdateParentModules>();
            services.AddScoped<PatchParentModulesStatus>();
            // Validators
            services.AddTransient<IValidator<ParentModulesCreateDto>, ParentModulesCreateValidator>();
            services.AddTransient<IValidator<ParentModulesUpdateDto>, ParentModulesUpdateValidator>();
            services.AddTransient<IValidator<ParentModulesStatusToogleDto>, ParentModulesStatusToogleValidator>();
            // Infra
            services.AddScoped<IParentModulesRepository, ParentModulesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

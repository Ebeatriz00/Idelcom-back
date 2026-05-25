using Application.DTOs.MovementTypes;
using Application.UseCases.MovementTypes;
using Application.Validators.MovementTypes;
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
    public static class MovementTypesInjection
    {
        public static IServiceCollection AddMovementTypesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMovementTypes>();
            services.AddScoped<GetAllMovementTypes>();
            services.AddScoped<GetMovementTypesById>();
            services.AddScoped<UpdateMovementTypes>();
            services.AddScoped<PatchMovementTypesStatus>();
            services.AddScoped<GetSelectMovementTypes>();

            services.AddTransient<IValidator<MovementTypesCreateDto>, MovementTypesCreateValidator>();
            services.AddTransient<IValidator<MovementTypesUpdateDto>, MovementTypesUpdateValidator>();
            services.AddTransient<IValidator<MovementTypesStatusToggleDto>, MovementTypesStatusToggleValidator>();

            services.AddScoped<IMovementTypesRepository, MovementTypesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

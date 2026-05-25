using Application.DTOs.MovSunat;
using Application.UseCases.MovSunat;
using Application.Validators.MovSunat;
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
    public static class MovSunatInjection
    {
        public static IServiceCollection AddMovSunatServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMovSunat>();
            services.AddScoped<GetAllMovSunat>();
            services.AddScoped<GetMovSunatById>();
            services.AddScoped<UpdateMovSunat>();
            services.AddScoped<PatchMovSunatStatus>();
            services.AddScoped<GetSelectMovSunat>();

            services.AddTransient<IValidator<MovSunatCreateDto>, MovSunatCreateValidator>();
            services.AddTransient<IValidator<MovSunatUpdateDto>, MovSunatUpdateValidator>();
            services.AddTransient<IValidator<MovSunatStatusToggleDto>, MovSunatStatusToggleValidator>();

            services.AddScoped<IMovSunatRepository, MovSunatRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();


            return services;
        }
    }
}

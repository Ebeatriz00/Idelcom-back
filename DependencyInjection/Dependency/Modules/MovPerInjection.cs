using Application.DTOs.MovPer;
using Application.UseCases.MovPer;
using Application.Validators.MovPer;
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
    public static class MovPerInjection
    {
        public static IServiceCollection AddMovPerServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMovPer>();
            services.AddScoped<GetAllMovPer>();
            services.AddScoped<GetMovPerById>();
            services.AddScoped<UpdateMovPer>();
            services.AddScoped<PatchMovPerStatus>();
            services.AddScoped<GetSelectMovPer>();

            services.AddTransient<IValidator<MovPerCreateDto>, MovPerCreateValidator>();
            services.AddTransient<IValidator<MovPerUpdateDto>, MovPerUpdateValidator>();
            services.AddTransient<IValidator<MovPerStatusToggleDto>, MovPerStatusToggleValidator>();

            services.AddScoped<IMovPerRepository, MovPerRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

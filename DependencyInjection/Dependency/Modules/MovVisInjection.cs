using Application.DTOs.MovVis;
using Application.UseCases.MovVis;
using Application.Validators.MovVis;
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
    public static class MovVisInjection
    {
        public static IServiceCollection AddMovVisServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMovVis>();
            services.AddScoped<GetAllMovVis>();
            services.AddScoped<GetMovVisById>();
            services.AddScoped<UpdateMovVis>();
            services.AddScoped<PatchMovVisStatus>();
            services.AddScoped<GetSelectMovVis>();

            services.AddTransient<IValidator<MovVisCreateDto>, MovVisCreateValidator>();
            services.AddTransient<IValidator<MovVisUpdateDto>, MovVisUpdateValidator>();
            services.AddTransient<IValidator<MovVisStatusToggleDto>, MovVisStatusToggleValidator>();

            services.AddScoped<IMovVisRepository, MovVisRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

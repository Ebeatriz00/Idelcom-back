using Application.DTOs.MovClas;
using Application.UseCases.MovClas;
using Application.Validators.MovClas;
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
    public static class MovClasInjection
    {
        public static IServiceCollection AddMovClasServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMovClas>();
            services.AddScoped<GetAllMovClas>();
            services.AddScoped<GetMovClasById>();
            services.AddScoped<UpdateMovClas>();
            services.AddScoped<PatchMovClasStatus>();
            services.AddScoped<GetSelectMovClas>();

            services.AddTransient<IValidator<MovClasCreateDto>, MovClasCreateValidator>();
            services.AddTransient<IValidator<MovClasUpdateDto>, MovClasUpdateValidator>();
            services.AddTransient<IValidator<MovClasStatusToggleDto>, MovClasStatusToggleValidator>();

            services.AddScoped<IMovClasRepository, MovClasRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

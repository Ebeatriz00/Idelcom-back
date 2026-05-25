using Application.DTOs.MovOper;
using Application.UseCases.MovOper;
using Application.Validators.MovOper;
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
    public static class MovOperInjection
    {
        public static IServiceCollection AddMovOperServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMovOper>();
            services.AddScoped<GetAllMovOper>();
            services.AddScoped<GetMovOperById>();
            services.AddScoped<UpdateMovOper>();
            services.AddScoped<PatchMovOperStatus>();
            services.AddScoped<GetSelectMovOper>();

            services.AddTransient<IValidator<MovOperCreateDto>, MovOperCreateValidator>();
            services.AddTransient<IValidator<MovOperUpdateDto>, MovOperUpdateValidator>();
            services.AddTransient<IValidator<MovOperStatusToggleDto>, MovOperStatusToggleValidator>();

            services.AddScoped<IMovOperRepository, MovOperRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

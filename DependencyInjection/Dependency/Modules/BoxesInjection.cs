using Application.DTOs.Boxes;
using Application.UseCases.Boxes;
using Application.Validators.Boxes;
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
    public static class BoxesInjection
    {
        public static IServiceCollection AddBoxesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateBoxes>();
            services.AddScoped<GetAllBoxes>();
            services.AddScoped<GetBoxesById>();
            services.AddScoped<UpdateBoxes>();
            services.AddScoped<PatchBoxesStatus>();
            services.AddScoped<GetSelectBoxes>();


            services.AddTransient<IValidator<BoxesCreateDto>, BoxesCreateValidator>();
            services.AddTransient<IValidator<BoxesUpdateDto>, BoxesUpdateValidator>();
            services.AddTransient<IValidator<BoxesStatusToggleDto>, BoxesStatusToggleValidator>();

            services.AddScoped<IBoxesRepository, BoxesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

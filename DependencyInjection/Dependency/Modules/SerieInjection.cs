using Application.DTOs.Series;
using Application.UseCases.Series;
using Application.Validators.Series;
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
    public static class SeriesInjection
    {
        public static IServiceCollection AddSeriesServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateSeries>();
            services.AddScoped<GetAllSeries>();
            services.AddScoped<GetSeriesById>();
            services.AddScoped<UpdateSeries>();
            services.AddScoped<PatchSeriesStatus>();
            services.AddScoped<GetSelectSeries>();

            // Validators
            services.AddTransient<IValidator<SeriesCreateDto>, SeriesCreateValidator>();
            services.AddTransient<IValidator<SeriesUpdateDto>, SeriesUpdateValidator>();
            services.AddTransient<IValidator<SeriesStatusToggleDto>, SeriesStatusToggleValidator>();

            // Infrastructure
            services.AddScoped<ISeriesRepository, SeriesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

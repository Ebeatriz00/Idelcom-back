
using Application.UseCases.Location;
using Core.Interfaces;
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
    public static class LocationInjection
    {
        public static IServiceCollection AddLocationServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectDepartment>();
            services.AddScoped<GetSelectProvince>();
            services.AddScoped<GetSelectDistrict>();

            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

using Application.UseCases.Observations;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class ObservationsInjection
    {
        public static IServiceCollection AddObservationsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateObservations>();
            services.AddScoped<GetAllObservations>();
            services.AddScoped<GetObservationsById>();
            services.AddScoped<UpdateObservation>();
            services.AddScoped<UpdateObservationDate>();
            services.AddScoped<GetAllByProjectObservations>();
            services.AddScoped<GetAllByHiringObservations>();
            services.AddScoped<IObservationsRepository, ObservationsRepository>();


            return services;
        }
    }
}

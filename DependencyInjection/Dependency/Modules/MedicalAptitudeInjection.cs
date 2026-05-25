using Application.UseCases.MedicalAptitude;
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
    public static class MedicalAptitudeInjection
    {
        public static IServiceCollection AddMedicalAptitudeServices(this IServiceCollection services)
        {
            services.AddScoped<CreateMedicalAptitude>();
            services.AddScoped<GetAllMedicalAptitude>();
            services.AddScoped<GetByIdMedicalAptitude>();
            services.AddScoped<UpdateMedicalAptitude>();
            services.AddScoped<PatchMedicalAptitude>();
            services.AddScoped<GetSelectMedicalAptitude>();

            services.AddScoped<IMedicalAptitudeRepository, MedicalAptitudeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

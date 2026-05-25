using Application.DTOs.Sector;
using Application.UseCases.Sector;
using Application.Validators.Sector;
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
    public static class SectorInjection
    {
        public static IServiceCollection AddSectorServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSector>();
            services.AddScoped<GetAllSector>();
            services.AddScoped<GetSectorById>();
            services.AddScoped<UpdateSector>();
            services.AddScoped<PatchSectorStatus>();
            services.AddScoped<GetSelectSector>();

            services.AddTransient<IValidator<SectorCreateDto>, SectorCreateValidator>();
            services.AddTransient<IValidator<SectorUpdateDto>, SectorUpdateValidator>();
            services.AddTransient<IValidator<SectorStatusToggleDto>, SectorStatusToggleValidator>();


            services.AddScoped<ISectorRepository, SectorRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

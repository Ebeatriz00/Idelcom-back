using Application.DTOs.PMVis;
using Application.UseCases.PMVis;
using Application.Validators.PMVis;
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
    public static class PMVisInjection
    {
        public static IServiceCollection AddPMVisServices(this IServiceCollection services)
        {
            services.AddScoped<CreatePMVis>();
            services.AddScoped<GetAllPMVis>();
            services.AddScoped<GetPMVisById>();
            services.AddScoped<UpdatePMVis>();
            services.AddScoped<PatchPMVisStatus>();
            services.AddScoped<GetSelectPMVis>();

            services.AddTransient<IValidator<PMVisCreateDto>, PMVisCreateValidator>();
            services.AddTransient<IValidator<PMVisUpdateDto>, PMVisUpdateValidator>();
            services.AddTransient<IValidator<PMVisStatusToggleDto>, PMVisStatusToggleValidator>();

            services.AddScoped<IPMVisRepository, PMVisRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

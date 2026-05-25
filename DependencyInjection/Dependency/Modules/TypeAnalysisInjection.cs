using Application.UseCases.TypeAnalysis;
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
    public static class TypeAnalysisInjection
    {
        public static IServiceCollection AddTypeAnalysisServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectTypeAnalysis>();
            services.AddScoped<ITypeAnalysisRepository, TypeAnalysisRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

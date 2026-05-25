using Application.UseCases.SupplierGroups;
using Application.UseCases.TypeSuppliers;
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
    public static class TypeSuppliersInjection
    {
        public static IServiceCollection AddTypeSuppliersServices(this IServiceCollection services)
        {
            services.AddScoped<GetSelectTypeSuppliers>();

            services.AddScoped<ITypeSuppliersRepository, TypeSuppliersRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

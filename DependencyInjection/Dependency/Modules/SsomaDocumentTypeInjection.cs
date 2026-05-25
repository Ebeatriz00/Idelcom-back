using Application.UseCases.SsomaDocumentType;
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
    public static class SsomaDocumentTypeInjection
    {
        public static IServiceCollection AddSsomaDocumentTypeServices(this IServiceCollection services)
        {
            services.AddScoped<CreateSsomaDocumentType>();
            services.AddScoped<GetAllSsomaDocumentType>();
            services.AddScoped<GetByIdSsomaDocumentType>();
            services.AddScoped<UpdateSsomaDocumentType>();
            services.AddScoped<PatchSsomaDocumentType>();
            services.AddScoped<GetSelectSsomaDocumentType>();

            services.AddScoped<ISsomaDocumentTypeRepository, SsomaDocumentTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

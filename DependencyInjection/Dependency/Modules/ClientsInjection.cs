using Application.DTOs.Clients;
using Application.UseCases.Clients;
using Application.Validators.Clients;
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
    public static class ClientsInjection
    {
        public static IServiceCollection AddClientsServices(this IServiceCollection services)
        {
            services.AddScoped<CreateClients>();
            services.AddScoped<GetAllClients>();
            services.AddScoped<GetAllHistoryClients>();
            services.AddScoped<GetByIdClients>();
            services.AddScoped<UpdateClients>();
            services.AddScoped<UpdateChangeSalesClients>();
            services.AddScoped<PatchClients>();
            services.AddScoped<GetSelectClients>();
            services.AddScoped<ExistsContacts>();
            services.AddScoped<GetClientsDetail>();

            services.AddTransient<IValidator<ClientsCreateDto>, ClientsCreateValidator>();
            services.AddTransient<IValidator<ClientsUpdateDto>, ClientsUpdateValidator>();

            services.AddTransient<IValidator<ClientsUpdateChangeSalesDto>, ClientsUpdateChangeSalesValidator>();
            services.AddTransient<IValidator<ClientsStatusToggleDto>, ClientsStatusToggleValidator>();

            services.AddScoped<IClientsRepository, ClientsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

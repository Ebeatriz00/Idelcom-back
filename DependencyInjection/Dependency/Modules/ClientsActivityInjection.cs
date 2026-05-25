using Application.DTOs.ClientsActivity;
using Application.UseCases.ClientsActivity;
using Application.Validators.ClientsActivity;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class ClientsActivityInjection
    {
        public static IServiceCollection AddClientsActivityServices(this IServiceCollection services)
        {
            services.AddScoped<CreateClientsActivity>();
            services.AddScoped<GetClientsActivities>();
            services.AddScoped<DeleteClientsActivity>();
            services.AddScoped<UpdateActivityStatus>();

            services.AddTransient<IValidator<ClientActivityCreateDto>, ClientsActivityCreateValidator>();
            services.AddTransient<IValidator<ClientsActivityDeleteDto>, ClientsActivityDeleteValidator>();
            services.AddTransient<IValidator<ClientActivityUpdateDto>, ClientsActivityUpdateValidator>();



            services.AddScoped<IClientsActivityRepository, ClientsActivityRepository>();

            return services;
        }
    }
}

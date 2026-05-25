using Application.DTOs.ContactsCrm;
using Application.UseCases.ConctactsCrm;
using Application.Validators.ContactsCrm;
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
    public static class ContactsCrmInjection
    {
        public static IServiceCollection AddContactsCrmServices(this IServiceCollection services)
        {
            services.AddScoped<CreateContactsCrm>();
            services.AddScoped<GetAllContactsCrm>();
            services.AddScoped<GetContactsCrmById>();
            services.AddScoped<UpdateContactsCrm>();
            services.AddScoped<PatchContactsCrmStatus>();
            services.AddScoped<GetSelectContactsCrm>();

            services.AddTransient<IValidator<ContactsCrmCreateDto>, ContactsCrmCreateValidator>();
            services.AddTransient<IValidator<ContactsCrmUpdateDto>, ContactsCrmUpdateValidator>();
            services.AddTransient<IValidator<ContactsCrmStatusToggleDto>, ContactsCrmStatusToggleValidator>();

            services.AddScoped<IContactsCrmRepository, ContactsCrmRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

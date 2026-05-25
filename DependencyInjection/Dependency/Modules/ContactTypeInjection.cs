using Application.DTOs.ContactType;
using Application.UseCases.ContactType;
using Application.Validators.ContactType;
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
    public static class ContactTypeInjection
    {
        public static IServiceCollection AddContactTypeServices(this IServiceCollection services)
        {
            services.AddScoped<CreateContactType>();
            services.AddScoped<GetAllContactType>();
            services.AddScoped<GetContactTypeById>();
            services.AddScoped<UpdateContactType>();
            services.AddScoped<PatchContactTypeStatus>();
            services.AddScoped<GetSelectContactType>();

            services.AddTransient<IValidator<ContactTypeCreateDto>, ContactTypeCreateValidator>();
            services.AddTransient<IValidator<ContactTypeUpdateDto>, ContactTypeUpdateValidator>();
            services.AddTransient<IValidator<ContactTypeStatusToggleDto>, ContactTypeStatusToggleValidator>();

            services.AddScoped<IContactTypeRepository, ContactTypeRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

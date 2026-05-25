

using Application.DTOs.UsersPreferences;
using Application.UseCases.Users;
using Application.UseCases.UsersPreferences;
using Application.Validators.UsersPreferences;
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
    public static class UsersPreferencesInjection
    {
        public static IServiceCollection AddUsersPreferencesServices(this IServiceCollection services)
        {

            services.AddScoped<GetNotifByIdUsersPrefe>();
            services.AddScoped<GetPrefeByIdUsersPrefe>();
            services.AddScoped<GetSeattingByIdUsersPrefe>();

            services.AddScoped<UpdateNotiUsersPrefe>();
            services.AddScoped<UpdatePrefeUsersPrefe>();
            services.AddScoped<UpdateSettingUsersPrefe>();

            services.AddTransient<IValidator<UsersPrefeNotiUpdateDto>, UsersPreferencesUpdateNotifValidator>();
            services.AddTransient<IValidator<UsersPrefeUpdateDto>, UsersPreferencesUpdateValidator>();
            services.AddTransient<IValidator<UsersPrefeSettingUpdateDto>, UsersPreferencesSettingUpdateValidator>();


            services.AddScoped<IUsersPreferencesRepository, UsersPreferencesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

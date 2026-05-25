using Application.DTOs.Users;
using Application.UseCases.Users;
using Application.Validators.Users;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class UsersInjection
    {
        public static IServiceCollection AddUsersServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateUsers>();
            services.AddScoped<GetAllUsers>();
            services.AddScoped<GetByIdUsers>();
            services.AddScoped<GetByIdUsersSetting>();
            services.AddScoped<GetExistsCodeUsers>();
            services.AddScoped<UpdateUsers>();
            services.AddScoped<UpdateSettingUsers>();
            services.AddScoped<PatchUsersStatus>();
            services.AddScoped<UpdatePasswordChange>();
            // Validators
            services.AddTransient<IValidator<UsersCreateDto>, UsersCreateValidator>();
            services.AddTransient<IValidator<UsersUpdateDto>, UsersUpdateValidator>();
            services.AddTransient<IValidator<UsersSettingUpdateDto>, UsersSettingUpdateValidator>();
            services.AddTransient<IValidator<UsersStatusToggleDto>, UsersStatusToggleValidator>();
            services.AddTransient<IValidator<UsersPasswordChangeDto>, UsersPasswordChangeValidator>();
            // Infra
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IPasswordService, HmacPasswordService>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

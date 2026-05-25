using Application.DTOs.ProfilesPermissions;
using Application.UseCases.ModulePermission;
using Application.UseCases.ProfilesPermissions;
using Application.Validators.ProfilesPermissions;
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
    public static class ProfilesPermissionsInjection
    {
        public static IServiceCollection AddProfilesPermissionsServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateProfilesPermissions>();
            services.AddScoped<GetAllProfilesPermissions>();
            services.AddScoped<GetByIdProfilesPermissions>();
            services.AddScoped<UpdateProfilesPermissions>();
            services.AddScoped<PatchProfilesPermissionsStatus>();
            // Validators
            services.AddTransient<IValidator<ProfilesPermissionsCreateDto>, ProfilesPermissionsCreateValidator>();
            services.AddTransient<IValidator<ProfilesPermissionsUpdateDto>, ProfilesPermissionsUpdateValidator>();
            services.AddTransient<IValidator<ProfilesPermissionsStatusToggleDto>, ProfilesPermissionsStatusToggleValidator>();
            // Infra
            services.AddScoped<IProfilesPermissionsRepository, ProfilesPermissionsRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

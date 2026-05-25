using Application.DTOs.Profiles;
using Application.UseCases.Profiles;
using Application.Validators.Profiles;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class ProfilesInjection
    {
        public static IServiceCollection AddProfilesServices(this IServiceCollection services)
        {
            // UseCases
            services.AddScoped<CreateProfiles>();
            services.AddScoped<GetAllProfiles>();
            services.AddScoped<GetSelectProfiles>();
            services.AddScoped<GetByIdProfiles>();
            services.AddScoped<UpdateProfiles>();
            services.AddScoped<PatchProfilesStatus>();
            // Validators
            services.AddTransient<IValidator<ProfilesCreateDto>, ProfilesCreateValidator>();
            services.AddTransient<IValidator<ProfilesUpdateDto>, ProfilesUpdateValidator>();
            services.AddTransient<IValidator<ProfilesStatusToggleDto>, ProfilesStatusToggleValidator>();
            // Infra
            services.AddScoped<IProfilesRepository, ProfilesRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}

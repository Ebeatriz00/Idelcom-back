using Application.DTOs.ReasonRejection;
using Application.UseCases.ReasonRejection;
using Application.Validators.ReasonRejection;
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
    public static class ReasonRejectionInjection
    {
        public static IServiceCollection AddReasonRejectionServices(this IServiceCollection services)
        {
            services.AddScoped<CreateReasonRejection>();
            services.AddScoped<GetAllReasonRejection>();
            services.AddScoped<GetByIdReasonRejection>();
            services.AddScoped<UpdateReasonRejection>();
            services.AddScoped<PatchReasonRejection>();
            services.AddScoped<GetSelectReasonRejection>();

            services.AddTransient<IValidator<ReasonRejectionCreateDto>, ReasonRejectionCreateValidator>();
            services.AddTransient<IValidator<ReasonRejectionUpdateDto>, ReasonRejectionUpdateValidator>();
            services.AddTransient<IValidator<ReasonRejectionStatusToggleDto>, ReasonRejectionStatusToggleValidator>();


            services.AddScoped<IReasonRejectionRepository, ReasonRejectionRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}

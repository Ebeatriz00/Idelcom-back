using Application.DTOs.FileTracking;
using Application.UseCases.FileTracking;
using Application.Validators.FileTracking;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection.Dependency.Modules
{
    public static class FileTrackingInjection
    {
        public static IServiceCollection AddFileTrackingServices(this IServiceCollection services)
        {
            services.AddScoped<CreateFileTrackingOpper>();
            services.AddScoped<CreateFileTrackingProject>();
            services.AddScoped<DeleteFileTrackingOpper>();
            services.AddScoped<DeleteFileTrackingProject>();

            services.AddTransient<IValidator<FileTrackingOpporCreateDto>, FileTrackingOpporCreateValidator>();
            services.AddTransient<IValidator<FileTrackingProjectCreateDto>, FileTrackingProjectCreateValidator>();
            services.AddTransient<IValidator<FileTrackingOpperDeleteDto>, FileTrackingOpperDeleteValidator>();
            services.AddTransient<IValidator<FileTrackingProjectDeleteDto>, FileTrackingProjectDeleteValidator>();

            services.AddScoped<IFileTrackingRepository, FileTrackingRepository>();
            return services;
        }
    }
}

using Application.Services.Files;
using Core.Interfaces;
using Core.Options;
using Infrastructure.ExternalServices;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class FileStorageInjection
    {
        public static IServiceCollection AddFileStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageOptions>(configuration.GetSection("Storage"));
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IStorageService, FileStorageService>();
            services.AddScoped<IFileUrlBuilder, FileUrlBuilder>();

            return services;
        }
    }
}

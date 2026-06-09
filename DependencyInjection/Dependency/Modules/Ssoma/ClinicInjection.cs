using Application.UseCases.Ssoma.Clinics;
using Core.Interfaces.Ssoma.Clinics;
using Infrastructure.Repositories.Ssoma.Clinics;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules.Ssoma
{
    public static class ClinicInjection
    {
        public static IServiceCollection AddClinicServices(this IServiceCollection services)
        {
            // Repository
            services.AddScoped<IClinicRepository, ClinicRepository>();

            // Use Cases
            services.AddScoped<CreateClinic>();
            services.AddScoped<UpdateClinic>();
            services.AddScoped<DeleteClinic>();
            services.AddScoped<GetAllClinics>();
            services.AddScoped<GetByIdClinic>();
            services.AddScoped<GetSelectClinic>();
            
            return services;
        }
    }
}

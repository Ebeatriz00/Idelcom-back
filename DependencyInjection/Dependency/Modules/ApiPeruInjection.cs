using Application.DTOs.ApiPeru;
using Application.UseCases.ApiPeru;
using Application.Validators.ApiPeru;
using Core.Interfaces.Services;
using Core.Options;
using FluentValidation;
using Infrastructure.ExternalServices.ApiPeru;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DependencyInjection.Dependency.Modules
{
    public static class ApiPeruInjection
    {
        public static IServiceCollection AddApiPeruServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiPeruOptions>(configuration.GetSection("ApiPeru"));

            services.AddScoped<ConsultApiPeruRuc>();
            services.AddTransient<IValidator<ApiPeruRucLookupRequestDto>, ApiPeruRucLookupRequestValidator>();

            services.AddHttpClient<IApiPeruRucLookupService, ApiPeruRucLookupService>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ApiPeruOptions>>().Value;

                if (Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
                    httpClient.BaseAddress = baseUri;

                if (options.TimeoutSeconds > 0)
                    httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            return services;
        }
    }
}

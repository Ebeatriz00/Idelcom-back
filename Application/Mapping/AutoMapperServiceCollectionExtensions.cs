using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class AutoMapperServiceCollectionExtensions
{
    public static IServiceCollection AddAutoMapper(
        this IServiceCollection services,
        Action<IMapperConfigurationExpression>? configurationAction,
        params Assembly[] assemblies)
    {
        var configuration = new MapperConfigurationExpression();
        configurationAction?.Invoke(configuration);

        var definitions = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => (Profile)Activator.CreateInstance(type)!)
            .SelectMany(profile => profile.Definitions)
            .ToList();

        services.AddSingleton<IMapper>(new RuntimeMapper(definitions));
        return services;
    }
}

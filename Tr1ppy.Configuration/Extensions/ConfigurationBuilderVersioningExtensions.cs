using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Tr1ppy.Configuration.Extensions;

public static class ConfigurationBuilderVersioningExtensions
{
    public static IServiceCollection LoadAppVersionFromAssemblyAttributes
    (
        this IServiceCollection services
    )
    {
        string fullVersion = 
            Assembly.GetEntryAssembly()!
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion ?? string.Empty;

        AppVersion appVersion = AppVersion.FromString(fullVersion);
        services.Configure<AppVersion>(appVersion.CopyTo);

        return services;
    }
}

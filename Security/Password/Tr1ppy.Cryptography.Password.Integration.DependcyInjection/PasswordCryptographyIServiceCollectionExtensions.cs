using Microsoft.Extensions.DependencyInjection;

namespace Tr1ppy.Cryptography.Password.Integration.DependencyInjection;

public static class PasswordCryptographyIServiceCollectionExtensions
{
    public static IServiceCollection AddPasswordCryptography
    (
        this IServiceCollection services,
        Action<PasswordCryptographyServiceSettings> configure
    )
    {
        services.Configure(configure);
        services.AddSingleton<PasswordCryptographyService>();
        return services;
    }
}

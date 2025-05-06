using Microsoft.Extensions.DependencyInjection;

namespace Tr1ppy.Cryptography.Password.Integration;

public static class PasswordCryptographyExtensions
{
    public static IServiceCollection AddPasswordCryptography
    (
        this IServiceCollection services,
        Action<HashPasswordSettings> configure
    )
    {
        services.Configure<HashPasswordSettings>(configure);
        services.AddSingleton<PasswordCryptographyService>();
        return services;
    }
}
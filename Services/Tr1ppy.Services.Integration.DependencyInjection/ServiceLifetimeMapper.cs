namespace Tr1ppy.Services.Integration.DependencyInjection;

public static class ServiceLifetimeMapper
{
    public static Microsoft.Extensions.DependencyInjection.ServiceLifetime Map(Attributes.ServiceLifetime lifetime)
    {
        return lifetime switch
        {
            Attributes.ServiceLifetime.Singleton => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton,
            Attributes.ServiceLifetime.Scoped => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped,
            Attributes.ServiceLifetime.Transient => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient,
        };
    }
}

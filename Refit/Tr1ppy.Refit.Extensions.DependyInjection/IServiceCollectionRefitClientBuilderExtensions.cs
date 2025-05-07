using Microsoft.Extensions.DependencyInjection;

namespace Tr1ppy.Refit.Extensions.DependencyInjection;

using Tr1ppy.Refit.Autofac;

public static class IServiceCollectionRefitClientBuilderExtensions
{
    public static IServiceCollectionRefitClientBuilder<TClient> AddRefitClient<TClient>(this IServiceCollection serviceCollection)
        where TClient : class
    {
        return new IServiceCollectionRefitClientBuilder<TClient>(serviceCollection);
    }
}

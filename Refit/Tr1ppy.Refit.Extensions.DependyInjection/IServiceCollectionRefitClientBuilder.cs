using Refit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tr1ppy.Refit.Autofac;

using Packages.Refit;

public class IServiceCollectionRefitClientBuilder<TClient>(IServiceCollection serviceCollection)
    : RefitClientBuilder<TClient, IServiceCollectionRefitClientBuilder<TClient>, IServiceCollection>(serviceCollection)
        where TClient : class
{
    public override IServiceCollection Register()
    {
        return _dependencyResolver.AddScoped(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = CreateHttpClient(httpClientFactory);

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            ResolveConnectionString(configuration);

            if (_baseAddress is not null)
                httpClient.BaseAddress = _baseAddress;

            return RestService.For<TClient>
            (
                client: httpClient,
                settings: _settings
            );

        });
    }
}

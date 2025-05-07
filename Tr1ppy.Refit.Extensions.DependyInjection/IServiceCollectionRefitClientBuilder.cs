using Refit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tr1ppy.Refit.Autofac;

using Packages.Refit;

public class IServiceCollectionRefitClientBuilder<TClient> 
    : RefitClientBuilder<TClient, IServiceCollectionRefitClientBuilder<TClient>>
        where TClient : class
{
    private readonly IServiceCollection _serviceCollection;

    public IServiceCollectionRefitClientBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public IServiceCollection Register()
    {
        return _serviceCollection.AddScoped(serviceProvider =>
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

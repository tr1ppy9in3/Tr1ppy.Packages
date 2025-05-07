using Refit;
using Autofac;

using Microsoft.Extensions.Configuration;

namespace Tr1ppy.Refit.Autofac;

using Packages.Refit;

public class AutofacRefitClientBuilder<TClient> 
    : RefitClientBuilder<TClient, AutofacRefitClientBuilder<TClient>>
        where TClient : class
{
    private readonly ContainerBuilder _containerBuilder;

    public AutofacRefitClientBuilder(ContainerBuilder containerBuilder)
    {
        _containerBuilder = containerBuilder;
    }

    public ContainerBuilder Register()
    {
        _containerBuilder.Register(ctx => 
        {
            var httpClientFactory = ctx.Resolve<IHttpClientFactory>();
            var httpClient = CreateHttpClient(httpClientFactory); 

            var configuration = ctx.Resolve<IConfiguration>();
            ResolveConnectionString(configuration);

            if (_baseAddress is not null)
                httpClient.BaseAddress = _baseAddress;

            return RestService.For<TClient>
            (
                client: httpClient,
                settings: _settings
            );
       }).As<TClient>().InstancePerLifetimeScope();

        return _containerBuilder;
    }
}

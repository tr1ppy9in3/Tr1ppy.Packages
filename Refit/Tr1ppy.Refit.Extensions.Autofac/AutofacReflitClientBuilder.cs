using Refit;
using Autofac;

using Microsoft.Extensions.Configuration;

namespace Tr1ppy.Refit.Autofac;

using Packages.Refit;

public class AutofacRefitClientBuilder<TClient>(ContainerBuilder containerBuilder)
    : RefitClientBuilder<TClient, AutofacRefitClientBuilder<TClient>, ContainerBuilder>(containerBuilder)
        where TClient : class
{
    public override ContainerBuilder Register()
    {
        _dependencyResolver.Register(ctx =>
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

        return _dependencyResolver;
    }
}

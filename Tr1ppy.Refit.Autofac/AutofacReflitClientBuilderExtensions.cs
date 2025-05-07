using Autofac;
using Tr1ppy.Refit.Autofac;

namespace Tr1ppy.Refit.Extensions.Autofac;

public static class AutofacReflitClientBuilderExtensions
{
    public static AutofacRefitClientBuilder<TClient> AddRefitClient<TClient>(this ContainerBuilder containerBuilder) 
        where TClient : class
    {
        return new AutofacRefitClientBuilder<TClient>(containerBuilder);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Tr1ppy.EntityFramework.Autofac;

namespace Tr1ppy.EntityFramework.DependencyInjection;

public static class ContainerBuilderEntityFrameworkExtensions
{
    public static DbContextInternalOptionsBuilderDependyInjection<TContext> AddDataContextTest<TContext>
    (
        this IServiceCollection serviceCollection
    )
        where TContext : DbContext
    {
        return new DbContextInternalOptionsBuilderDependyInjection<TContext>(serviceCollection);
    }
}
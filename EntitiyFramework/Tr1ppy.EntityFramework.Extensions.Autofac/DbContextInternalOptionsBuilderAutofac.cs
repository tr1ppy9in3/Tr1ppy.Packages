using Autofac;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tr1ppy.EntityFramework.Autofac;

public sealed class DbContextInternalOptionsBuilderAutofac<TContext>
    : DbContextInternalOptionsBuilder<TContext, DbContextInternalOptionsBuilderAutofac<TContext>>
        where TContext : DbContext
{
    private readonly ContainerBuilder _containerBuilder;

    public DbContextInternalOptionsBuilderAutofac(ContainerBuilder containerBuilder)
    {
        _containerBuilder = containerBuilder;
    }

    public ContainerBuilder Register()
    {
        _containerBuilder.Register(ctx =>
        {
            DbContextOptions? contextOptions = default;

            if (IsNeedConfiguration)
            {
                IConfiguration cfg = ctx.Resolve<IConfiguration>();
                contextOptions = Build(cfg); 
            }
            else
            {
                contextOptions = Build(null);
            }

            return (TContext)Activator.CreateInstance(typeof(TContext), contextOptions)!;
        }).As<TContext>().InstancePerLifetimeScope();

        return _containerBuilder;
    }
}

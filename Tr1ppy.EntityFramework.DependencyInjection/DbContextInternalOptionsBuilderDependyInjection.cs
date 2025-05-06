using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tr1ppy.EntityFramework.Autofac;

public sealed class DbContextInternalOptionsBuilderDependyInjection<TContext>
    : DbContextInternalOptionsBuilder<TContext, DbContextInternalOptionsBuilderDependyInjection<TContext>>
        where TContext : DbContext
{
    private readonly IServiceCollection _serviceCollection;

    public DbContextInternalOptionsBuilderDependyInjection(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public IServiceCollection Register()
    {
        _serviceCollection.AddScoped(serviceProvider =>
        {
            DbContextOptions<TContext>? dbContextOptions = default;
            if (IsNeedConfiguration)
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                dbContextOptions = Build(configuration);
            }
            else
            {
                dbContextOptions = Build(null);
            }

            return (TContext)Activator.CreateInstance(typeof(TContext), dbContextOptions)!;
        });

        return _serviceCollection;
    }
}

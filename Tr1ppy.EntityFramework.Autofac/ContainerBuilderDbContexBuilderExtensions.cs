using Autofac;
using Microsoft.EntityFrameworkCore;

namespace Tr1ppy.EntityFramework.Autofac;

public static class ContainerBuilderEntityFrameworkExtensions
{
    public static DbContextInternalOptionsBuilderAutofac<TContext> AddDataContext<TContext>(this ContainerBuilder containerBuilder) 
        where TContext : DbContext
    {
        return new DbContextInternalOptionsBuilderAutofac<TContext>(containerBuilder);
    }
}
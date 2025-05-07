using System.Reflection;
using Microsoft.Extensions.Logging;

using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace Autofac.Extensions.MediatR;

public static class ContainerBuilderMediatRExtensions
{
    public static ContainerBuilder AddMediatR
    (
        this ContainerBuilder containerBuilder, 
        ILogger? logger = default, 
        params Assembly[] assemblies
    )
    {
        MediatRConfiguration configuration =
            MediatRConfigurationBuilder.Create(assemblies)
                                       .WithAllOpenGenericHandlerTypesRegistered()
                                       .WithRegistrationScope(RegistrationScope.Scoped)
                                       .Build();

        logger?.LogDebug($"Registred mapping profiles from {string.Join(" ", assemblies.Select(assembly => assembly.GetName().Name))}");
        containerBuilder.RegisterMediatR(configuration);
        return containerBuilder;
    }
}


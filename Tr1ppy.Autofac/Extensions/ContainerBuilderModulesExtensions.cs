using Autofac;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Autofac.Extensions.Modules;

using Tr1ppy.AutoFac.Helpers;

public static class ContainerBuilderModulesExtensions
{
    public static ContainerBuilder RegisterDomainsModules
    (
        this ContainerBuilder containerBuilder,
        Action<AssemblyDomainFilterOptions>? configure = null,
        ILogger? logger = null
    )
    {
        var options = new AssemblyDomainFilterOptions();
        configure?.Invoke(options);

        List<Type> modulesTypes = [
            ..AssemblyLoader.ProjectAssembliesFromApp(options)
                            .SelectMany(assembly => assembly.GetTypes()
                            .Where(type => typeof(Autofac.Module).IsAssignableFrom(type) && !type.IsAbstract))
        ];

        foreach (Type moduleType in modulesTypes)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            bool hasParameters = 
                moduleType.GetConstructors()
                          .Any(c => c.GetParameters().Length > 0);

            if (!hasParameters)
                RegisterParameterlessModule(containerBuilder, moduleType);
            else
                RegisterParametersModule(containerBuilder, moduleType);

            logger?.LogDebug
            (
                message: "{ModuleName} dependencies created -> {ElapsedMilliseconds} ms.",
                args: [moduleType.Name, stopwatch.ElapsedMilliseconds]
            );

            stopwatch.Stop();
        }

        return containerBuilder;
    }

    public static void RegisterParameterlessModule(ContainerBuilder builder, Type moduleType)
    {
        if (Activator.CreateInstance(moduleType) is Autofac.Module moduleInstance)
        {
            builder.RegisterModule(moduleInstance);
        }
    }

    public static void RegisterParametersModule(ContainerBuilder builder, Type moduleType)
    {
        builder.RegisterBuildCallback(ctx =>
        {
            var constructor = 
                moduleType.GetConstructors()
                          .OrderByDescending(c => c.GetParameters().Length)
                          .First();

            var parameters = 
                constructor.GetParameters()
                           .Select(p => ctx.Resolve(p.ParameterType))
                           .ToArray();

            if (Activator.CreateInstance(moduleType, parameters) is Autofac.Module module)
            {
                module.Configure(builder.ComponentRegistryBuilder);
                builder.ComponentRegistryBuilder.Build();
            }
        });
    }
}


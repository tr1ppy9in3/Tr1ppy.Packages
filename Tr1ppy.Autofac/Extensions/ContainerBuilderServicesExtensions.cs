using Autofac;
using Autofac.Core;
using Autofac.Builder;

using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Autofac.Extensions.Services;

using Tr1ppy.DependencyInjection.Enums;
using Tr1ppy.DependencyInjection.Attributes;
using Tr1ppy.DependencyInjection.Attributes.Description;

public static class ContainerBuilderServicesExtensions
{
    private static readonly HashSet<Type> RegisteredServices = [];

    public static ContainerBuilder RegisterServicesWithAttributes
    (
        this ContainerBuilder containerBuilder, 
        ILogger? logger = null,
        params Assembly[] assemblies
    )
    {
        List<Type> assemblyTypes = [
            .. assemblies.SelectMany(a => a.GetTypes())
                         .Where(t => t.IsClass && !t.IsAbstract)
        ];

        RegisterAutoregistredServices(containerBuilder, assemblyTypes, logger);
        CheckoutManuallyRegistredServices(containerBuilder, assemblyTypes, logger);
        
        return containerBuilder;
    }

    #region Registration

    private static void RegisterAutoregistredServices
    (
        ContainerBuilder containerBuilder, 
        List<Type> assemblyTypes,
        ILogger? logger = null
    )
    {
        List<Type> autoregistredServicesTypes =
            [.. assemblyTypes.Where(type => type.GetCustomAttribute<AutoregistredServiceAttribute>() is not null)];

        foreach (Type implementationType in autoregistredServicesTypes)
        {
            RegisterService(containerBuilder, implementationType, logger);
        }
    }

    private static void CheckoutManuallyRegistredServices
    (
        ContainerBuilder containerBuilder,
        List<Type> assemblyTypes,
        ILogger? logger = null
    )
    {
        List<Type> manuallyRegisteredServiceTypes =
            [.. assemblyTypes.Where(type => type.GetCustomAttribute<ManuallyRegistredServiceAttribute>() is not null)];

        containerBuilder.RegisterBuildCallback(container =>
        {
            foreach (var serviceType in manuallyRegisteredServiceTypes)
            {
                var isRegistered =
                    container.ComponentRegistry.Registrations
                        .Any(r =>
                            r.Services.OfType<TypedService>().Any(ts => ts.ServiceType == serviceType) ||
                            r.Services.OfType<TypedService>().Any(ts => serviceType.GetInterfaces().Contains(ts.ServiceType))
                        );

                if (!isRegistered)
                    throw new InvalidOperationException($"Service {serviceType.Name} must be registered in the DI container.");
            }
        });
    }

    private static void RegisterService
    (
        ContainerBuilder builder,
        Type implementationType,
        ILogger? logger = default
    )
    {
        if (RegisteredServices.Contains(implementationType))
            return;

        AutoregistredServiceAttribute serviceAttribute = implementationType.GetCustomAttribute<AutoregistredServiceAttribute>()!;
        Type[] interfaces = implementationType.GetInterfaces();

        switch (interfaces.Length)
        {
            case 0:
                RegisterServiceWithoutInterface(builder, implementationType, serviceAttribute);
                break;

            case 1:
                RegisterServiceWithInterface(builder, implementationType, serviceAttribute);
                break;

            case > 1:
                logger?.LogWarning($"Skipped {implementationType.Name}: multiple interfaces found.");
                break;
        }

        RegisteredServices.Add(implementationType);
    }

    private static void RegisterServiceWithInterface
    (
        ContainerBuilder builder,
        Type implementationType,
        AutoregistredServiceAttribute autoregistredServiceAttribute
    )
    {
        var interfaceType = implementationType.GetInterfaces()[0];
        var registration = builder.RegisterType(implementationType).As(interfaceType);

        ApplyServiceLifetime(registration, autoregistredServiceAttribute);
        ApplyServiceDescription(registration, implementationType, interfaceType);
    }

    private static void RegisterServiceWithoutInterface
    (
        ContainerBuilder builder,
        Type implementationType,
        AutoregistredServiceAttribute autoregistredServiceAttribute
    )
    {
        var registration = builder.RegisterType(implementationType).AsSelf();

        ApplyServiceLifetime(registration, autoregistredServiceAttribute);
        ApplyServiceDescription(registration, implementationType);
    }

    #endregion

    #region Service description 

    private static void ApplyServiceLifetime
    (
        IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> registrationBuilder,
        AutoregistredServiceAttribute autoregistredServiceAttribute
    )
    {
        switch (autoregistredServiceAttribute.Lifetime)
        {
            case ServiceLifetime.Singleton:
                registrationBuilder.SingleInstance();
                break;

            case ServiceLifetime.Scoped:
                registrationBuilder.InstancePerLifetimeScope();
                break;

            case ServiceLifetime.ExternallyOwned:
                registrationBuilder.ExternallyOwned();
                break;

            case ServiceLifetime.Transient:

            default:
                registrationBuilder.InstancePerDependency();
                break;
        }
    }

    private static void ApplyServiceDescription
    (
        IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> registrationBuilder,
        Type implementationType,
        Type? interfaceType = default
    )
    {
        if (interfaceType is not null)
        {
            var namedAttribute = implementationType.GetCustomAttribute<NamedServiceAttribute>();
            if (namedAttribute is not null)
            {
                registrationBuilder.Named(namedAttribute.Name, interfaceType);
            }

            var keyedAttribute = implementationType.GetCustomAttribute<KeyedServiceAttribute>();
            if (keyedAttribute is not null)
            {
                registrationBuilder.Keyed(keyedAttribute.Key, interfaceType);
            }
        }

        List<MetadataServiceAttribute> metadataAttributes = 
            implementationType.GetCustomAttributes<MetadataServiceAttribute>().ToList();

        foreach (MetadataServiceAttribute metadataAttribute in metadataAttributes)
        {
            registrationBuilder.WithMetadata(metadataAttribute.Key, metadataAttribute.Value);
        }
    }

    #endregion
}

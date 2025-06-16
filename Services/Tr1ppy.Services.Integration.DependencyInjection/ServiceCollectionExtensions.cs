using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Tr1ppy.Services.Attributes;

namespace Tr1ppy.Services.Integration.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static readonly HashSet<Type> RegisteredServices = [];

    public static IServiceCollection AddAttributedServices(
        this IServiceCollection services, 
        ILogger? logger = default,
        params Assembly[] assemblies
    )
    {
        var assemblyTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsClass && !t.IsAbstract);
        var autoregistredServicesTypes = assemblyTypes.Where(type =>
            type.GetCustomAttribute<AutoregisteredServiceAttribute>() is not null
        );

        foreach (Type autoregisterService in autoregistredServicesTypes)
        {
            var platformAttrbute = autoregisterService.GetCustomAttribute<OnPlatformAttribute>();
            if (platformAttrbute is not null && !platformAttrbute.IsSupportByCurrentPlatform(autoregisterService))
                continue;

            if (RegisteredServices.Contains(autoregisterService))
                continue;

            AutoregisteredServiceAttribute attribute = autoregisterService.GetCustomAttribute<AutoregisteredServiceAttribute>()!;
            Type[] interfaces = autoregisterService.GetInterfaces();
            switch (interfaces.Length)
            {
                case 0:
                    RegisterServiceWithoutInterfaces(services, attribute, autoregisterService);
                    break;

                case 1:
                    RegisterServiceWithOneInterface(services, attribute, autoregisterService, interfaces.First());
                    break;

                case > 1:
                    logger?.LogWarning($"Skipped {autoregisterService.Name}: multiple interfaces found.");
                    break;
            }

            RegisteredServices.Add(autoregisterService);
        }
        return services;
    }

    public static IServiceCollection CheckoutMannualyRegisterServies(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        IEnumerable<Type> assemblyTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsClass && !t.IsAbstract);
        IEnumerable<Type> manuallyRegisteredServicesTypes = assemblyTypes.Where(type =>
            type.GetCustomAttribute<MustManuallyRegisterServiceAttribute>() is not null
        );

        foreach (Type manuallyRegisterService in manuallyRegisteredServicesTypes)
        {
            if (!services.Any(service => service.ImplementationType == manuallyRegisterService))
                throw new InvalidOperationException();
        }

        return services;
    }

    private static void RegisterServiceWithoutInterfaces(
        IServiceCollection services,
        AutoregisteredServiceAttribute attribute,
        Type serviceType
    )
    {
        ServiceDescriptor serviceDescriptor = new(
            serviceType: serviceType,
            implementationType: serviceType,
            serviceKey: KeyedAttribute.GetKeyFromType(serviceType),
            lifetime: ServiceLifetimeMapper.Map(attribute.Lifetime)
        );
        
        services.Add(serviceDescriptor);
    }

    private static void RegisterServiceWithOneInterface(
        IServiceCollection services, 
        AutoregisteredServiceAttribute attribute,
        Type serviceType, 
        Type interfaceType
    )
    {
        ServiceDescriptor serviceDescriptor = new(
            serviceType: interfaceType,
            implementationType: serviceType, 
            serviceKey: KeyedAttribute.GetKeyFromType(serviceType), 
            lifetime: ServiceLifetimeMapper.Map(attribute.Lifetime)
        );

        services.Add(serviceDescriptor);
    }
} 

using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Tr1ppy.Services.Attributes;
using Tr1ppy.Services.Attributes.Descriptions;

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
            if (ServiceAttributeResolver.HavePlatfromAttributeAndNotSupportedOnCurrent(autoregisterService))
                continue;

            if (RegisteredServices.Contains(autoregisterService))
                continue;

            AutoregisteredServiceAttribute attribute = autoregisterService.GetCustomAttribute<AutoregisteredServiceAttribute>()!;
            Type[] abstractions = ServiceTypeFilter.GetAbstractionsToRegister(autoregisterService).ToArray();
            switch (abstractions.Length)
            {
                case 0:
                    RegisterServiceWithoutAbstraction(services, attribute, autoregisterService);
                    break;

                case > 0:
                    foreach (Type abstraction in abstractions)
                        RegisterServiceWithOneAbstraction(services, attribute, autoregisterService, abstraction);

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

    private static void RegisterServiceWithoutAbstraction(
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

    private static void RegisterServiceWithOneAbstraction(
        IServiceCollection services, 
        AutoregisteredServiceAttribute attribute,
        Type serviceType, 
        Type abstractionType
    )
    {
        ServiceDescriptor serviceDescriptor = new(
            serviceType: abstractionType,
            implementationType: serviceType, 
            serviceKey: KeyedAttribute.GetKeyFromType(serviceType), 
            lifetime: ServiceLifetimeMapper.Map(attribute.Lifetime)
        );

        services.Add(serviceDescriptor);
    }
} 

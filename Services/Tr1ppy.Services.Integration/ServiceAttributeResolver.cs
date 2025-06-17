using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using Tr1ppy.Services.Attributes.Descriptions;

namespace Tr1ppy.Services.Integration;

public static class ServiceAttributeResolver
{
    public static bool HavePlatfromAttributeAndNotSupportedOnCurrent(Type serviceType)
    {
        var platformAttrbute = serviceType.GetCustomAttribute<OnPlatformAttribute>();
        return platformAttrbute is not null && !platformAttrbute.IsSupportByCurrentPlatform(serviceType);
    }

    public static bool TryGetAbstractionFromAttribute(Type serviceType, [NotNullWhen(true)] out IEnumerable<Type>? abstractions)
    {
        abstractions = null;

        var abstractionAttrbites = serviceType.GetCustomAttributes<FromAbstractionAttribute>();
        if (abstractionAttrbites is not null)
        {
            var abstractionsList = new List<Type>(abstractionAttrbites.Count());
            foreach (var abstractionAttribute in abstractionAttrbites)
            {
                var abstraction = abstractionAttribute.Abstraction;
                if (!abstraction.IsAssignableFrom(serviceType))
                    throw new ArgumentException($"Unable to register {serviceType} from {serviceType} cause is not implemnted!");

                abstractionsList.Add(abstraction);
            }

            abstractions = abstractionsList;
            return abstractions.Any();
        }
        else
        {
            return false;
        }
    }
}

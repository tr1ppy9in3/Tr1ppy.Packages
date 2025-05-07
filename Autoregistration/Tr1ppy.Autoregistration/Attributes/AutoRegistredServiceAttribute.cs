using Tr1ppy.DependencyInjection.Enums;

namespace Tr1ppy.DependencyInjection.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AutoregistredServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

    public AutoregistredServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Lifetime = lifetime;
    }
}
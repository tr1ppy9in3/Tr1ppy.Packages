namespace Tr1ppy.DependencyInjection.Attributes.Description;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class KeyedServiceAttribute : Attribute
{
    public object Key { get; }

    public KeyedServiceAttribute(object key)
    {
        Key = key;
    }
}
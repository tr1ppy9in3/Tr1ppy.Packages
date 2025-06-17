using System.Reflection;

namespace Tr1ppy.Services.Attributes.Descriptions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class KeyedAttribute(object key) : Attribute
{
    public object Key { get; } = key;

    public static object? GetKeyFromType(Type type)
    {
        var attribute = type.GetCustomAttribute<KeyedAttribute>();
        return attribute?.Key;
    }
}
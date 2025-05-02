namespace Tr1ppy.DependencyInjection.Attributes.Description;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MetadataServiceAttribute : Attribute
{
    public string Key { get; }
    public object Value { get; }

    public MetadataServiceAttribute(string key, object value)
    {
        Key = key;
        Value = value;
    }
}
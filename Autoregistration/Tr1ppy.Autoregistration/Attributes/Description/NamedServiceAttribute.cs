namespace Tr1ppy.DependencyInjection.Attributes.Description;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class NamedServiceAttribute : Attribute
{
    public string Name { get; }

    public NamedServiceAttribute(string name)
    {
        Name = name;
    }
}
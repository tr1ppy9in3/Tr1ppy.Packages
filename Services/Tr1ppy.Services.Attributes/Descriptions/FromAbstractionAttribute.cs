namespace Tr1ppy.Services.Attributes.Descriptions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class FromAbstractionAttribute : Attribute
{
    public Type Abstraction { get; }

    public FromAbstractionAttribute(Type abstraction)
    {
        ArgumentNullException.ThrowIfNull(abstraction);
        if (abstraction.IsSealed && !abstraction.IsInterface)
        {
            throw new ArgumentException($"Abstraction type '{abstraction.Name}' cannot be a sealed class.", nameof(abstraction));
        }
        Abstraction = abstraction;
    }
}
        
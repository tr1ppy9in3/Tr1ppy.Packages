namespace Tr1ppy.Queries.Integration;

public class InheritedTypesConfiguration
{
    public HashSet<Type> IncludedTypes { get; set; } = new HashSet<Type>();

    public HashSet<Type> ExcludedTypes { get; set; } = new HashSet<Type>();
}

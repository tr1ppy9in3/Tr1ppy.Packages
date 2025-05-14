namespace Tr1ppy.Queries.Integration;

/// <summary>
/// Builder for configuring which subtypes of a base type should be included or excluded in query processing.
/// </summary>
/// <typeparam name="TParentClass">The base type for which subtypes will be managed.</typeparam>
public class InheritedTypesConfgurationBuilder<TParentClass>
{
    private readonly HashSet<Type> _includedTypes = [];
    private readonly HashSet<Type> _excludedTypes = [];

    /// <summary>
    /// Marks a specific subtype of <typeparamref name="TParentClass"/> to be included in processing.
    /// </summary>
    /// <typeparam name="TSubtype">The subtype to include.</typeparam>
    public InheritedTypesConfgurationBuilder<TParentClass> IncludeSubtype<TSubtype>() where TSubtype : TParentClass
    {
        _includedTypes.Add(typeof(TSubtype));
        return this;
    }

    /// <summary>
    /// Marks a specific subtype of <typeparamref name="TParentClass"/> to be excluded from processing.
    /// </summary>
    /// <typeparam name="TSubtype">The subtype to exclude.</typeparam>
    public void ExcludeSubtype<TSubtype>() where TSubtype : TParentClass
        => _excludedTypes.Add(typeof(TSubtype));

    /// <summary>
    /// Builds a <see cref="InheritedTypesConfiguration"/> object containing the included and excluded subtype mappings.
    /// </summary>
    /// <returns>A configuration object used to filter types during query execution.</returns>
    public InheritedTypesConfiguration Build()
    {
        return new InheritedTypesConfiguration
        {
            IncludedTypes = _includedTypes,
            ExcludedTypes = _excludedTypes
        };
    }
}
using Tr1ppy.Querries.Integration;
using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Proccesors;

namespace Tr1ppy.Queries.Integration;

/// <summary>
/// A specialized builder for creating <see cref="QueryConfiguration{TPayload, TResult}"/> instances
/// using type-based registration for providers and processors. 
/// Designed for integration with dependency injection containers.
/// </summary>
/// <typeparam name="TPayload">The type of payload items to be processed.</typeparam>
/// <typeparam name="TResult">The result type produced after processing each payload item.</typeparam>
public class TypedQueryConfigurationBuilder<TPayload, TResult>
    : QueueConfigurationBuilder<TPayload, TResult>
{
    #region Fields

    private Type _processorType;

    private readonly List<Type> _providerTypes = new();
    private readonly List<Type> _preProcessorTypes = new();
    private readonly List<Type> _postProcessorTypes = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedQueryConfigurationBuilder{TPayload, TResult}"/> class
    /// with the specified queue name.
    /// </summary>
    /// <param name="name">The unique name of the query.</param>
    public TypedQueryConfigurationBuilder(string name) : base(name) { }

    #endregion 

    #region Public methods

    /// <summary>
    /// Adds a provider by a runtime type. The provider must inherit from <see cref="BaseItemsProvider{TPayload}"/>.
    /// </summary>
    /// <param name="providerType">The type of the provider to register.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the type does not inherit from BaseItemsProvider.</exception>
    public TypedQueryConfigurationBuilder<TPayload, TResult> AddProvider(Type providerType)
    {
        EnsureImplement(typeof(BaseItemsProvider<TPayload>), providerType);

        _providerTypes.Add(providerType);
        return this;
    }

    /// <summary>
    /// Adds a provider by generic type. The provider will be resolved from the DI container.
    /// </summary>
    /// <typeparam name="TProvider">The type of the provider to register.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    public TypedQueryConfigurationBuilder<TPayload, TResult> AddProvider<TProvider>()
        where TProvider : BaseItemsProvider<TPayload>
    {
        _providerTypes.Add(typeof(TProvider));
        return this;
    }

    /// <summary>
    /// Adds a pre-processor by a runtime type. The type must implement <see cref="IQueuePreProcessor{TPayload}"/>.
    /// </summary>
    /// <param name="preProcessorType">The type of the pre-processor to register.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the type does not implement IQueuePreProcessor.</exception>
    public TypedQueryConfigurationBuilder<TPayload, TResult> AddPreProcessor(Type preProcessorType)
    {
        EnsureImplement(typeof(IQueuePreProcessor<TPayload>), preProcessorType);

        _preProcessorTypes.Add(preProcessorType);
        return this;
    }

    /// <summary>
    /// Adds a pre-processor by generic type. The pre-processor will be resolved from the DI container.
    /// </summary>
    /// <typeparam name="TPreProcessor">The type of the pre-processor to register.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    public TypedQueryConfigurationBuilder<TPayload, TResult> AddPreProcessor<TPreProcessor>()
        where TPreProcessor : IQueuePreProcessor<TPayload>
    {
        _preProcessorTypes.Add(typeof(TPreProcessor));
        return this;
    }

    /// <summary>
    /// Sets the processor type that will handle the core processing logic of the queue.
    /// The processor will be resolved from the dependency injection container at runtime.
    /// </summary>
    /// <param name="processorType">
    /// The runtime <see cref="Type"/> of the processor. Must implement 
    /// <see cref="IQueueProcessor{TPayload, TResult}"/>.
    /// </param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided type does not implement the required processor interface.
    /// </exception>
    public TypedQueryConfigurationBuilder<TPayload, TResult> SetProcessor(Type processorType)
    {
        EnsureImplement(typeof(IQueueProcessor<TPayload, TResult>), processorType);

        _processorType = processorType;
        return this;
    }

    /// <summary>
    /// Sets the processor type that will handle the core processing logic of the queue.
    /// The processor will be resolved from the dependency injection container at runtime.
    /// </summary>
    /// <typeparam name="TProcessor">
    /// The processor type. Must implement <see cref="IQueueProcessor{TPayload, TResult}"/>.
    /// </typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    public TypedQueryConfigurationBuilder<TPayload, TResult> SetProcessor<TProcessor>()
        where TProcessor : IQueueProcessor<TPayload, TResult>
    {
        _processorType = typeof(TProcessor);
        return this;
    }

    /// <summary>
    /// Adds a post-processor by a runtime type. The type must implement <see cref="IQueuePostProcessor{TPayload, TResult}"/>.
    /// </summary>
    /// <param name="postProcessorType">The type of the post-processor to register.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the type does not implement IQueuePostProcessor.</exception>
    public TypedQueryConfigurationBuilder<TPayload, TResult> AddPostProcessor(Type postProcessorType)
    {
        EnsureImplement(typeof(IQueuePostProcessor<TPayload, TResult>), postProcessorType);

        _postProcessorTypes.Add(postProcessorType);
        return this;
    }

    /// <summary>
    /// Adds a post-processor by generic type. The post-processor will be resolved from the DI container.
    /// </summary>
    /// <typeparam name="TPostProcessor">The type of the post-processor to register.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    public TypedQueryConfigurationBuilder<TPayload, TResult> AddPostProcessor<TPostProcessor>()
        where TPostProcessor : IQueuePostProcessor<TPayload, TResult>
    {
        _postProcessorTypes.Add(typeof(TPostProcessor));
        return this;
    }

    #endregion

    #region Private methods

    private static void EnsureImplement(Type parentType, Type targetType)
    {
        if (!parentType.IsAssignableFrom(targetType))
            throw new ArgumentException($"Type {FormatType(targetType)} must implement {FormatType(parentType)}");
    }

    private static string FormatType(Type type)
    {
        if (!type.IsGenericType)
            return type.FullName ?? type.Name;

        var typeName = type.GetGenericTypeDefinition().FullName!;
        typeName = typeName[..typeName.IndexOf('`')];

        var genericArgs = type.GetGenericArguments()
            .Select(FormatType)
            .ToArray();

        return $"{typeName}<{string.Join(", ", genericArgs)}>";
    }

    #endregion
}

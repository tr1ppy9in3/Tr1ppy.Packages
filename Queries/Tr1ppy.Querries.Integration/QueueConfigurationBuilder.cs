
using System;

using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Configuration;
using Tr1ppy.Queries.Abstractions.Context;
using Tr1ppy.Queries.Abstractions.Proccesors;
using Tr1ppy.Queries.Integration;

namespace Tr1ppy.Querries.Integration;

/// <summary>
/// Fluent builder for constructing <see cref="QueryConfiguration{TPayload, TResult}"/> instances.
/// </summary>
/// <typeparam name="TPayload"> The type of the data being processed by the query. </typeparam>
/// <typeparam name="TResult"> The result type produced after processing each payload item. </typeparam>
public class QueueConfigurationBuilder<TPayload, TResult> 
{
    #region Fields

    private readonly QueryConfiguration<TPayload, TResult> _configuration;

    #endregion

    #region FluentAPI

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueConfigurationBuilder{TPayload, TResult}"/> class.
    /// </summary>
    /// <param name="name"> The unique name of the query. </param>
    public QueueConfigurationBuilder(string name)
    {
        _configuration = new(name);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueConfigurationBuilder{TPayload, TResult}"/> class.
    /// </summary>
    /// <param name="name"> The unique name of the query. </param>
    public QueueConfigurationBuilder(Enum nameEnum)
    {
        _configuration = new(nameEnum);
    }

    /// <summary>
    /// Sets an optional capacity limit for the queue.
    /// </summary>
    /// <param name="capacity"> The maximum number of items allowed in the queue. </param>
    /// <returns> The builder instance for method chaining .</returns>
    public QueueConfigurationBuilder<TPayload, TResult> WithCapacity(int capacity)
    {
        _configuration.Capacity = capacity;
        return this;
    }

    public QueueConfigurationBuilder<TPayload, TResult> AddProvider(
        BaseItemsProvider<TPayload> provider)
    {
        _configuration.ItemsProviders.Add(provider);
        return this;
    }

    /// <summary>
    /// Sets the core processing function for the query.
    /// </summary>
    /// <param name="processingFunction"> A function that transforms a payload into a result. </param>
    /// <returns> The builder instance for method chaining. </returns>
    public QueueConfigurationBuilder<TPayload, TResult> SetProcessing(
        Func<QueueProcessContext<TPayload>, TResult> processingFunction)
    {
        _configuration.ItemsProcessingFunction = processingFunction;
        return this;
    }

    public QueueConfigurationBuilder<TPayload, TResult> SetProcessing(
        IQueueProcessor<TPayload, TResult> processor)
    {
        _configuration.ItemsProcessor = processor;
        return this;
    }

    /// <summary>
    /// Adds a pre-processing action to be executed before the main processing function.
    /// </summary>
    /// <param name="preProcessingAction"> An action to be performed on the payload before processing. </param>
    /// <returns> The builder instance for method chaining. </returns>
    public QueueConfigurationBuilder<TPayload, TResult> AddPreProcessing(
        Action<QueuePreProcessContext<TPayload>> preProcessingAction)
    {
        _configuration.ItemsPreProcessingActions.Add(preProcessingAction);
        return this;
    }

    public QueueConfigurationBuilder<TPayload, TResult> AddPreProcessing(
        IQueuePreProcessor<TPayload> preProcessor)
    {
        _configuration.ItemsPreProcessors.Add(preProcessor);
        return this;
    }

    /// <summary>
    /// Adds a post-processing action to be executed after the main processing function.
    /// </summary>
    /// <param name="postProcessingAction"> An action to be performed on the payload and its result after processing. </param>
    /// <returns> The builder instance for method chaining. </returns>
    public QueueConfigurationBuilder<TPayload, TResult> AddPostProcessing(
        Action<QueuePostProcessContext<TPayload, TResult>> postProcessingAction)
    {
        _configuration.ItemsPostProcessingActions.Add(postProcessingAction);
        return this;
    }

    public QueueConfigurationBuilder<TPayload, TResult> AddPostProcessing(
        IQueuePostProcessor<TPayload, TResult> postProcessor)
    {
        _configuration.ItemsPostProcessors.Add(postProcessor);
        return this;
    }


    /// <summary>
    /// Configures type filtering rules using an inherited types configuration builder.
    /// </summary>
    /// <param name="configure"> A delegate used to configure allowed or excluded types. </param>
    /// <returns> The builder instance for method chaining. </returns>
    public QueueConfigurationBuilder<TPayload, TResult> ConfigureTypes(
        Action<InheritedTypesConfgurationBuilder<TPayload>> configure)
    {
        var builder = new InheritedTypesConfgurationBuilder<TPayload>();
        configure.Invoke(builder);

        _configuration.TypesConfiguration = builder.Build();
        return this;
    }

    /// <summary>
    /// Finalizes and constructs the <see cref="QueryConfiguration{TPayload, TResult}"/> instance.
    /// </summary>
    /// <returns> The constructed query configuration object. </returns>
    public QueryConfiguration<TPayload, TResult> Build()
        => _configuration;

    #endregion
}
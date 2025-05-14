using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Context;
using Tr1ppy.Queries.Abstractions.Proccesors;

namespace Tr1ppy.Queries.Integration;

public class QueryConfiguration<TPayload, TResult>
{
    /// <summary>
    /// The unique name of the query.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The optional capacity limit of the queue. 
    /// If set, limits the number of items processed concurrently or buffered.
    /// </summary>
    public int? Capacity { get; set; }

    public InheritedTypesConfiguration? TypesConfiguration { get; set; }

    public HashSet<IQueueItemsProvider<TPayload>> ItemsProviders { get; set; } = new();

    #region Processing

    private Func<QueueProcessContext<TPayload>, TResult>? _itemsProcessingFunction;
    private IQueueProcessor<TPayload, TResult>? _itemsProcessor;

    /// <summary>
    /// The core processing function that transforms a payload into a result.
    /// Cannot be set if <see cref="ItemsProcessor"/> is already assigned.
    /// </summary>
    public Func<QueueProcessContext<TPayload>, TResult>? ItemsProcessingFunction
    {
        get => _itemsProcessingFunction;
        set
        {
            if (_itemsProcessor is not null && value is not null)
                throw new InvalidOperationException("Cannot set ItemsProcessingFunction when ItemsProcessor is already assigned.");
            
            _itemsProcessingFunction = value;
        }
    }

    /// <summary>
    /// The object-based processor that handles payload processing logic.
    /// Cannot be set if <see cref="ItemsProcessingFunction"/> is already assigned.
    /// </summary>
    public IQueueProcessor<TPayload, TResult>? ItemsProcessor
    {
        get => _itemsProcessor;
        set
        {
            if (_itemsProcessingFunction is not null && value is not null) 
                throw new InvalidOperationException("Cannot set ItemsProcessor when ItemsProcessingFunction is already assigned.");
            
            _itemsProcessor = value;
        }
    }

    #endregion

    #region Preprocessing

    /// <summary>
    /// A set of pre-processing actions to be applied to each payload before the main processing logic.
    /// These actions do not alter the payload but can perform logging, validation, etc.
    /// </summary>
    public HashSet<Action<QueuePreProcessContext<TPayload>>> ItemsPreProcessingActions { get; set; } = new();

    public HashSet<IQueuePreProcessor<TPayload>> ItemsPreProcessors { get; set; } = new();

    #endregion

    #region Postprocessing

    /// <summary>
    /// A set of post-processing actions to be applied after the payload has been processed.
    /// These actions receive both the payload and its result and can be used for logging, cleanup, or result propagation.
    /// </summary>
    public HashSet<Action<QueuePostProcessContext<TPayload, TResult>>> ItemsPostProcessingActions { get; set; } = new();

    public HashSet<IQueuePostProcessor<TPayload, TResult>> ItemsPostProcessors { get; set; } = new();

    #endregion

    /// <summary>
    /// Validates the configuration to ensure it has exactly one processing mechanism.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if both or neither processing function and processor are configured.
    /// </exception>
    public void Validate()
    {
        bool hasFunction  = ItemsProcessingFunction is not null;
        bool hasProcessor = ItemsProcessor is not null;

        if (hasProcessor && hasFunction)
            throw new InvalidOperationException("Cannot configure both ItemsProcessor and ItemsProcessingFunction.");

        if (!hasProcessor && !hasFunction)
            throw new InvalidOperationException("One of ItemsProcessor or ItemsProcessingFunction must be configured.");

        ItemsPreProcessors = EnsureOnlyOneTypeInCollection(ItemsPreProcessors);
        ItemsPostProcessors = EnsureOnlyOneTypeInCollection(ItemsPostProcessors);
        ItemsPreProcessingActions = EnsureOnlyOneTypeInCollection(ItemsPreProcessingActions);
        ItemsPostProcessingActions = EnsureOnlyOneTypeInCollection(ItemsPostProcessingActions);
    }

    private static HashSet<TItem> EnsureOnlyOneTypeInCollection<TItem>(IEnumerable<TItem> collection)
    {
        return [.. collection
            .GroupBy(item => item?.GetType())
            .Select(group => group.First())];
    }
}

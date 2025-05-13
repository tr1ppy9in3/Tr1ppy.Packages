namespace Tr1ppy.Queries.Abstractions;

/// <summary>
/// Represents a storage mechanism for persisting the state of a queue.
/// Used to durably save and restore queue elements across application restarts or failures.
/// </summary>
/// <typeparam name="TPayload">The type of the queue elements.</typeparam>
public interface IQueueStateStorage<TPayload> where TPayload : class
{
    /// <summary>
    /// Persists a single queue element to the state storage.
    /// Typically called when an element is enqueued at runtime.
    /// </summary>
    /// <param name="item"> The element to persist. </param>
    /// <param name="cancellationToken"> Token to observe while waiting for the task to complete. </param>
    Task PersistAsync(
        TPayload item, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Persists a batch of queue elements to the state storage in a single operation.
    /// Useful for performance optimization when handling multiple items at once.
    /// </summary>
    /// <param name="items">The collection of elements to persist.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    Task BatchPersistAsync(
        IEnumerable<TPayload> items, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveAsync(
        TPayload item, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveBatchAsync(
        IEnumerable<TPayload> items, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Restores the collection of persisted queue elements.
    /// Typically called at application startup to resume processing from a previous state.
    /// </summary>
    /// <param name="cancellationToken"> Token to observe while waiting for the task to complete. </param>
    /// <returns> A collection of previously persisted queue elements .</returns>
    Task<IEnumerable<TPayload>> RestoreAsync(
        CancellationToken cancellationToken = default
    );
}

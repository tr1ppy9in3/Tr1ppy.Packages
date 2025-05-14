namespace Tr1ppy.Queries.Abstractions;

/// <summary>
/// Represents a provider that supplies items to be processed by the queue.
/// </summary>
/// <typeparam name="TPayload"> The type of the items being provided. </typeparam>
public interface IQueueItemsProvider<TPayload>
{
    /// <summary>
    /// Asynchronously retrieves a stream of items to be processed by the queue.
    /// </summary>
    /// <param name="context"> The queue current execution context. </param>
    /// <param name="cancellationToken"> A token to monitor for cancellation requests. </param>
    /// <returns> An asynchronous stream of queue items. </returns>
    public IAsyncEnumerable<TPayload> GetAsync(
        QueueContext context,
        CancellationToken cancellationToken = default
    );
}
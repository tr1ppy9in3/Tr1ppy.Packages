using System.Runtime.CompilerServices;

namespace Tr1ppy.Queries.Abstractions;

public abstract class BaseItemsProvider<TPayload>
{
    /// <summary>
    /// Asynchronously retrieves a stream of items to be processed by the queue.
    /// Automatically increments the queue counter on each iteration.
    /// </summary>
    public async IAsyncEnumerable<TPayload> GetAsync(
        QueueCounter queueCounter,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var enumerator = GetItemsAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);
        try
        {
            while (await enumerator.MoveNextAsync())
            {
                queueCounter.Increment();
                yield return enumerator.Current;
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
        }
    }

    /// <summary>
    /// Must be implemented by derived classes to fetch items from the data source.
    /// </summary>
    protected abstract IAsyncEnumerable<TPayload> GetItemsAsync(
        CancellationToken cancellationToken = default);
}
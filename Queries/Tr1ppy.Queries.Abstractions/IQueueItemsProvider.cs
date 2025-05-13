namespace Tr1ppy.Queries.Abstractions;

public interface IQueueItemsProvider<TPayload>
{
    public IAsyncEnumerable<TPayload> GetAsync(CancellationToken cancellationToken = default);
}

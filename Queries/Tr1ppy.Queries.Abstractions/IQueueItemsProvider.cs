namespace Tr1ppy.Queries.Abstractions;

public interface IQueueItemsProvider<TPayload> where TPayload : class 
{
    public IAsyncEnumerable<TPayload> GetAsync(CancellationToken cancellationToken = default);
}

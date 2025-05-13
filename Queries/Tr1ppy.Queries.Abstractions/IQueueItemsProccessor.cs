namespace Tr1ppy.Queries.Abstractions;

public interface IQueueItemsProccessor<TPayload> where TPayload : class
{
    public Task ProccessAsync(TPayload payload, CancellationToken cancellationToken);
}

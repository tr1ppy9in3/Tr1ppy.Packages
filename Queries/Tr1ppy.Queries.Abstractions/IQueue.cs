namespace Tr1ppy.Queries.Abstractions;

public interface IQueue<TPayload>
{
    event Action<TPayload>? OnDequeue;
    event Action<TPayload>? OnEnqueue;

    event Action<TPayload>? OnQueueFull;
    event Action<TPayload>? OnQueueEmpty;

    bool IsEmpty { get; }
    int Count { get; }

    Task EnqueueAsync(TPayload payload, CancellationToken cancellationToken = default);

    TPayload DequeueAsync(CancellationToken cancellationToken = default);
}

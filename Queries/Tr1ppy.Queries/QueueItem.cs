namespace Tr1ppy.Queries.Abstractions;

/// <summary>
/// Wrapper for queue items.
/// </summary>
/// <typeparam name="TPayload"></typeparam>
public sealed class QueueItem<TPayload> 
{
    public Guid Id { get; init; } = Guid.NewGuid(); 

    public TPayload Payload { get; init; }

    public DateTime EnqueuedAt { get; init; }

    public QueueItem(TPayload payload)
    {
        Payload = payload;
        EnqueuedAt = DateTime.UtcNow;
    }
}

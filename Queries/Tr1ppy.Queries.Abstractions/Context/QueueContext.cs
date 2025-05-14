using Tr1ppy.Queries;

/// <summary>
/// Represents the execution context of a queue, containing metadata and statistics
/// that can be shared across all queue components such as producers, processors.
/// </summary>
public class QueueContext
{
    /// <summary>
    /// Gets the unique name of the queue or processing channel.
    /// Useful for logging, diagnostics, or routing logic.
    /// </summary>
    public string QueueName { get; init; }

    /// <summary>
    /// Gets the total capacity of the queue.
    /// This value may be used to limit or monitor the number of items the queue can hold.
    /// </summary>
    public int? QueueCapacity { get; init; }

    /// <summary>
    /// Gets the counter object used to track various queue metrics such as the number of items
    /// processed, failed, skipped, or currently pending.
    /// </summary>
    public QueueCounter Count { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueContext"/> class with the specified
    /// name, capacity, and counter.
    /// </summary>
    /// <param name="name">The unique name of the queue.</param>
    /// <param name="capacity">The maximum capacity or size of the queue.</param>
    /// <param name="counter">An object that tracks queue-related statistics and metrics.</param>
    public QueueContext(string name, int? capacity, QueueCounter counter)
    {
        QueueName = name;
        QueueCapacity = capacity;
        Count = counter;
    }
}

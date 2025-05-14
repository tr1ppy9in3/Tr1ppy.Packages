using Tr1ppy.Queries.Abstractions.Context;

namespace Tr1ppy.Queries.Abstractions.Proccesors;

/// <summary>
/// Defines a pre-processing operation that is executed before a queue item is processed.
/// </summary>
/// <typeparam name="TPayload">The type of the input data (queue item).</typeparam>
public interface IQueuePreProcessor<TPayload>
{
    /// <summary>
    /// Performs a pre-processing operation on the given payload before it 
    /// is processed by the main processor.
    /// </summary>
    /// <param name="payload"> The queue item to be pre-processed. </param>
    /// <param name="context"> The queue current execution context. </param>
    /// <param name="cancellationToken"> A token to monitor for cancellation requests. </param>
    Task PreProcessAsync(
        QueuePreProcessContext<TPayload> context, 
        CancellationToken cancellationToken = default
    );
}

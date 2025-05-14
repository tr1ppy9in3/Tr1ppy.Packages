using Tr1ppy.Queries.Abstractions.Context;

namespace Tr1ppy.Queries.Abstractions.Proccesors;


/// <summary>
/// Defines a processor that handles the core logic for processing items in a queue.
/// </summary>
/// <typeparam name="TPayload"> The type of the input data (queue item). </typeparam>
/// <typeparam name="TResult"> The type of the result produced after processing the payload. </typeparam>
public interface IQueueProcessor<TPayload, TResult>
{
    /// <summary>
    /// Processes the specified payload and returns a result.
    /// </summary>
    /// <param name="payload"> The queue item to be processed. </param>
    /// <param name="counter"> An object used to track queue processing statistics or progress. </param>
    /// <param name="cancellationToken"> A token to monitor for cancellation requests. </param>
    /// <returns> A task representing the asynchronous operation, containing the result of processing. </returns>
    Task<TResult> ProcessAsync(
        QueueProcessContext<TPayload> context,
        CancellationToken cancellationToken = default
    );
}
using Tr1ppy.Queries.Abstractions.Context;

namespace Tr1ppy.Queries.Abstractions.Proccesors;

/// <summary>
/// Defines a post-processing operation that is executed after a queue item has been processed.
/// </summary>
/// <typeparam name="TPayload"> The type of the input data (queue item). </typeparam>
/// <typeparam name="TResult"> The type of the result produced by processing the payload. </typeparam>
public interface IQueuePostProcessor<TPayload, TResult>
{
    /// <summary>
    /// Performs a post-processing operation on the given payload and its corresponding result.
    /// This method is called after the payload has been processed by the main processor.
    /// </summary>
    /// <param name="context"> The queue current execution context. </param>
    /// <param name="payload"> The original queue item that was processed. </param>
    /// <param name="result"> The result of processing the payload. </param>
    /// <param name="cancellationToken"> A token to monitor for cancellation requests. </param>
    Task PostProcessAsync(
        QueuePostProcessContext<TPayload, TResult> context,
        CancellationToken cancellationToken = default
    );
}
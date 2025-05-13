using Tr1ppy.Queries.Abstractions.Proccesors;

namespace Tr1ppy.Queries.Proccesors.Postproccesors;

public class LoggingPostProcessor<TPayload, TResult> : IQueuePostProcessor<TPayload, TResult>
{
    public Task PostProcessAsync(TPayload payload, TResult result, CancellationToken cancellationToken)
    {
        var payloadString = payload?.ToString() ?? string.Empty;
        var resultString = result?.ToString() ?? string.Empty;

        Console.WriteLine($"Postproccesing {payloadString} with result {resultString}");
        return Task.CompletedTask;
    }
}

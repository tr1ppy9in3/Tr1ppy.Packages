using Tr1ppy.Queries.Abstractions.Proccesors;

namespace Tr1ppy.Queries.Proccesors.Preprocessors;

public class LoggingPrepocessor<TPayload> : IQueuePreProcessor<TPayload>
{
    public Task PreProcessAsync(TPayload payload, CancellationToken cancellationToken)
    {
        var payloadString = payload?.ToString() ?? string.Empty;

        Console.WriteLine($"Preproccesing {payloadString}");
        return Task.CompletedTask;
    }
}

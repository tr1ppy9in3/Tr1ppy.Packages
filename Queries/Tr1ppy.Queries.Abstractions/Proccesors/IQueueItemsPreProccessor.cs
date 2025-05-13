namespace Tr1ppy.Queries.Abstractions.Proccesors;

public interface IQueuePreProcessor<TPayload>
{
    Task PreProcessAsync(TPayload payload, CancellationToken cancellationToken);
}

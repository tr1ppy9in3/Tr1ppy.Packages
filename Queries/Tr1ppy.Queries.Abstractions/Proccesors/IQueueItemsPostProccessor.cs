namespace Tr1ppy.Queries.Abstractions.Proccesors;

public interface IQueuePostProcessor<TPayload, TResult>
{
    Task PostProcessAsync(TPayload payload, TResult result, CancellationToken cancellationToken);
}
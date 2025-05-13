namespace Tr1ppy.Queries.Abstractions.Proccesors;

public interface IQueueProcessor<TPayload, TResult>
{
    Task<TResult> ProcessAsync(TPayload payload, CancellationToken cancellationToken);
}
namespace Tr1ppy.Queries.Abstractions.Context;

public sealed class QueuePostProcessContext<TPayload, TResult>
{
    public QueueContext ProcessContext { get; set; }

    public TResult Result { get; }

    public TPayload Payload { get; }

    public QueuePostProcessContext(QueueContext processContext, TResult result, TPayload payload)
    {
        ProcessContext = processContext;
        Result = result;
        Payload = payload;
    }
}

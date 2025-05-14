namespace Tr1ppy.Queries.Abstractions.Context;

public class QueueProcessContext<TPayload>
{
    public QueueContext ProcessContext { get; set; }

    public TPayload Payload { get; }

    public QueueProcessContext(QueueContext processContext, TPayload payload)
    {
        ProcessContext = processContext;
        Payload = payload;
    }
}

namespace Tr1ppy.Queries.Abstractions.Context;

public class QueuePreProcessContext<TPayload>
{
    public QueueContext ProcessContext { get; set; }

    public TPayload Payload { get; }

    public QueuePreProcessContext(QueueContext processContext, TPayload payload)
    {
        ProcessContext = processContext;
        Payload = payload;
    }
}

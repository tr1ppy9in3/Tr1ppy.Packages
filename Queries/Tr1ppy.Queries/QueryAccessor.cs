using Microsoft.Extensions.DependencyInjection;

namespace Tr1ppy.Queries;

public class QueryAccessor<TPayload, TResult>
{
    private readonly Dictionary<string, ProccesableQueue<TPayload, TResult>> _queuesByName = new();
    private readonly Dictionary<Type, List<ProccesableQueue<TPayload, TResult>>> _queuesByType = new();

    public QueryAccessor(IServiceProvider provider)
    {
        var queues = provider.GetServices<ProccesableQueue<TPayload, TResult>>();
        InitializeDictionaries(queues);
    }

    public QueryAccessor(IEnumerable<ProccesableQueue<TPayload, TResult>> queues)
    {
        InitializeDictionaries(queues);
    }

    public async Task EnqueueAsync(TPayload payload)
    {
        if (payload is null)
            return;

        if (_queuesByType.TryGetValue(payload.GetType(),out var queue))
        {
            await queue.First().EnqueueAsync(payload);   
        }
    }

    public async Task EnqueueAsync(TPayload payload, string queueName)
    {
        if (payload is null)
            return;

        if (_queuesByName.TryGetValue(queueName, out var queue))
        {
            await queue.EnqueueAsync(payload);
        }
    }

    private void InitializeDictionaries(IEnumerable<ProccesableQueue<TPayload, TResult>> queues)
    {
        foreach (var queue in queues)
        {
            _queuesByName[queue.Name] = queue;

            if (queue.IncludedSubTypes.Count > 0)
            {
                foreach (var subType in queue.IncludedSubTypes)
                {
                    RegisterType(subType, queue);
                }

            }

            else if (queue.ExcludedSubTypes.Count > 0)
            {
                foreach (var subType in queue.ExcludedSubTypes)
                {

                }
            }

            else
            {
                RegisterType(typeof(TPayload), queue);
            }
        }
    }

    private void RegisterType(Type type, ProccesableQueue<TPayload, TResult> queue)
    {
        if (!_queuesByType.TryGetValue(type, out var list))
            _queuesByType[type] = list = new();

        list.Add(queue);
    }

    private void UnregisterType(Type type, ProccesableQueue<TPayload, TResult> queue)
    {

    }
}

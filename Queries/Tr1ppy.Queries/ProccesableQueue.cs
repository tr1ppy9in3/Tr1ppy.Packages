using System.Threading.Channels;

using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Proccesors;

namespace Tr1ppy.Queries;

public class ProccesableQueue<TPayload, TResult> where TPayload : class
{
    private readonly string _name;

    private readonly Channel<TPayload> _channel;
    private readonly IQueueStateStorage<TPayload>? _stateStorage;
    private readonly HashSet<IQueueItemsProvider<TPayload>> _itemsProvider;

    private readonly IQueueProcessor<TPayload, TResult> _itemsProccessor;
    private readonly HashSet<IQueuePreProcessor<TPayload>> _preProcessors = new();
    private readonly HashSet<IQueuePostProcessor<TPayload, TResult>> _postProcessors = new();

    private readonly List<Task>? _runningTasks;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ProccesableQueue
    (
        string name,
        IQueueStateStorage<TPayload>? queueStateStorage,

        IQueueProcessor<TPayload,TResult> proccessor,
        IEnumerable<IQueuePreProcessor<TPayload>>? preProcessors,
        IEnumerable<IQueuePostProcessor<TPayload, TResult>>? postProcessors,

        IEnumerable<IQueueItemsProvider<TPayload>> itemsProvider,
        int? capacity)
    {
        _name = name;
        _stateStorage = queueStateStorage;
        _itemsProccessor = proccessor;
        _itemsProvider = itemsProvider.ToHashSet();

        if (preProcessors is not null)
        {
            EnsureOnlyOneTypePreproccesor(_preProcessors);
            _preProcessors = preProcessors.ToHashSet();
        }

        if (postProcessors is not null)
        {
            EnsureOnlyOneTypePostproccesor(_postProcessors);
            _postProcessors = postProcessors.ToHashSet();
        }

        _channel = capacity is not null
           ? Channel.CreateBounded<TPayload>(capacity.Value)
           : Channel.CreateUnbounded<TPayload>();
    }

    private static void EnsureOnlyOneTypePreproccesor(HashSet<IQueuePreProcessor<TPayload>> preProcessors)
    {
        var unique = preProcessors
            .GroupBy(p => p.GetType())
            .Select(g => g.First())
            .ToHashSet();

        preProcessors.Clear();
        foreach (var processor in unique)
            preProcessors.Add(processor);
    }


    private static void EnsureOnlyOneTypePostproccesor(HashSet<IQueuePostProcessor<TPayload, TResult>> postProcessors)
    {
        var unique = postProcessors
            .GroupBy(p => p.GetType())
            .Select(g => g.First())
            .ToHashSet();

        postProcessors.Clear();
        foreach (var processor in unique)
            postProcessors.Add(processor);
    }

    public async Task RestoreAsync()
    {
        if (_stateStorage is null)
            return;

        var items = await _stateStorage.RestoreAsync(_cancellationTokenSource.Token);
        foreach (var item in items)
        {
            await _channel.Writer.WriteAsync(item, _cancellationTokenSource.Token);
        }
    }

    public async Task EnqueueAsync(TPayload item)
    {
        if (_stateStorage is not null)
           await _stateStorage.PersistAsync(item, _cancellationTokenSource.Token);

        await _channel.Writer.WriteAsync(item, _cancellationTokenSource.Token);
    }

    public Task StartAsync()
    {
        foreach (var provider in _itemsProvider)
        {
            _ = Task.Run(async () =>
            {
                await foreach (var item in provider.GetAsync(_cancellationTokenSource.Token))
                {
                    await EnqueueAsync(item);
                }
            }, _cancellationTokenSource.Token);
        }

        _ = Task.Run(async () =>
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
            {
                try
                {
                    await ProcessItemAsync(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Processor] Error: {ex.Message}");
                }
            }
        }, _cancellationTokenSource.Token);

        return Task.CompletedTask;
    }

    private async Task ProcessItemAsync(TPayload payload)
    {
        foreach (IQueuePreProcessor<TPayload> preProcessor in _preProcessors)
            await preProcessor.PreProcessAsync(payload, _cancellationTokenSource.Token);

        TResult result = await _itemsProccessor.ProcessAsync(payload, _cancellationTokenSource.Token);

        foreach(IQueuePostProcessor<TPayload, TResult> postProccesor in _postProcessors)
            await postProccesor.PostProcessAsync(payload, result, _cancellationTokenSource.Token);
    }

    public async Task StopAsync()
    {
        await _cancellationTokenSource.CancelAsync();

        try
        {
            _channel.Writer.Complete();
        }
        catch { }

        try
        {
            await Task.WhenAll(_runningTasks);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {

        }
    } 
}

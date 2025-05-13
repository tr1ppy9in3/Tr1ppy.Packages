using System.Threading.Channels;

using Tr1ppy.Queries.Abstractions;

namespace Tr1ppy.Queries;

public class QueueEngine<TPayload> where TPayload : class
{
    private readonly Channel<TPayload> _channel;
    private readonly IQueueStateStorage<TPayload> _stateStorage;

    private readonly IQueueItemsProccessor<TPayload> _itemsProccessor;
    private readonly HashSet<IQueueItemsProvider<TPayload>> _itemsProvider;

    private readonly List<Task> _runningTasks;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public QueueEngine(IQueueStateStorage<TPayload> queueStateStorage, int? capacity)
    {
        _stateStorage = queueStateStorage;
        _channel = capacity is not null
           ? Channel.CreateBounded<TPayload>(capacity.Value)
           : Channel.CreateUnbounded<TPayload>();
    }

    public async Task RestoreAsync()
    {
        var items = await _stateStorage.RestoreAsync(_cancellationTokenSource.Token);
        foreach (var item in items)
        {
            await _channel.Writer.WriteAsync(item, _cancellationTokenSource.Token);
        }
    }

    public async Task EnqueueAsync(TPayload item)
    {
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
                    await _itemsProccessor.ProccessAsync(item, _cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Processor] Error: {ex.Message}");
                }
            }
        }, _cancellationTokenSource.Token);

        return Task.CompletedTask;
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

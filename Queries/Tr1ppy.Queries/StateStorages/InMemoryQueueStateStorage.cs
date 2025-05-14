//using System.Runtime.CompilerServices;

//using Tr1ppy.Queries.Abstractions;
//using System.Collections.Concurrent;


//namespace Tr1ppy.Queries.Sources;

///// <summary>
///// По факту заглушка.
///// </summary>
///// <typeparam name="TPayload"></typeparam>
//public class InMemoryQueueStateStorage<TPayload> : 
//    IQueueStateStorage<TPayload> where TPayload : class
//{
//    private readonly ConcurrentQueue<TPayload> _storage  = new();

//    /// <summary>
//    /// <inheritdoc/>
//    /// </summary>
//    public async IAsyncEnumerable<TPayload> ReadAsync(
//        [EnumeratorCancellation] CancellationToken cancellationToken = default)
//    {
//        while (!cancellationToken.IsCancellationRequested)
//        {
//            if (_storage.TryDequeue(out var item))
//            {
//                yield return item;
//            }
//            else
//            {
//                await Task.Delay(50, cancellationToken); 
//            }
//        }
//    }

//    /// <summary>
//    /// <inheritdoc/>
//    /// </summary>
//    public Task SaveAsync(
//        TPayload item, 
//        CancellationToken cancellationToken = default)
//    {
//        _storage.Enqueue(item);
//        return Task.CompletedTask;
//    }

//    /// <summary>
//    /// <inheritdoc/>
//    /// </summary>
//    public async Task BatchSaveAsync(
//        IEnumerable<TPayload> items, 
//        CancellationToken cancellationToken = default)
//    {
//        foreach (var item in items)
//            await SaveAsync(item, cancellationToken);
//    }
//}

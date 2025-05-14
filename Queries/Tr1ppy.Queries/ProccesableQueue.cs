using System.Diagnostics.Metrics;
using System.Threading.Channels;

using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Configuration;
using Tr1ppy.Queries.Abstractions.Context;
using Tr1ppy.Queries.Abstractions.Proccesors;

namespace Tr1ppy.Queries;

public class ProccesableQueue<TPayload, TResult> where TPayload : class
{
    #region Fields

    private readonly string _name;
    private readonly int? _capacity;
    private readonly QueueCounter _counter = new();

    private readonly HashSet<Type> _includedSubTypes = new();
    private readonly HashSet<Type> _excludedSubTypes = new();
        
    private readonly Channel<TPayload> _channel;
    private readonly IQueueStateStorage<TPayload>? _stateStorage;
    private readonly HashSet<BaseItemsProvider<TPayload>> _itemsProviders= new();

    private readonly IQueueProcessor<TPayload, TResult>? _itemsProcessor;
    private readonly Func<QueueProcessContext<TPayload>, TResult>? _itemsProcessingFunction;

    private readonly HashSet<IQueuePreProcessor<TPayload>> _preProcessors = new();
    private readonly HashSet<Action<QueuePreProcessContext<TPayload>>> _itemsPreProcessingActions = new();

    private readonly HashSet<IQueuePostProcessor<TPayload, TResult>> _postProcessors = new();
    private readonly HashSet<Action<QueuePostProcessContext<TPayload, TResult>>> _itemsPostPriccesingActions = new();

    private readonly List<Task> _runningTasks = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region Properies

    public int ItemsCount =>
        _counter.Value;

    #endregion

    #region Constructors

    public ProccesableQueue(QueryConfiguration<TPayload, TResult> configuration)
    {
        configuration.Validate();

        _name = configuration.Name;
        _capacity = configuration.Capacity;

        _includedSubTypes = configuration.TypesConfiguration.IncludedTypes;
        _includedSubTypes = configuration.TypesConfiguration.ExcludedTypes;

        _stateStorage = null;
        _itemsProviders = configuration.ItemsProviders;

        _itemsProcessor = configuration.ItemsProcessor;
        _itemsProcessingFunction = configuration.ItemsProcessingFunction;

        _itemsPreProcessingActions = configuration.ItemsPreProcessingActions;
        _preProcessors = configuration.ItemsPreProcessors;

        _itemsPostPriccesingActions = configuration.ItemsPostProcessingActions;
        _postProcessors = configuration.ItemsPostProcessors;

        _channel = configuration.Capacity is not null
           ? Channel.CreateBounded<TPayload>(configuration.Capacity.Value)
           : Channel.CreateUnbounded<TPayload>();
    }

    #endregion

    #region Public methods

    public async Task InitializeAsync()
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
        _runningTasks.Add(RunProcessorAsync());

        foreach (var provider in _itemsProviders)
        {
            var providerTask = RunProviderAsync(provider);
            _runningTasks.Add(providerTask);
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _channel.Writer.TryComplete();

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

    #endregion

    #region Private methods

    private async Task RunProviderAsync(BaseItemsProvider<TPayload> provider)
    {
        await foreach (var item in provider.GetAsync(_counter, _cancellationTokenSource.Token))
        {
            await EnqueueAsync(item);
        }
    }

    private async Task RunProcessorAsync()
    {
        await foreach (TPayload payload in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
        {
            Type payloadType = payload.GetType();
            if (!ShouldProcessPayload(payloadType))
                continue;

            var context = new QueueContext(_name, _capacity, ItemsCount);

            try
            {
                await PreProcessItemAsync(context, payload);

                TResult result = await ProcessItemAsync(context, payload);
                _counter.Decrement();

                await PostProcessItemAsync(context, payload, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Processor] Error: {ex.Message}");
            }
        }
    }

    private bool ShouldProcessPayload(Type payloadType)
    {
        if (payloadType == typeof(TPayload))
            return true;

        if (_includedSubTypes?.Count > 0 && !_includedSubTypes.Contains(payloadType))
            return false;

        if (_excludedSubTypes?.Contains(payloadType) == true)
            return false;

        return true;
    }

    private async Task<TResult> ProcessItemAsync(QueueContext context, TPayload payload)
    {
        var processContext = new QueueProcessContext<TPayload>(context, payload);

        if (_itemsProcessor is not null)
            return await _itemsProcessor.ProcessAsync(processContext, _cancellationTokenSource.Token);

        if (_itemsProcessingFunction is not null)
            return _itemsProcessingFunction.Invoke(processContext);

        throw new ArgumentNullException("Proccesing", "Unable to find available processing action!");
    }

      
    private async Task PreProcessItemAsync(QueueContext context, TPayload payload)
    {
        var preProcessContext = new QueuePreProcessContext<TPayload>(context, payload);
        
        if (_itemsPreProcessingActions is not null && _itemsPreProcessingActions.Count > 0)
        {
            foreach (var action in _itemsPreProcessingActions)
                action.Invoke(preProcessContext);
        }

        foreach (IQueuePreProcessor<TPayload> preProcessor in _preProcessors)
            await preProcessor.PreProcessAsync(preProcessContext, _cancellationTokenSource.Token);
    }

    private async Task PostProcessItemAsync(QueueContext context, TPayload payload, TResult result)
    {
        var postProcessContext = new QueuePostProcessContext<TPayload, TResult>(context, result, payload);

        if (_itemsPostPriccesingActions is not null && _itemsPostPriccesingActions.Count > 0)
        {
            foreach (var action in _itemsPostPriccesingActions)
                action.Invoke(postProcessContext);
        }

        foreach (IQueuePostProcessor<TPayload, TResult> postProccesor in _postProcessors)
            await postProccesor.PostProcessAsync(postProcessContext, _cancellationTokenSource.Token);
    }
    

    #endregion
}

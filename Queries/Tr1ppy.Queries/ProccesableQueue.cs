using System.Diagnostics.Metrics;
using System.Threading.Channels;

using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Configuration;
using Tr1ppy.Queries.Abstractions.Context;
using Tr1ppy.Queries.Abstractions.Proccesors;
using Tr1ppy.Querries.Integration;

namespace Tr1ppy.Queries;

/// <summary>
/// Represents a processable queue with support for type filtering, 
/// pre-processing, post-processing, state persistence, and external item providers.
/// </summary>
/// <typeparam name="TPayload"> The type of items processed by the queue. </typeparam>
/// <typeparam name="TResult"> The result type produced after processing each item. </typeparam>
public class ProccesableQueue<TPayload, TResult>
{

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


    #region Properies

    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    public string Name =>
        _name;

    /// <summary>
    /// Gets the number of items currently pending.
    /// </summary>
    public int ItemsCount =>
        _counter.Value;

    /// <summary>
    /// Gets the explicitly included types that this queue supports.
    /// </summary>
    public HashSet<Type> IncludedSubTypes =>
        _includedSubTypes;

    /// <summary>
    /// Gets the explicitly excluded types that this queue will ignore.
    /// </summary>
    public HashSet<Type> ExcludedSubTypes =>
        _excludedSubTypes;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProccesableQueue{TPayload, TResult}"/> class
    /// using the specified configuration.
    /// </summary>
    /// <param name="configuration"> The configuration that defines processing logic and settings. </param>
    public ProccesableQueue(QueryConfiguration<TPayload, TResult> configuration)
    {
        configuration.Validate();

        _name = configuration.Name;
        _capacity = configuration.Capacity;

        _includedSubTypes = configuration.TypesConfiguration.IncludedTypes;
        _excludedSubTypes = configuration.TypesConfiguration.ExcludedTypes;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="ProccesableQueue{TPayload, TResult}"/> class
    /// using the specified configuration builder.
    /// </summary>
    /// <param name="configurationBuilder"> The configuration builder. </param>
    public ProccesableQueue(QueueConfigurationBuilder<TPayload, TResult> configurationBuilder)
        : this(configurationBuilder.Build())
    {
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Restores items from persistent storage (if enabled) into the queue.
    /// </summary>
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

    /// <summary>
    /// Enqueues a payload into the queue, persisting it if required.
    /// </summary>
    /// <param name="payload"> The payload to enqueue. </param>
    public async Task EnqueueAsync(TPayload payload)
    {
        if (payload is null)
            return;

        if (!IsSupportType(payload.GetType()))
            return;

        if (_stateStorage is not null)
           await _stateStorage.PersistAsync(payload, _cancellationTokenSource.Token);

        await _channel.Writer.WriteAsync(payload, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Starts the queue processor and all configured item providers.
    /// </summary>
    public void Start()
    {
        _runningTasks.Add(RunProcessorAsync());
        _runningTasks.AddRange(_itemsProviders.Select(RunProviderAsync));
    }

    /// <summary>
    /// Stops processing, cancels all tasks, and completes the queue channel.
    /// </summary>
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

    /// <summary>
    /// Checks whether a given type is supported by the queue based on include/exclude filters.
    /// </summary>
    /// <param name="type">The actual type of the payload.</param>
    /// <returns> <see langword="true"/> if the type is supported, otherwise <see langword="false"/>. </returns>
    public bool IsSupportType(Type type)
    {
        if (type is null)
            return false;

        if (type == typeof(TPayload))
            return true;

        if (_excludedSubTypes.Contains(type))
            return false;

        if (_includedSubTypes.Count > 0 && !_includedSubTypes.Contains(type))
            return false;

        return true;
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
        if (_itemsProcessor is null && _itemsProcessingFunction is null)
            return;

        await foreach (TPayload payload in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
        {
            if (payload is null)
                return;

            Type payloadType = payload.GetType();
            if (!IsSupportType(payloadType))
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

using Tr1ppy.Queries.Abstractions.Proccesors;
using Tr1ppy.Queries.Providers;
using Tr1ppy.Querries.Integration;
using Tr1ppy.Queries.Abstractions.Context;
using System.Runtime.InteropServices;

namespace Tr1ppy.Queries.Tests;

internal class Program
{
    static async Task Main(string[] args)
    {
        var preProcessor = new TestPreProcessor();
        var processor = new TestProcessor();
        var postProcessor = new TestPostProcessor();    

        var provider = new RandomItemProvider<string>(new() { Delay = 1000 });

        var queryBuilder = new QueueConfigurationBuilder<string, int>("object query")
            .WithCapacity(100)
            .AddProvider(provider)
            .AddPreProcessing(preProcessor)
            .AddPostProcessing(postProcessor)
            .SetProcessing(processor);

        var queryConfiguration = queryBuilder.Build();

        var query = new ProccesableQueue<string, int>(queryConfiguration);
        await query.StartAsync();
        await Task.Delay(1001);
        Console.WriteLine(query.ItemsCount);
        Console.ReadKey();

    }

    class TestPreProcessor : IQueuePreProcessor<string>
    {
        public Task PreProcessAsync(QueuePreProcessContext<string> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Queue preprocess, payload: {context.Payload}");
            return Task.CompletedTask;
        }
    }

    class TestProcessor : IQueueProcessor<string, int>
    {
        public Task<int> ProcessAsync(QueueProcessContext<string> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Queue process, payload: {context.Payload}");
            return Task.FromResult(1);
        }
    }

    class TestPostProcessor : IQueuePostProcessor<string, int>
    {
        public async Task PostProcessAsync(QueuePostProcessContext<string, int> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Queue postprocessor, payload: {context.Payload}, result: {context.Result}");

            await Task.Delay(10000);
        }
    }


    static void TestPreProcessingAction(QueuePreProcessContext<string> ctx)
    {
        Console.WriteLine("Preprocessing action");
    }

    static int TestProcessingAction(QueueProcessContext<string> ctx)
    {
        Console.WriteLine("Processing action");
        return 1;
    }

    static void TestPostProcessingAction(QueuePostProcessContext<string, int> ctx)
    {
        Console.WriteLine("Postprocessing action");
    }
}

using Tr1ppy.Queries.Abstractions.Proccesors;
using Tr1ppy.Queries.Providers;
using Tr1ppy.Querries.Integration;
using Tr1ppy.Queries.Abstractions.Context;
using System.Runtime.InteropServices;
using AutoFixture;
using Tr1ppy.Queries.Integration;

namespace Tr1ppy.Queries.Tests;

internal class Program
{
    static async Task Main(string[] args)
    {
        var typedBuilder = new TypedQueryConfigurationBuilder<string, int>(string.Empty)
            .AddPreProcessor<TestPreProcessor<string>>()
            .SetProcessor<TestProcessor<string, int>>()
            .AddPostProcessor<TestPostProcessor<string, int>>();



        //var preProcessor = new TestPreProcessor<object>();
        //var processor = new TestProcessor<object, int>();
        //var postProcessor = new TestPostProcessor<object, int>();    
        //var provider = new RandomItemProvider<object>(new() { Delay = 0 });
        //var queryBuilder = new QueueConfigurationBuilder<object, int>("object query1")
        //    .WithCapacity(100)
        //    .AddPreProcessing(preProcessor)
        //    .AddPostProcessing(postProcessor)
        //    .SetProcessing(processor);

        //var queryConfiguration1 = queryBuilder.Build();
        //var query1 = new ProccesableQueue<object, int>(queryConfiguration1);

        //var preProcessor2 = new TestPreProcessor<object>();
        //var processor2 = new TestProcessor<object, int>();
        //var postProcessor2 = new TestPostProcessor<object, int>();
        //var provider2 = new RandomItemProvider<object>(new() { Delay = 0 });
        //var queryBuilder2 = new QueueConfigurationBuilder<object, int>("object query2")
        //    .WithCapacity(100)
        //    .ConfigureTypes(opts =>
        //    {
        //        opts.IncludeSubtype<string>();
        //    })
        //    .AddPreProcessing(preProcessor2)
        //    .AddPostProcessing(postProcessor2)
        //    .SetProcessing(processor2);

        //var queryConfiguration2 = queryBuilder2.Build();
        //var query2 = new ProccesableQueue<object, int>(queryConfiguration2);

        //List<ProccesableQueue<object, int>> list = new() { query2, query1 };
        //var queryAccessor = new QueryAccessor<object, int>(list);

        //await queryAccessor.EnqueueAsync(string.Empty);
        //await queryAccessor.EnqueueAsync(new object());
        //await queryAccessor.EnqueueAsync(new object(), "object query1");
    }

    class TestPreProcessor<TPayload> : IQueuePreProcessor<TPayload>
    {
        public Task PreProcessAsync(QueuePreProcessContext<TPayload> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Queue preprocess, payload: {context.Payload}");
            Console.WriteLine(context.ProcessContext.Count);
            return Task.CompletedTask;
        }
    }

    class TestProcessor<TPayload, TResult> : IQueueProcessor<TPayload, TResult>
    {
        public Task<TResult> ProcessAsync(QueueProcessContext<TPayload> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Queue process, payload: {context.Payload}");
            var fixture = new Fixture();

            return Task.FromResult(fixture.Create<TResult>());
        }
    }

    class TestPostProcessor<TPayload, TResult> : IQueuePostProcessor<TPayload, TResult>
    {
        public Task PostProcessAsync(QueuePostProcessContext<TPayload, TResult> context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Queue postprocessor, payload: {context.Payload}, result: {context.Result}");
            return Task.CompletedTask;
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

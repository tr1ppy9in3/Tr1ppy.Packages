using System.Collections.Concurrent;

using Tr1ppy.Queries.Abstractions;
using Tr1ppy.Queries.Abstractions.Proccesors;
using Tr1ppy.Queries.Proccesors.Postproccesors;
using Tr1ppy.Queries.Proccesors.Preprocessors;
using Tr1ppy.Queries.Providers;

namespace Tr1ppy.Queries.Tests;

internal class Program
{
    const int itemsPerWriter = 10;
    const int writersCount = 3;
    const int totalMessages = itemsPerWriter * writersCount;

    static async Task Main(string[] args)
    {
        //var source = new InMemoryQueueSource<string>();


        //_ = Task.Run(async () =>
        //{
        //    for (int i = 1; i <= 5; i++)
        //    {
        //        await source.SaveAsync($"Command {i}");
        //        Console.WriteLine(i);
        //    }
        //});

        //_ = Task.Run(async () =>
        //{
        //    for (int i = 5; i <= 10; i++)
        //    {
        //        await source.SaveAsync($"Command {i}");
        //        Console.WriteLine(i);
        //    }
        //});

        //_ = Task.Run(async () =>
        //{
        //    for (int i = 15; i <= 20; i++)
        //    {
        //        await source.SaveAsync($"Command {i}");
        //        Console.WriteLine(i);
        //    }
        //});



        //var reader1 = Task.Run(async () => 
        //{

        //    await foreach (var item in source.ReadAsync())
        //    {
        //        Console.WriteLine($"[Reader 1] Readed {item.Id}");
        //    }
        //});

        //var reader2 = Task.Run(async () =>
        //{

        //    await foreach (var item in source.ReadAsync())
        //    {
        //        Console.WriteLine($"[Reader 2] Readed {item.Id}");
        //    }
        //});

        //await Task.WhenAll(reader1 , reader2);

        var settings = new RandomItemProviderSettings() { Delay = 1000 };
        var provider = new RandomItemProvider<string>(settings);
        var loggingPreProccesor = new LoggingPrepocessor<string>();
        var loggingPreProccesor2 = new LoggingPrepocessor<string>();
        var loggingPostProccesor = new LoggingPostProcessor<string, int>();
        var loggingPostProccesor2 = new LoggingPostProcessor<string, int>();

        var queueEngine = new ProccesableQueue<string, int>
        (
            name: "First queue",
            capacity: 100,
            queueStateStorage: null,
            proccessor: new StringProccesor(),
            preProcessors: [loggingPreProccesor, loggingPreProccesor2],
            postProcessors: [loggingPostProccesor, loggingPostProccesor2],
            itemsProvider: [provider]
        );

        await queueEngine.StartAsync();
        Console.ReadLine();
    }
}


class StringProccesor : IQueueProcessor<string, int>
{
    public async Task<int> ProcessAsync(string payload, CancellationToken cancellationToken)
    {
        Console.WriteLine("Processing" + payload);

        await Task.Delay(5000);
        return 1;
    }
}



using System.Collections.Concurrent;

using Tr1ppy.Queries.Abstractions;

namespace Tr1ppy.Queries.Tests;

internal class Program
{
    const int itemsPerWriter = 10;
    const int writersCount = 3;
    const int totalMessages = itemsPerWriter * writersCount;

    static async Task Main(string[] args)
    {
        var source = new InMemoryQueueSource<string>();

        
        _ = Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                await source.SaveAsync($"Command {i}");
                Console.WriteLine(i);
            }
        });

        _ = Task.Run(async () =>
        {
            for (int i = 5; i <= 10; i++)
            {
                await source.SaveAsync($"Command {i}");
                Console.WriteLine(i);
            }
        });

        _ = Task.Run(async () =>
        {
            for (int i = 15; i <= 20; i++)
            {
                await source.SaveAsync($"Command {i}");
                Console.WriteLine(i);
            }
        });



        var reader1 = Task.Run(async () => 
        {

            await foreach (var item in source.ReadAsync())
            {
                Console.WriteLine($"[Reader 1] Readed {item.Id}");
            }
        });

        var reader2 = Task.Run(async () =>
        {

            await foreach (var item in source.ReadAsync())
            {
                Console.WriteLine($"[Reader 2] Readed {item.Id}");
            }
        });

        await Task.WhenAll(reader1 , reader2);
    }
}

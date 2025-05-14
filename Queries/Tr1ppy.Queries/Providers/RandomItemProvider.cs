using AutoFixture;
using Tr1ppy.Queries.Abstractions;

namespace Tr1ppy.Queries.Providers;

public class RandomItemProvider<TPaylod> : IQueueItemsProvider<TPaylod>
{
    private readonly RandomItemProviderSettings _settings;

    public RandomItemProvider(RandomItemProviderSettings settings)
    {
        _settings = settings;
    }

    public async IAsyncEnumerable<TPaylod> GetAsync(QueueContext context, CancellationToken cancellationToken = default)
    {
        var fixture = new Fixture();

        while (!cancellationToken.IsCancellationRequested)
        {
            var fixtureValue = fixture.Create<TPaylod>();
            Console.WriteLine($"[{context.QueueName}] {context.Count.Value} Produced " + fixtureValue!.ToString());

            context.Count.Increment();
            yield return fixtureValue;

            await Task.Delay(_settings.Delay, cancellationToken);
            await Task.Yield();
        }
    }
}

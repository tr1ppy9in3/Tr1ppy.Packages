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

    public async IAsyncEnumerable<TPaylod> GetAsync(CancellationToken cancellationToken = default)
    {
        var fixture = new Fixture();

        while (!cancellationToken.IsCancellationRequested)
        {
            var fixtureValue = fixture.Create<TPaylod>();
            Console.WriteLine("Produced " + fixtureValue!.ToString());
            yield return fixtureValue;

            await Task.Delay(_settings.Delay, cancellationToken);
            await Task.Yield();
        }
    }
}

using AutoFixture;

using System.Runtime.CompilerServices;

using Tr1ppy.Queries.Abstractions;

namespace Tr1ppy.Queries.Providers;

public class RandomItemProvider<TPayload> : BaseItemsProvider<TPayload>
{
    private readonly RandomItemProviderSettings _settings;

    public RandomItemProvider(RandomItemProviderSettings settings)
    {
        _settings = settings;
    }


    protected override async IAsyncEnumerable<TPayload> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var fixture = new Fixture();

        while (!cancellationToken.IsCancellationRequested)
        {
            var fixtureValue = fixture.Create<TPayload>();

            yield return fixtureValue;

            await Task.Delay(_settings.Delay, cancellationToken);
            await Task.Yield();
        }
    }
}

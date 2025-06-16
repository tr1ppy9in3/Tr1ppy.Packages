using Tr1ppy.Services.Attributes;

namespace Tr1ppy.Services.Tests;

[Keyed("allah")]
[OnPlatform(SupportedOSPlatform.All)]
[AutoregisteredService(ServiceLifetime.Scoped)]
internal class Foo : IFoo
{
    public void Bar()
    {
        Console.WriteLine("BAR");
    }
}

using Tr1ppy.Services.Attributes;
using Tr1ppy.Services.Attributes.Descriptions;

namespace Tr1ppy.Services.Tests;

[OnPlatform(SupportedOSPlatform.All)]
[AutoregisteredService(ServiceLifetime.Scoped)]
internal class Foo : BaseFoo
{
    public override void Bar()
    {
        Console.WriteLine("BAR");
    }
}

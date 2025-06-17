using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

using Tr1ppy.Services.Integration.DependencyInjection;

namespace Tr1ppy.Services.Tests;

internal class Program
{
    static void Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddAttributedServices(assemblies: Assembly.GetExecutingAssembly());

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var foo = scope.ServiceProvider.GetRequiredService<IFoo>();
        foo.Bar();
    }
}

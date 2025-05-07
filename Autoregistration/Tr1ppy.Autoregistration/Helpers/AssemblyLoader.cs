using System.Reflection;

using Tr1ppy.Autoregistration.Services;

namespace Tr1ppy.AutoFac.Helpers;

public static class AssemblyLoader
{
    private static readonly AppDomain 
        CurrentDomain = AppDomain.CurrentDomain;

    public static IEnumerable<Assembly> ProjectAssembliesFromApp(AssemblySearchFilterOptions? options = default)
    {
        List<Assembly> assemblies = new();

        foreach (var dllPath in Directory.GetFiles(CurrentDomain.BaseDirectory, "*.dll"))
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            assemblies.Add(assembly);
        }

        return FilterAssemblies(assemblies, options);
    }

    private static List<Assembly> FilterAssemblies
    (
        List<Assembly> assemblies,
        AssemblySearchFilterOptions? options
    )
    {
        if (options is null)
            return assemblies;

        return [
        .. assemblies.Where(assembly =>
        {
            var name = assembly.GetName().Name ?? string.Empty;

            bool isIncluded = options.IncludedDomains.Count == 0 || options.IncludedDomains.Any(domain => name.Contains(domain));
            bool isExcluded = options.ExcludedDomains.Any(domain => name.Contains(domain));

            return isIncluded && !isExcluded;
        })];
    }
}

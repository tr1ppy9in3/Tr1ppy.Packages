using System.Reflection;

namespace Tr1ppy.AutoFac.Helpers;

public static class AssemblyLoader
{
    private static readonly AppDomain 
        CurrentDomain = AppDomain.CurrentDomain;

    public static IEnumerable<Assembly> ProjectAssembliesFromApp(AssemblyDomainFilterOptions? options = default)
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
        AssemblyDomainFilterOptions? options
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

public class AssemblyDomainFilterOptions
{
    public List<string> IncludedDomains { get; } = new();
    public List<string> ExcludedDomains { get; } = new();

    public AssemblyDomainFilterOptions IncludeDomainName(string domainName)
    { 
        IncludedDomains.Add(domainName);
        return this; 
    }

    public AssemblyDomainFilterOptions IncludeDomainNames(params string[] domainNames)
    { 
        IncludedDomains.AddRange(domainNames); 
        return this; 
    }

    public AssemblyDomainFilterOptions ExcludeDomainName(string domainName)
    { 
        ExcludedDomains.Add(domainName); 
        return this; 
   }

    public AssemblyDomainFilterOptions ExcludeDomainNames(string[] domainNames)
    {
        ExcludedDomains.AddRange(domainNames);
        return this;
    } 

    public AssemblyDomainFilterOptions IncludeCurrentDomainName(char? splitBy = default)
    {
        string? currentDomainName = Assembly.GetEntryAssembly()?.GetName().Name;
        if (string.IsNullOrWhiteSpace(currentDomainName))
            return this;

        var domains = splitBy is not null
            ? currentDomainName.Split(splitBy.Value, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : [currentDomainName];

        foreach (var domain in domains)
        {
            if (!IncludedDomains.Contains(domain))
                IncludedDomains.Add(domain);
        }

        return this;
    }
}
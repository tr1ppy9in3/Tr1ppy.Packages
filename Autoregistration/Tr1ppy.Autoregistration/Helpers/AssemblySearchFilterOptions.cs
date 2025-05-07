using System.Reflection;

namespace Tr1ppy.Autoregistration.Services;

public class AssemblySearchFilterOptions
{
    public List<string> IncludedDomains { get; } = new();
    public List<string> ExcludedDomains { get; } = new();

    public AssemblySearchFilterOptions IncludeDomainName(string domainName)
    {
        IncludedDomains.Add(domainName);
        return this;
    }

    public AssemblySearchFilterOptions IncludeDomainNames(params string[] domainNames)
    {
        IncludedDomains.AddRange(domainNames);
        return this;
    }

    public AssemblySearchFilterOptions ExcludeDomainName(string domainName)
    {
        ExcludedDomains.Add(domainName);
        return this;
    }

    public AssemblySearchFilterOptions ExcludeDomainNames(string[] domainNames)
    {
        ExcludedDomains.AddRange(domainNames);
        return this;
    }

    public AssemblySearchFilterOptions IncludeCurrentDomainName(char? splitBy = default)
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

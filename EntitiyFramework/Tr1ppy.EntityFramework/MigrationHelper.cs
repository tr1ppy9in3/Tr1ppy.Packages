
using Microsoft.Extensions.Configuration;

namespace Tr1ppy.EntityFramework;

public static class MigrationHelper
{
    private static string _dbProviderDefaultKey = "DbProvider";

    public static (DbProvider, string) FindSettings()
    {
        bool isSrc = false;
        var currentDirectory = Directory.GetCurrentDirectory();

        while (currentDirectory is not null && isSrc != true)
        {
            if (string.Equals(Path.GetFileName(currentDirectory), "src", StringComparison.OrdinalIgnoreCase))
            {
                isSrc = true;
            }

            var subDirectories = Directory.GetDirectories(currentDirectory, "*", SearchOption.AllDirectories);
            foreach (var subDir in subDirectories)
            {
                var result = FindSettingsInDirectory(subDir);
                if (result is not null)
                    return result.Value;
            }

            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        throw new ArgumentNullException("connectionString", "Строка подключения не найдена.");
    }

    private static (DbProvider, string)? FindSettingsInDirectory(string directory)
    {
        var configFilePath = Path.Combine(directory, "appsettings.json");

        if (!File.Exists(configFilePath))
            return null;

        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile(configFilePath)
                .Build();

        var dbProviderString = configuration.GetSection(_dbProviderDefaultKey).Value;
        if (dbProviderString is null)
            throw new ArgumentNullException("DbProviderString");
        
        if (!Enum.TryParse(dbProviderString, out DbProvider dbProvider))
            throw new ArgumentNullException("DbProvider");

        var connectionString = configuration.GetConnectionString(dbProviderString.ToString());
        if (connectionString is null)
            throw new ArgumentNullException("ConnectionString");

        return (dbProvider, connectionString);
    }

}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tr1ppy.EntityFramework;

public abstract class DbContextInternalOptionsBuilder<TContext, TBuilder> 
    where TBuilder : DbContextInternalOptionsBuilder<TContext, TBuilder>
    where TContext : DbContext
{
    public bool IsNeedConfiguration { get; set; } = false;

    #region Fields

    private string _customConnectionString = default;
    private Action<DbContextOptionsBuilder>? _configureDbContextOptions = default;

    #region DbProvider

    private DbProvider? _dbProvider = default;
    private string? _dbProviderConfigurationPath = default;
    private const string _dbProviderConfigurationPathDefault = "DbProvider";

    #endregion

    #endregion

    #region Public methods

    #region Fluent API

    public TBuilder WithCustomConnectionString(string customConnectionString)
    {
        _customConnectionString = customConnectionString;
        return (TBuilder)this;
    }

    public TBuilder WithProvider(DbProvider dbProvider)
    {
        _dbProvider = dbProvider;
        return (TBuilder)this;
    }

    public TBuilder WithProviderFromConfiguration(string configurationPath)
    {
        _dbProviderConfigurationPath = configurationPath;
        IsNeedConfiguration = true;
        return (TBuilder)this;
    }

    public TBuilder Configure(Action<DbContextOptionsBuilder> action)
    {
        _configureDbContextOptions = action;
        return (TBuilder)this;
    }

    #endregion

    public DbContextOptions<TContext> Build(IConfiguration? configuration)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        if (IsNeedConfiguration && configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        var connectionString = ExtractConnectionString(configuration);
        var dbProvider = ExtractDbProvider(configuration);

        optionsBuilder.ApplyProvider(dbProvider, connectionString);
        _configureDbContextOptions?.Invoke(optionsBuilder);

        return optionsBuilder.Options;
;    }

    #endregion

    #region Private methods

    private string ExtractConnectionString(IConfiguration? configuration)
    {
        if (!string.IsNullOrEmpty(_customConnectionString))
        {
            return _customConnectionString;
        }

        if (configuration is not null)
        {
            var dbProvider = ExtractDbProvider(configuration);
            var connectionString = configuration.GetConnectionString(dbProvider.ToString());

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }
        }

        throw new ArgumentNullException("connectionString", "Не удалось найти строку соединения!");
    }

    private DbProvider ExtractDbProvider(IConfiguration? configuration)
    {

        if (_dbProvider.HasValue)
        {
            return _dbProvider.Value;
        }

        if (!string.IsNullOrEmpty(_dbProviderConfigurationPath))
        {
            var section = configuration?.GetSection(_dbProviderConfigurationPath);
            if (Enum.TryParse(section?.Value, out DbProvider selectedProvider))
            {
                return selectedProvider;
            }

            throw new InvalidOperationException($"Не удалось получить DbProvider из конфигурации по пути: {_dbProviderConfigurationPath}");
        }

        else if (IsNeedConfiguration)
        {
            var section = configuration!.GetSection(_dbProviderConfigurationPathDefault);
            if (Enum.TryParse(section.Value, out DbProvider defaultProvider))
            {
                return defaultProvider;
            }

            throw new InvalidOperationException($"Не указан DbProvider и отсутствует в конфигурации по стандартному ключу {_dbProviderConfigurationPathDefault}");
        }

        throw new ArgumentNullException("Не удалось найти провайдера БД!");
    }
    
    #endregion
}


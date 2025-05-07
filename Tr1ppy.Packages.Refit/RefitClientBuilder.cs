using Refit;
using Microsoft.Extensions.Configuration;

namespace Tr1ppy.Packages.Refit;

public abstract class RefitClientBuilder<TClient, TBuilder, TDependencyResolver>
    where TClient : class
    where TBuilder: RefitClientBuilder<TClient, TBuilder, TDependencyResolver>
{
    #region Fields

    protected TDependencyResolver _dependencyResolver

    protected string? _httpClientName = default;
    protected Action<HttpClient>? _configureHttpClient = default;

    protected Uri? _baseAddress = default;
    protected RefitSettings? _settings = default;
    protected string? _connectionStringKey = default;

    #endregion

    protected RefitClientBuilder(TDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
    }

    #region Fluent API

    public TBuilder WithHttpClientName(string httpClinetName)
    {
        _httpClientName = httpClinetName;
        return (TBuilder)this;  
    }

    public TBuilder ConfigureHttpClient(Action<HttpClient> configureHttpClient)
    {
        _configureHttpClient = configureHttpClient;
        return (TBuilder)this;
    }

    public TBuilder WithBaseAddress(Uri baseAddress)
    {
        _baseAddress = baseAddress;
        return (TBuilder)this;
    }

    public TBuilder WithConnectionStringFromConfiguration(string connectionStringKey)
    {
        _connectionStringKey = connectionStringKey;
        return (TBuilder)this;
    }

    public TBuilder WithRefitSettings(RefitSettings settings)
    {
        _settings = settings;
        return (TBuilder)this;
    }

    #endregion

    public abstract TDependencyResolver Register();

    protected HttpClient CreateHttpClient(IHttpClientFactory httpClientFactory)
    {
        string httpClientName = ResolveHttpClientName();
        HttpClient httpClient = httpClientFactory.CreateClient(httpClientName);

        _configureHttpClient!(httpClient);
        return httpClient;
    }

    private string ResolveHttpClientName()
    {
        return string.IsNullOrWhiteSpace(_httpClientName)
            ? typeof(TClient).FullName!
            : _httpClientName;
    }

    protected void ResolveConnectionString(IConfiguration configuration)
    {
        if (_connectionStringKey is not null)
        {
            var connectionString = configuration.GetConnectionString(_connectionStringKey);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, $"Can't find connection string for ${_connectionStringKey}");

            _baseAddress = new Uri(connectionString);
        }
    }
}

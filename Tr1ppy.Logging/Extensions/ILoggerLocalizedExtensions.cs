using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace Tr1ppy.Logging.Extensions;

public static class ILoggerLocalizedExtensions
{
    public static void Log<T>
    (
        this ILogger<T> logger, 
        LogLevel logLevel, 
        IStringLocalizer localizer, 
        Enum key, 
        params object[] args
    )
    {
        var message = localizer[key.ToString(), args];
        logger.Log(logLevel, message, args);
    }

    public static void Log<T>
    (
        this ILogger<T> logger,
        LogLevel logLevel,
        IStringLocalizer localizer,
        string key,
        params object[] args
    )
    {
        var message = localizer[key, args];
        logger.Log(logLevel, message, args);
    }

    public static void LogInformation<T>
    (
        this ILogger<T> logger, 
        IStringLocalizer localizer, 
        Enum key, 
        params object[] args
    )
    {
        var message = localizer[key.ToString(), args];
        logger.LogInformation(message, args);
    }

    public static void LogInformation<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        string key,
        params object[] args
    )
    {
        var message = localizer[key, args];
        logger.LogInformation(message, args);
    }

    public static void LogWarning<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        Enum key,
        params object[] args
    )
    {
        var message = localizer[key.ToString(), args];
        logger.LogWarning(message, args);
    }

    public static void LogWarning<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        string key,
        params object[] args
    )
    {
        var message = localizer[key, args];
        logger.LogWarning(message, args);
    }

    public static void LogDebug<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        Enum key,
        params object[] args
    )
    {
        var message = localizer[key.ToString(), args];
        logger.LogWarning(message, args);
    }

    public static void LogDebug<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        string key,
        params object[] args
    )
    {
        var message = localizer[key, args];
        logger.LogWarning(message, args);
    }

    public static void LogTrace<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        Enum key,
        params object[] args
    )
    {
        var message = localizer[key.ToString(), args];
        logger.LogWarning(message, args);
    }

    public static void LogTrace<T>
    (
        this ILogger<T> logger,
        IStringLocalizer localizer,
        string key,
        params object[] args
    )
    {
        var message = localizer[key, args];
        logger.LogWarning(message, args);
    }
}

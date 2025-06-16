using System.Text;

namespace Tr1ppy.System.Abstractions.Errors;

public class Error<TErrorEnum> where TErrorEnum : Enum
{
    public TErrorEnum Code { get; private set; }
    public string Message { get; private set; } 
    public Exception? InnerException { get; private set; }

    private Dictionary<string, object>? _details; 

    public IReadOnlyDictionary<string, object>? Details => _details; 
    public object? InnerError { get; private set; } 

    private Error(TErrorEnum code, string message)
    {
        Code = code;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }   

    /// <summary>
    /// Создает новый экземпляр ошибки с указанным кодом и сообщением.
    /// </summary>
    public static Error<TErrorEnum> From(TErrorEnum code, string message)
    {
        return new Error<TErrorEnum>(code, message);
    }

    /// <summary>
    /// Создает новый экземпляр ошибки, используя исключение как источник.
    /// </summary>
    public static Error<TErrorEnum> FromException(TErrorEnum code, Exception exception, string? message = null)
    {
        return new Error<TErrorEnum>(code, message ?? exception.Message)
                   .WithInnerException(exception);
    }

    /// <summary>
    /// Создает новый экземпляр ошибки, оборачивая другую ошибку.
    /// </summary>
    public static Error<TErrorEnum> FromInnerError(TErrorEnum code, object innerError, string? message = null)
    {
        return new Error<TErrorEnum>(code, message ?? $"An internal error occurred: {innerError}")
                   .WithInnerError(innerError);
    }

    public Error<TErrorEnum> WithMessage(string message)
    {
        Message = message;
        return this;
    }

    public Error<TErrorEnum> WithCode(TErrorEnum code)
    {
        Code = code;
        return this;
    }

    /// <summary>
    /// Добавляет внутреннее исключение к ошибке.
    /// </summary>
    public Error<TErrorEnum> WithInnerException(Exception? innerException)
    {
        InnerException = innerException;
        return this;
    }

    /// <summary>
    /// Добавляет одну деталь к ошибке.
    /// </summary>
    public Error<TErrorEnum> WithDetail(string key, object value)
    {
        _details ??= new Dictionary<string, object>(); 
        _details[key] = value;
        return this;
    }

    /// <summary>
    /// Добавляет коллекцию деталей к ошибке.
    /// </summary>
    public Error<TErrorEnum> WithDetails(IDictionary<string, object>? details)
    {
        if (details == null) return this;

        _details ??= new Dictionary<string, object>();
        foreach (var item in details)
        {
            _details[item.Key] = item.Value;
        }
        return this;
    }

    /// <summary>
    /// Оборачивает другую ошибку как внутреннюю ошибку.
    /// </summary>
    public Error<TErrorEnum> WithInnerError(object? innerError)
    {
        InnerError = innerError;
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Error Code: {Code}");
        sb.AppendLine($"Message: {Message}");

        if (InnerException is not null)
        {
            sb.AppendLine($"Inner Exception: {InnerException.GetType().Name} - {InnerException.Message}");
        }

        if (InnerError is not null)
        {
            sb.AppendLine($"Inner Error: {InnerError.ToString()}"); 
        }

        if (Details is not null && Details.Count > 0)
        {
            sb.AppendLine("Details:");
            foreach (var detail in Details)
            {
                sb.AppendLine($"- {detail.Key}: {detail.Value}");
            }
        }

        return sb.ToString();
    }
}
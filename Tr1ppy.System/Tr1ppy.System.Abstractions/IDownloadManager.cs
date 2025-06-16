using Kontur.Results;

using Tr1ppy.System.Abstractions.Errors;

namespace Tr1ppy.System.Abstractions;

public interface IDownloadManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="destinitationDirectory"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public Task<Result<Error<DownloadErrorEnum>, string>> DownloadFileAsync(
        string url,
        string destinitationDirectory,
        DownloadFileOptions? options = default
    );
}


/// <summary>
/// 
/// </summary>
/// <param name="FileName"></param>
/// <param name="LimitSizeInBytes"></param>
/// <param name="AllowedExtensions"></param>
/// <param name="AllowedMimeTypes"></param>
public record DownloadFileOptions(
    bool? Overwrite = default,
    string? FileName = default,
    long? LimitSizeInBytes = default,
    string[]? AllowedExtensions  = default,
    string[]? RejectedExtensions = default,
    string[]? AllowedMimeTypes = default,
    string[]? RejectedMimeTypes = default
);
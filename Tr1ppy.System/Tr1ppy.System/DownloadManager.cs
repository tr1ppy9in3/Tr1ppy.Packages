//using Kontur.Results;

//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//using SharpCompress.Common;

//using System.IO;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Net.NetworkInformation;

//using Tr1ppy.System.Abstractions;
//using Tr1ppy.System.Abstractions.Errors;

//namespace Tr1ppy.System;

//public sealed class DownloadManager : IDownloadManager
//{
//    private readonly IFileManager _fileManager;
//    private readonly IDirectoryManager _directoryManager;
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly ILogger<DownloadManager> _logger;


//    public DownloadManager(
//        IFileManager fileManager, 
//        IDirectoryManager directoryManager, 
//        IHttpClientFactory httpClientFactory, 
//        ILogger<DownloadManager> logger
//    )
//    {
//        _fileManager = fileManager
//            ?? throw new ArgumentNullException(nameof(fileManager));

//        _directoryManager = directoryManager
//            ?? throw new ArgumentNullException(nameof(directoryManager));

//        _httpClientFactory = httpClientFactory
//            ?? throw new ArgumentNullException(nameof(httpClientFactory));

//        _logger = logger 
//            ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task<Result<Error<DownloadErrorEnum>, string>> DownloadFileAsync(
//        string url, 
//        string destinitationDirectory,
//        DownloadFileOptions? options = null
//    )
//    {
//        Error<DownloadErrorEnum> error;

//        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) || !(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
//        {
//            _logger.LogError("DownloadFileAsync: Provided URL is invalid or unsupported: '{Url}'.", url);
//            return Error<DownloadErrorEnum>.From(DownloadErrorEnum.InvalidUrl, $"Invalid or unsupported URL: '{url}'.");
//        }

//        if (string.IsNullOrWhiteSpace(destinitationDirectory))
//        {
//            _logger.LogError("DownloadFileAsync: Destination directory cannot be null or empty for URL '{Url}'.", url);
//            return Error<DownloadErrorEnum>.From(DownloadErrorEnum.InvalidUrl, "Destination directory cannot be null or empty.").WithDetail("Url", url);
//        }

//        var creatingDirectoryResult = _directoryManager.EnsureExists(destinitationDirectory, EnsurePolicy.AttemptToAchieveState);
//        if (creatingDirectoryResult.TryGetFault(out var creatingDirectoryFault))
//        {
//            _logger.LogError("DownloadFileAsync: Destination directory is not exists and can't create for URL '{Url}'.", url);
//            return Error<DownloadErrorEnum>.From(DownloadErrorEnum.IOError, "Unable to handle direcotry!");
//        }

//        HttpClient httpClient = _httpClientFactory.CreateClient();
//        HttpContent? file = default;
//        string? fileName = default;

//        try
//        {
//            using HttpRequestMessage headRequest = new(HttpMethod.Head, url);
//            using HttpResponseMessage headResponse = await httpClient.SendAsync(headRequest, HttpCompletionOption.ResponseHeadersRead);
//            headResponse.EnsureSuccessStatusCode();

//            long? contentLength = headResponse.Content.Headers.ContentLength;
//            if (contentLength is not null && contentLength > options?.LimitSizeInBytes)
//            {
//                error = Error<DownloadErrorEnum>.From(code: DownloadErrorEnum.ContentTooLarge, message: $"Content large then {options.LimitSizeInBytes} bytes");

//                _logger.LogError(StandartError(fileName, url, error));
//                return error;
//            }

//            using HttpRequestMessage mainRequest = new(HttpMethod.Get, url);
//            using HttpResponseMessage mainResponse = await httpClient.SendAsync(mainRequest, HttpCompletionOption.ResponseHeadersRead);
//            mainResponse.EnsureSuccessStatusCode();

//            fileName = GetFileNameFromResponse(mainResponse, options);
//            string? extension = Path.GetExtension(fileName);

//            if (options?.RejectedExtensions?.Length > 0 && extension is not null)
//            {
//                if (options.RejectedExtensions.Contains(extension))
//                {
//                    error = Error<DownloadErrorEnum>.From(code: DownloadErrorEnum.InavlidContentType, message: $"File extension '{extension}' is in the rejected list.")
//                        .WithDetail("ActualExtensions", extension)
//                        .WithDetail("RejectedList", string.Join(", ", options.RejectedExtensions));

//                    _logger.LogError(StandartError(fileName, url, error));
//                    return error;
//                }
//            }

//            if (options?.AllowedExtensions?.Length > 0 && extension is not null)
//            {
//               if (!options.AllowedExtensions.Contains(extension))
//               {
//                    error = Error<DownloadErrorEnum>.From(code: DownloadErrorEnum.InavlidContentType, message: $"File extension '{extension}' is not in allowed list.")
//                        .WithDetail("ActualExtensions", extension)
//                        .WithDetail("AllowedList", string.Join(", ", options.AllowedExtensions));

//                    _logger.LogError(StandartError(fileName, url, error));
//                    return error;
//               }
//            }

//            file = new ByteArrayContent(await mainResponse.Content.ReadAsByteArrayAsync());
//        }
//        catch (Exception ex)
//        {
//            error = MapExceptionToDownloadError(ex, url);
//            _logger.LogError(StandartError(fileName, url, error));

//            return error;
//        }

//        fileName ??= "downloaded";
//        string filePath = Path.Combine(destinitationDirectory, fileName!);

//        EnsurePolicy ensurePolicy = (options?.Overwrite == true) ? EnsurePolicy.AttemptToAchieveState : EnsurePolicy.CheckOnlyAndFail;
//        var ensuringFileNotExistsResult = _fileManager.EnsureNotExists(filePath, ensurePolicy);
//        if (ensuringFileNotExistsResult.TryGetFault(out var ensuringFileNotExistsFault))
//        {
//            // TODO
//            _logger.LogError("DownloadFileAsync: Destination directory is not exists and can't create for URL '{Url}'.", url);
//            return Error<DownloadErrorEnum>.From(DownloadErrorEnum.IOError, "Unable to handle direcotry!");
//        }

//        try
//        {
//            _logger.LogTrace("Saving downloaded file to: {FilePath}", filePath);

//            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
//            await file.CopyToAsync(stream);
//        }
//        catch (Exception ex)
//        {
//            error = MapExceptionToDownloadError(ex, url);
//            _logger.LogError(StandartError(fileName, url, error));

//            return error;
//        }

//        _logger.LogInformation("File successfully downloaded from {Url} to {FilePath}", url, filePath);
//        return filePath;
//    }

//    private static string StandartError(string? fileName, string url, Error<DownloadErrorEnum> error) =>
//        $"Unable to download file `{fileName}` from {url}`. Details: {error}";

//    private static string? GetFileNameFromResponse(HttpResponseMessage response, DownloadFileOptions? options = null)
//    {
//        if (options?.FileName is not null)
//            return options.FileName;
        
//        ContentDispositionHeaderValue? contentDisposition = response.Content.Headers.ContentDisposition;
//        string? fileNameFromContentDisposition = GetFileNameFromContentDispisition(contentDisposition);
//        if (fileNameFromContentDisposition is not null)
//            return fileNameFromContentDisposition;

//        string? filenameFromUrl = Path.GetFileName(response.RequestMessage?.RequestUri?.LocalPath);
//        if (!string.IsNullOrEmpty(filenameFromUrl) && filenameFromUrl != "/")
//            return filenameFromUrl;

//        return null;
//    }

//    private static string? GetFileNameFromContentDispisition(ContentDispositionHeaderValue? contentDisposition)
//    {
//        if (!string.IsNullOrEmpty(contentDisposition?.FileNameStar))
//            return contentDisposition.FileNameStar;
        
//        if (!string.IsNullOrEmpty(contentDisposition?.FileName))
//            return contentDisposition.FileName.Trim('"');

//        return null;
//    }

//    private static Error<DownloadErrorEnum> MapExceptionToDownloadError(Exception ex, string url, string? details = null)
//    {
//        var commonDetails = new Dictionary<string, object> { { "Url", url } };
//        if (details != null) commonDetails.Add("Details", details);

//        return ex switch
//        {
//            HttpRequestException hrex when hrex.StatusCode.HasValue =>
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.HttpRequestFailed, hrex, $"HTTP request failed with status code {(int)hrex.StatusCode.Value}.")
//                                        .WithDetails(commonDetails.WithDetail("StatusCode", (int)hrex.StatusCode.Value)),
//            HttpRequestException hrex =>
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.NetworkError, hrex, $"Network error during download: {hrex.Message}")
//                                        .WithDetails(commonDetails),

//            OperationCanceledException oce =>
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.NetworkError, oce, "Download operation was cancelled.")
//                                        .WithDetails(commonDetails),

//            UriFormatException ufe =>
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.InvalidUrl, ufe, $"Invalid URL format: {ufe.Message}")
//                                        .WithDetails(commonDetails),

//            PathTooLongException ptle =>
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.PathTooLong, ptle, $"Destination path is too long: {ptle.Message}")
//                                        .WithDetails(commonDetails),

//            UnauthorizedAccessException uae =>
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.AccessDenied, uae, $"Access denied to destination path: {uae.Message}")
//                                        .WithDetails(commonDetails),

//            IOException ioe => 
//                Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.IOError, ioe, $"An I/O error occurred during file write: {ioe.Message}")
//                                        .WithDetails(commonDetails),

//            _ => Error<DownloadErrorEnum>.FromException(DownloadErrorEnum.UnknownError, ex, "An unexpected error occurred during download.")
//                                        .WithDetails(commonDetails)
//        };
//    }
//}

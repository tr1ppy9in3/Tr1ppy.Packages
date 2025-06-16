namespace Tr1ppy.System.Abstractions.Errors;

public enum DownloadErrorEnum
{
    InvalidUrl,
    NetworkError,
    HttpRequestFailed,
    InavlidContentType,
    ContentTooLarge,
    AccessDenied,
    PathTooLong,
    IOError,
    UnknownError,
    FileNameNotFound,
    FileAlreadyExists,
}

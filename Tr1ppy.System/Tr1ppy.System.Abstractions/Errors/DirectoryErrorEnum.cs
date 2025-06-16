namespace Tr1ppy.System.Abstractions.Errors;

public enum DirectoryErrorEnum
{
    None,

    NotFound,
    AlreadyExists,

    AccessDenied,
    NotEmpty,

    InvalidPath,

    IOError,

    PathTooLong,

    OperationCancelled, 

    UnknownError,

    OperationFailed
}
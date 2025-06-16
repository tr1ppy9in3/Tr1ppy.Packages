namespace Tr1ppy.System.Abstractions.Errors;

public enum FileErrorEnum
{
    NotFound,
    AlreadyExists,

    ACCESS_DENIED,
    INVALID_PATH,
    CANNOT_CREATE,
    OPERATION_CANCELED,
    UNKNOWN
}

using Kontur.Results;

using Microsoft.Extensions.Logging;

using Tr1ppy.System.Abstractions;
using Tr1ppy.System.Abstractions.Errors;
using Tr1ppy.System.Abstractions.Models;
using Tr1ppy.System.Extensions;

namespace Tr1ppy.System;

public sealed class DirectoryManager : IDirectoryManager
{
    private readonly ILogger<DirectoryManager> _logger;

    public DirectoryManager(ILogger<DirectoryManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Public methods

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> EnsureExists(
        string directoryPath,
        EnsurePolicy ensurePolicy = EnsurePolicy.CheckOnlyAndFail,
        FileSystemPermissions? permissions = default
    )
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            _logger.LogError(InvalidPathErrorMessage("EnsureExists", directoryPath));
            return InvalidPathError("EnsureExists", directoryPath);
        }

        if (Directory.Exists(directoryPath))
            return Result.Succeed();
        
        return ensurePolicy switch
        {
            EnsurePolicy.AttemptToAchieveState => Error<DirectoryErrorEnum>.From(
                code: DirectoryErrorEnum.NotFound,
                message: $"Directory '{directoryPath}' does not exist."
            ).WithDetail("Policy", nameof(EnsurePolicy.CheckOnlyAndFail)),

            EnsurePolicy.CheckOnlyAndFail => Create(new(directoryPath, permissions)),
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> EnsureNotExsits(
        string directoryPath, 
        EnsurePolicy ensurePolicy = EnsurePolicy.CheckOnlyAndFail,
        bool isRecursive = false
    )
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            _logger.LogError(InvalidPathErrorMessage("EnsureDeleted", directoryPath));
            return InvalidPathError("EnsureDeleted", directoryPath);
        }

        if (!Directory.Exists(directoryPath))
            return Result.Succeed();

        return ensurePolicy switch
        {
            EnsurePolicy.AttemptToAchieveState => Error<DirectoryErrorEnum>.From(
                code: DirectoryErrorEnum.AlreadyExists,
                message: $"Directory '{directoryPath}' exists but should not according to policy."
            ).WithDetail("Policy", nameof(EnsurePolicy.CheckOnlyAndFail)),

            EnsurePolicy.CheckOnlyAndFail => Remove(new(directoryPath, isRecursive)),
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> Copy(DirectoryCopyData directory)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> CopyRange(IEnumerable<DirectoryCopyData> directories)
        => directories.ExecuteForEach(Copy);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> Create(DirectoryCreateData directory)
    {
        var existsResult = EnsureNotExsits(directory.DirectoryPath);
        if (existsResult.TryGetFault(out Error<DirectoryErrorEnum>? fault))
            return fault;

        try
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(directory.DirectoryPath);
            if (directory.FileSystemPermissions is not null)
            {

            }

        }
        catch (Exception ex)
        {
            return MapExceptionToError(ex, directory.DirectoryPath);
        }

        return Result.Succeed();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> CreateRange(IEnumerable<DirectoryCreateData> directories)
        => directories.ExecuteForEach(Create);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> Remove(DirectoryRemoveData directory)
    {
        var existsResult = EnsureExists(directory.DirectoryPath, EnsurePolicy.CheckOnlyAndFail);
        if (existsResult.TryGetFault(out Error<DirectoryErrorEnum>? fault))
            return fault;

        try
        {
            Directory.Delete(directory.DirectoryPath, directory.IsRecursive);
            _logger.LogInformation(
                message: "Successfully removed directory '{DirectoryPath}'. Recursive: {RecursiveFlag}.", 
                args: [directory.DirectoryPath, directory.IsRecursive]
            );

            return Result.Succeed();
        }
        catch(Exception ex)
        {
            Error<DirectoryErrorEnum> error = MapExceptionToError(ex, directory.DirectoryPath);
            _logger.LogError(
                exception: ex,
                message: "Unable to remove directory {DirectoryPath} with recuvirsive: {RecursiveFlag}. Error code: {ErrorCode}",
                args: [directory.DirectoryPath, directory.IsRecursive, error.Code]
            );

            return error;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> RemoveRange(IEnumerable<DirectoryRemoveData> directories)
        => directories.ExecuteForEach(Remove);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> Rename(DirectoryRenameData directory)
    {
        Error<DirectoryErrorEnum>? fault;

        var existsResult = EnsureExists(directory.DirectoryPath);
        if (existsResult.TryGetFault(out fault))
            return fault;

        if (string.IsNullOrWhiteSpace(directory.NewDirectoryName))
        {
            Error<DirectoryErrorEnum> error = Error<DirectoryErrorEnum>.From(
                code: DirectoryErrorEnum.InvalidPath,
                message: "New directory path cannot be null or empty."
            );

            _logger.LogError(
                message: "Unable to rename directory. Error: {Error}",
                args: error.ToString()
            );

            return error;
        }

        string? parentPath = Path.GetDirectoryName(directory.DirectoryPath);
        string newFullPath = Path.Combine(parentPath ?? string.Empty, directory.NewDirectoryName);

        var notExistsResult = EnsureNotExsits(newFullPath);
        if (notExistsResult.TryGetFault(out fault))
            return fault;

        try
        {
            Directory.Move(directory.DirectoryPath, newFullPath);
            _logger.LogInformation(
                message: "Successfully renamed directory from '{OldPath}' to '{NewPath}'.", 
                args: [directory.DirectoryPath, newFullPath]
            );

            return Result.Succeed();
        }
        catch (Exception ex) 
        {
            fault = MapExceptionToError(ex, directory.DirectoryPath);
            _logger.LogError(
                exception: ex,
                message: "Unable to rename directory {Path} to {NewName}. Error code: {ErrorCode}",
                args: [directory.DirectoryPath, directory.NewDirectoryName, fault.Code]
            );

            return fault;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> RenameRange(IEnumerable<DirectoryRenameData> directories)
        => directories.ExecuteForEach(Rename);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<DirectoryErrorEnum>> RenameRange(IEnumerable<string> directoriesPaths, string newName)
        => directoriesPaths.ExecuteForEach(directoryPath => Rename(new(DirectoryPath: directoryPath, NewDirectoryName: newName)));

    #endregion

    #region Private methods

    /// <summary>
    /// Mapping helping method.
    /// </summary>
    private static Error<DirectoryErrorEnum> MapExceptionToError(Exception ex, string directoryPath)
    {
        var commonDetails = new Dictionary<string, object> { { "DirectoryPath", directoryPath } };

        return ex switch
        {
            OperationCanceledException oce =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.OperationCancelled,
                    exception: oce)
                .WithMessage("Directory operation was cancelled.")
                .WithDetails(commonDetails),

            UnauthorizedAccessException uae =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.AccessDenied,
                    exception: uae)
                .WithMessage($"Access denied for path '{directoryPath}'. Check permissions.")
                .WithDetails(commonDetails),

            DirectoryNotFoundException dnf =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.NotFound,
                    exception: dnf)
                .WithMessage($"Directory '{directoryPath}' not found.")
                .WithDetails(commonDetails),

            PathTooLongException ptle =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.PathTooLong,
                    exception: ptle)
                .WithMessage($"Path '{directoryPath}' is too long.")
                .WithDetails(commonDetails),

            ArgumentException ae =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.InvalidPath,
                    exception: ae)
                .WithMessage($"Invalid argument provided for path '{directoryPath}': {ae.Message}")
                .WithDetails(commonDetails),

            IOException ioe when ioe.HResult == - -2147024891 =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.AlreadyExists,
                    exception: ioe)
                .WithMessage($"Directory '{directoryPath}' already exists.")
                .WithDetails(commonDetails),

            IOException ioe =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.IOError,
                    exception: ioe)
                .WithMessage($"An I/O error occurred for '{directoryPath}': {ioe.Message}")
                .WithDetails(commonDetails),

            _ =>
                Error<DirectoryErrorEnum>.FromException(
                    code: DirectoryErrorEnum.UnknownError,
                    exception: ex)
                .WithMessage($"An unexpected error occurred for '{directoryPath}'.")
                .WithDetails(commonDetails)
        };
    }
    
    /// <summary>
    /// Generating invalid path error.
    /// </summary>
    private static Error<DirectoryErrorEnum> InvalidPathError(string methodName, string receivedPath) =>
       Error<DirectoryErrorEnum>.From(
           code: DirectoryErrorEnum.InvalidPath,
           message: InvalidPathErrorMessage(methodName, receivedPath)
       );

    /// <summary>
    /// Generating invalid path error message.
    /// </summary>
    private static string InvalidPathErrorMessage(string methodName, string receivedPath) =>
        $"{methodName}: Directory path cannot be null or empty. Received: {receivedPath}";

    #endregion
}

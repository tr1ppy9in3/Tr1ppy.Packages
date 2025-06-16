using Kontur.Results;

using Microsoft.Extensions.Logging;

using Tr1ppy.System.Abstractions;
using Tr1ppy.System.Abstractions.Errors;
using Tr1ppy.System.Extensions;

namespace Tr1ppy.System;

/// <summary>
/// 
/// </summary>
public sealed class FileManager : IFileManager
{
    private readonly ILogger<FileManager> _logger;
    private readonly IDirectoryManager _directoryManager;

    public FileManager(ILogger<FileManager> logger, IDirectoryManager directoryManager)
    {
        _directoryManager = directoryManager 
            ?? throw new ArgumentNullException(nameof(directoryManager));

        _logger = logger 
            ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<Error<FileErrorEnum>>> EnsureExistsAsync(
        string filePath, 
        EnsurePolicy policy = EnsurePolicy.CheckOnlyAndFail,
        bool? overwrite = null,
        byte[]? content = null, 
        FileOptions? options = null
    )
    {   
        return policy switch
        {
            EnsurePolicy.CheckOnlyAndFail => Error<FileErrorEnum>.From(
                code: FileErrorEnum.NotFound,
                message: filePath
            ),

            EnsurePolicy.AttemptToAchieveState => await CreateAsync(new(filePath, overwrite, content, options)),

            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> EnsureNotExists(
        string filePath,
        EnsurePolicy policy = EnsurePolicy.CheckOnlyAndFail
    )
    {
        return policy switch
        {
            EnsurePolicy.CheckOnlyAndFail => Error<FileErrorEnum>.From(
                code: FileErrorEnum.AlreadyExists,
                message: filePath
            ),

            EnsurePolicy.AttemptToAchieveState => Remove(filePath),

            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<Error<FileErrorEnum>>> CreateAsync(FileCreationData file)
    {
        Error<FileErrorEnum>? fault = default;

        var ensuringResult = EnsureNotExists(file.FilePath);
        if (ensuringResult.TryGetFault(out fault))
            return fault;

        var ensuringDirectoryCreated = _directoryManager.EnsureExists(Path.GetDirectoryName(file.FilePath), EnsurePolicy.AttemptToAchieveState);
        if (ensuringDirectoryCreated.TryGetFault(out Error<DirectoryErrorEnum>? directoryNotExistsfault))
            return directoryNotExistsfault;

        try
        {
            if (file.Content is null || file.Content.Length == 0)
            {
                using FileStream stream = File.Create(file.FilePath);
            }
            else
            {
                await File.WriteAllBytesAsync(file.FilePath, file.Content);
            }
        }
        catch (Exception ex)
        {

        }

        _logger.LogInformation("");
        return Result.Succeed();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task<Result<Error<FileErrorEnum>>> CreateRangeAsync(IEnumerable<FileCreationData> files)
        => files.ExecuteForEachAsync(CreateAsync);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>, string> Move(FileMoveData file)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> MoveRange(IEnumerable<FileMoveData> files)
        => await files.ExecuteForEach(Move);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> Remove(string filePath)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> RemoveRange(IEnumerable<string> filePaths) 
        => filePaths.ExecuteForEach(Remove);
    

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> Rename(FileRenameData file)
    {
        try
        {

        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> RenameRange(IEnumerable<FileRenameData> files)
        => files.ExecuteForEach(Rename);


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result<Error<FileErrorEnum>> RenameRange(IEnumerable<string> filePaths, string newName)
        => filePaths.ExecuteForEach(filePath => Rename(new(filePath, newName)));
}


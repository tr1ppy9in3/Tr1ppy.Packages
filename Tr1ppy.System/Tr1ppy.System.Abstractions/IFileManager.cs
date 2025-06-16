using Kontur.Results;

using Tr1ppy.System.Abstractions.Errors;
using Tr1ppy.System.Abstractions.Models;

namespace Tr1ppy.System.Abstractions;

public interface IFileManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="policy"></param>
    /// <param name="content"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<Result<FileErrorEnum>> EnsureExistsAsync(
        string filePath, 
        EnsurePolicy policy = EnsurePolicy.CheckOnlyAndFail, 
        byte[]? content = default,
        FileOptions? options = default
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>> EnsureNotExists(
        string filePath,
        EnsurePolicy policy = EnsurePolicy.CheckOnlyAndFail
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<Result<Error<FileErrorEnum>>> CreateAsync(FileCreationData file);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    Task<Result<Error<FileErrorEnum>>> CreateRangeAsync(IEnumerable<FileCreationData> files);
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>> Remove(string filePath);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>> RemoveRange(IEnumerable<string> filePaths);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>> Rename(FileRenameData file);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>> RenameRange(IEnumerable<FileRenameData> files);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePaths"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>> RenameRange(IEnumerable<string> filePaths, string newName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>, string> Move(FileMoveData file);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    Result<Error<FileErrorEnum>, string> MoveRange(IEnumerable<FileMoveData> files);
}

public record FileCreationData(
    string FilePath,
    bool? Overwrite = false,
    byte[]? Content = default, 
    FileOptions? Options = default,
    FileSystemPermissions? Permissions = default
);
public record FileRenameData(string FilePath,string NewName);
public record FileMoveData(string CurrentFilePath,string DestinitationDirectory);

public enum EnsurePolicy
{
    AttemptToAchieveState,
    CheckOnlyAndFail
}
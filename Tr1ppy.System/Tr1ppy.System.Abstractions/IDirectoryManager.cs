using Kontur.Results;

using Tr1ppy.System.Abstractions.Errors;
using Tr1ppy.System.Abstractions.Models;

namespace Tr1ppy.System.Abstractions;

public interface IDirectoryManager
{
    /// <summary>
    /// Ensures that the directory exists at the specified path, according to the specified policy.
    /// </summary>
    /// <param name="directoryPath"> Path to the directory whose existence is to be guaranteed. </param>
    /// <param name="ensurePolicy"> Behavior policy. </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> EnsureExists(
        string directoryPath, 
        EnsurePolicy ensurePolicy = EnsurePolicy.CheckOnlyAndFail,
        FileSystemPermissions? permissions = default
    );

    /// <summary>
    /// Ensures that the directory does not exist at the specified path, according to the specified policy.
    /// </summary>
    /// <param name="directoryPath"> Path to the directory whose not existence is to be guaranteed. </param>
    /// <param name="ensurePolicy"> Behavior policy. </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> EnsureNotExsits(
        string directoryPath, 
        EnsurePolicy ensurePolicy = EnsurePolicy.CheckOnlyAndFail,
        bool isRecursive = false
    );

    /// <summary>
    /// Создает новую директорию.
    /// </summary>
    /// <param name="directory"> An object containing the information for directory create. </param>
    /// <returns>
    /// Результат операции.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> Create(DirectoryCreateData directory);

    /// <summary>
    /// Создает несколько директорий.
    /// </summary>
    /// <param name="directories"> An object collection containing the information for directory create. </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> CreateRange(IEnumerable<DirectoryCreateData> directories);

    /// <summary>
    /// Removes a directory using the provided data.
    /// </summary>
    /// <param name="directory">An object containing the information for directory removal, including recursive option.</param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> Remove(DirectoryRemoveData directory);

    /// <summary>
    /// Removes multiple directories using a collection of removal data for each directory.
    /// </summary>
    /// <param name="directories"> Коллекция информации, необходимой для переименования. </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> RemoveRange(IEnumerable<DirectoryRemoveData> directories);

    /// <summary>
    /// Renames a directory using the provided data.
    /// </summary>
    /// <param name="directory"> An object containing the information for directory rename. </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> Rename(DirectoryRenameData directory);

    /// <summary>
    /// Renames multiple directories using a collection of data.
    /// </summary>
    /// <param name="directories"></param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> RenameRange(IEnumerable<DirectoryRenameData> directories);

    /// <summary>
    /// Renames multiple directories using a collection of data.
    /// </summary>
    /// <param name="directoriesPaths"> Collection of directories path to rename. </param>
    /// <param name="newName"> New name to rename directories in coolection </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> RenameRange(IEnumerable<string> directoriesPaths, string newName);

    /// <summary>
    /// Copies a directory using the provided data.
    /// </summary>
    /// <param name="directory"> An object containing the information for directory copying. </param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> Copy(DirectoryCopyData directory);

    /// <summary>
    /// Copies multiple directories using a collection of data.
    /// </summary>
    /// <param name="directories">An oobject collection containing the information for directory rename.</param>
    /// <returns>
    /// A <see cref="Result{TError}"/> indicating the success of the operation or an <see cref="Error{TError}"/> in case of failure.
    /// </returns>
    public Result<Error<DirectoryErrorEnum>> CopyRange(IEnumerable<DirectoryCopyData> directories);
}

/// <summary>
/// Represents the data required to create a directory.
/// </summary>
/// <param name="DirectoryPath"> The full path to the directory to be created. </param>
/// <param name="FileSystemPermissions"></param>
public record DirectoryCreateData(string DirectoryPath, FileSystemPermissions? FileSystemPermissions = default);

/// <summary>
/// Represents the data required to rename or move a directory.
/// </summary>
/// <param name="DirectoryPath"></param>
/// <param name="NewDirectoryName"></param>
public record DirectoryRenameData(string DirectoryPath, string NewDirectoryName);

/// <summary>
/// Represents the data required to remove a directory.
/// </summary>
/// <param name="DirectoryPath"></param>
/// <param name="IsRecursive">
/// A flag indicating whether to remove the directory recursively, including all its contents.
/// Defaults to <see langword="false"/>.
/// </param>
public record DirectoryRemoveData(string DirectoryPath, bool IsRecursive = false);

/// <summary>
/// Represents the data required to copy a directory.
/// </summary>
/// <param name="SourceDirectoryPath"> The full path to the source directory to be copied. </param>
/// <param name="DestinitationDirectoryPath"> The full path to the target directory where the content will be copied. </param>
public record DirectoryCopyData(string SourceDirectoryPath, string DestinitationDirectoryPath);
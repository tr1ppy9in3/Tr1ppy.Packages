using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

using Microsoft.Extensions.Logging;
using Tr1ppy.System.Abstractions;
using Kontur.Results;
using Tr1ppy.System.Abstractions.Errors;

namespace Tr1ppy.System;

public sealed class ArchiveManager : IArchiveManager
{
    #region Dependency injecion

    private readonly IFileGuard _fileGuard;
    private readonly IDirectoryGuard _directoryGuard;
    private readonly ILogger<ArchiveManager> _logger;

    #endregion

    #region Constructors

    public ArchiveManager(
        IFileGuard fileGuard, 
        IDirectoryGuard directoryGuard, 
        ILogger<ArchiveManager> logger
    )
    {
        _fileGuard = fileGuard
            ?? throw new ArgumentNullException(nameof(fileGuard));

        _directoryGuard = directoryGuard
            ?? throw new ArgumentNullException(nameof(directoryGuard));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region Public methods

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Result<ArchiveErrorEnum> Extract
    (
        string archivePath, 
        string extractionDirectory, 
        ReaderOptions? readerOptions, 
        ExtractionOptions extractionOptions
    )
    {

        var ensureArchiveExistsResult = _fileGuard.EnsureExists(archivePath);
        if (ensureArchiveExistsResult.Failure) 
        { 
        }

        var ensureExtractionDirectoryExists = _directoryGuard.EnsureExists(extractionDirectory);
        if (!ensureExtractionDirectoryExists.TryGetValue(out string? existingExtractionDirectoryPath))
        {
            throw new ArgumentNullException();
        }

        try
        {
            using IArchive archive = ArchiveFactory.Open(archivePath, readerOptions);
            archive.WriteToDirectory(existingExtractionDirectoryPath, extractionOptions);
        }
        catch (Exception ex)
        {
            Error<ArchiveErrorEnum> error = MapExceptionToArchiveError(ex, archivePath, extractionDirectory);
            _logger.LogError
            (
                exception: ex,
                message: "Unable to extract archive {ArchivePath} to {DestinationDirectory}. Error code: {ErrorCode}",
                args: [archivePath, extractionDirectory, error.Code]
            );
            return error;
        }

        return Result.Succeed();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> GetContentPaths
    (
        string archivePath, 
        ReaderOptions? readerOptions
    )
    {
        var ensureArchiveExistsResult = _fileGuard.EnsureExists(archivePath);
        if (ensureArchiveExistsResult.Failure) { }

        try
        {
            using IArchive archive = ArchiveFactory.Open(archivePath, readerOptions);
            List<string> keys = new(archive.Entries.Count());
            
            foreach (IEntry entry in archive.Entries)
            {
                if (entry.Key is not null)
                    keys.Add(entry.Key);
            }

            return keys;
        }
        catch (Exception ex)
        {
            Error<ArchiveErrorEnum> error = MapExceptionToArchiveError(ex, archivePath);
            _logger.LogError
            (
                exception: ex,
                message: "Unable to provide archive contents path {ArchivePath}. Error: {ErrorCode}",
                args: [archivePath, error.Code ]
            );
        }

        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsEmpty(string archivePath, ReaderOptions? readerOptions)
    {
        var ensureArchiveExistsResult = _fileGuard.EnsureExists(archivePath);
        if (ensureArchiveExistsResult.Failure) { }

        try
        {
            using IArchive archive = ArchiveFactory.Open(archivePath, readerOptions);
            return !archive.Entries.Any();
        }
        catch (Exception ex)
        {
            Error<ArchiveErrorEnum> error = MapExceptionToArchiveError(ex, archivePath);
            _logger.LogError
            (
                exception: ex, 
                message: "Unable to check empty archive {ArchivePath}. Error: {ErrorCode}",
                args: [archivePath, error.Code]
            );
        }

        return true;
    }

    #endregion

    private static Error<ArchiveErrorEnum> MapFileGuardErrorToArchiveError(
        Error<FileErrorEnum> error,
        st)

    private static Error<ArchiveErrorEnum> MapExceptionToArchiveError(
        Exception ex, 
        string archivePath, 
        string? extractionDirectory = null
    )
    {
        File.Create()

        var commonDetails = new Dictionary<string, object> { { "ArchivePath", archivePath } };
        if (extractionDirectory is not null) 
            commonDetails.Add("ExtractionDirectory", extractionDirectory);

        File.R()

        return ex switch
        {
            OperationCanceledException oce =>
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.ARCHIVE_OPERATION_CANCELLED, 
                    oce,
                    "Archive operation was cancelled.")
                .WithDetails(commonDetails),

            UnauthorizedAccessException uae =>
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.ARCHIVE_ACCESS_DENIED,
                    uae,
                    "Access to archive or extraction directory is denied.")
                .WithDetails(commonDetails),


            ArgumentException ae => 
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.INVALID_ARGUMENT_PATH,
                    ae,
                    "Invalid argument path provided.")
                .WithDetails(commonDetails),

            NotSupportedException nse =>
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.INVALID_ARGUMENT_PATH,
                    nse,
                    "Operation not supported for the provided path (e.g., device not ready).")
                .WithDetails(commonDetails),

            IOException ioe =>
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.EXTRACTION_WRITE_FAILED,
                    ioe,
                    "An I/O error occurred during archive extraction/reading.")
                .WithDetails(commonDetails),

            ArchiveException ae => 
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.INVALID_OR_CORRUPTED_ARCHIVE,
                    ae,
                    "Archive file is invalid or corrupted.")
                .WithDetails(commonDetails),

            _ => 
                Error<ArchiveErrorEnum>.FromException(
                    ArchiveErrorEnum.UNKNOWN_ARCHIVE_ERROR,
                    ex,
                    "An unexpected error occurred during archive operation.")
                .WithDetails(commonDetails)
        };
    }
}

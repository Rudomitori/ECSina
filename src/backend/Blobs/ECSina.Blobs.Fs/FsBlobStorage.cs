using ECSina.Blobs.Abstractions;
using ECSina.Common.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECSina.Blobs.Fs;

/// <summary>
/// Stores BLOBs in a file system. The root dir is specified by option <see cref="FsBlobStorage.Options.RootDirPath"/>.
/// </summary>
/// <seealso cref="IBlobStorage"/>
public sealed class FsBlobStorage : BaseBlobStorage
{
    // TODO: May be, have to add more logging here

    #region Constructor and dependencies

    private readonly string _rootPath;
    private readonly ILogger<FsBlobStorage> _logger;

    public FsBlobStorage(IOptions<Options> options, ILogger<FsBlobStorage> logger)
    {
        _logger = logger;
        _rootPath =
            options.Value.RootDirPath
            ?? throw new ArgumentException(
                "The root directory path is not specified",
                nameof(options)
            );

        _logger.LogInformation("Blob storage's root dir path: {RootDirPath}", _rootPath);

        EnsureDirExists(_rootPath);
    }

    #endregion

    /// <inheritdoc/>
    public override Task<Stream> OpenRead(
        Guid blobId,
        CancellationToken cancellationToken = default
    )
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var fileInfo = new FileInfo(GetBlobFilePath(blobId));

            if (!fileInfo.Exists)
                throw new CodedException<IBlobStorage.ErrorCodes>(
                    "The BLOB not found",
                    IBlobStorage.ErrorCodes.BlobNotFound
                );

            return Task.FromResult(fileInfo.OpenRead() as Stream);
        }
        catch (Exception e)
        {
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "Internal error",
                IBlobStorage.ErrorCodes.Internal,
                e
            );
        }
    }

    /// <inheritdoc/>
    public override Task<Stream> OpenReadWrite(
        Guid blobId,
        CancellationToken cancellationToken = default
    )
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var fileInfo = new FileInfo(GetBlobFilePath(blobId));

            if (!fileInfo.Exists)
                throw new CodedException<IBlobStorage.ErrorCodes>(
                    "The BLOB not found",
                    IBlobStorage.ErrorCodes.BlobNotFound
                );

            return Task.FromResult(fileInfo.Open(FileMode.Open, FileAccess.ReadWrite) as Stream);
        }
        catch (Exception e)
        {
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "Internal error",
                IBlobStorage.ErrorCodes.Internal,
                e
            );
        }
    }

    /// <inheritdoc/>
    public override Task<Stream> Create(Guid blobId, CancellationToken cancellationToken = default)
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var fileInfo = new FileInfo(GetBlobFilePath(blobId));

            if (fileInfo.Exists)
                throw new CodedException<IBlobStorage.ErrorCodes>(
                    "The BLOB already exists",
                    IBlobStorage.ErrorCodes.BlobAlreadyExists
                );

            return Task.FromResult(fileInfo.Create() as Stream);
        }
        catch (Exception e)
        {
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "Internal error",
                IBlobStorage.ErrorCodes.Internal,
                e
            );
        }
    }

    /// <inheritdoc/>
    public override Task<bool> Exist(Guid blobId, CancellationToken cancellationToken = default)
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var fileInfo = new FileInfo(GetBlobFilePath(blobId));

            return Task.FromResult(fileInfo.Exists);
        }
        catch (Exception e)
        {
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "Internal error",
                IBlobStorage.ErrorCodes.Internal,
                e
            );
        }
    }

    /// <inheritdoc/>
    public override Task Remove(Guid blobId, CancellationToken cancellationToken = default)
    {
        var removed = InnerRemoveIfExists(blobId, cancellationToken);
        if (!removed)
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "The BLOB not found",
                IBlobStorage.ErrorCodes.BlobNotFound
            );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task<bool> RemoveIfExist(
        Guid blobId,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(InnerRemoveIfExists(blobId, cancellationToken));
    }

    private bool InnerRemoveIfExists(Guid blobId, CancellationToken cancellationToken)
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var fileInfo = new FileInfo(GetBlobFilePath(blobId));

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                return true;
            }
            else
                return false;
        }
        catch (Exception e)
        {
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "Internal error",
                IBlobStorage.ErrorCodes.Internal,
                e
            );
        }
    }

    /// <inheritdoc/>
    public override Task Move(
        Guid oldBlobId,
        Guid newBlobId,
        CancellationToken cancellationToken = default
    )
    {
        if (oldBlobId == default)
            throw new ArgumentException(
                $"{nameof(oldBlobId)} must be not default",
                nameof(oldBlobId)
            );

        if (newBlobId == default)
            throw new ArgumentException(
                $"{nameof(newBlobId)} must be not default",
                nameof(newBlobId)
            );

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (File.Exists(GetBlobFilePath(oldBlobId)) is false)
                throw new CodedException<IBlobStorage.ErrorCodes>(
                    "A blob with that id was not found",
                    IBlobStorage.ErrorCodes.BlobNotFound
                );

            if (File.Exists(GetBlobFilePath(newBlobId)))
                throw new CodedException<IBlobStorage.ErrorCodes>(
                    "A blob with the same id already exists",
                    IBlobStorage.ErrorCodes.BlobAlreadyExists
                );

            File.Move(GetBlobFilePath(oldBlobId), GetBlobFilePath(newBlobId));

            return Task.CompletedTask;
        }
        catch (CodedException<IBlobStorage.ErrorCodes>)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new CodedException<IBlobStorage.ErrorCodes>(
                "An inner error has occurred when moving the blob file",
                IBlobStorage.ErrorCodes.Internal,
                e
            );
        }
    }

    private static void EnsureDirExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path!);
    }

    private string GetBlobFilePath(Guid blobId) => Path.Combine(_rootPath, blobId.ToString());

    public sealed class Options
    {
        public required string RootDirPath { get; set; }
    }
}

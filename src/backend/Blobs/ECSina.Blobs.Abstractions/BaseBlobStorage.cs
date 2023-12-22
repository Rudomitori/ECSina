using ECSina.Common.Core.Exceptions;
using ECSina.Common.Core.Extensions;

namespace ECSina.Blobs.Abstractions;

/// <summary>
/// Provides the realisation of several methods of <see cref="IBlobStorage" /> interface,
/// that can be realised as a combination of other methods of the interface.
/// <br />
/// <br />
/// There is not reason to use this class, if all methods will be overrode in an inheritor.
/// </summary>
public abstract class BaseBlobStorage : IBlobStorage
{
    /// <inheritdoc/>
    public abstract Task<Stream> OpenRead(
        Guid blobId,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc/>
    public abstract Task<Stream> OpenReadWrite(
        Guid blobId,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc/>
    public abstract Task<Stream> Create(Guid blobId, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<Stream> CreateOrReplace(
        Guid blobId,
        CancellationToken cancellationToken = default
    )
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await RemoveIfExist(blobId, cancellationToken);
            return await Create(blobId, CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CodedException<IBlobStorage.ErrorCodes>)
        {
            throw;
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
    public virtual async Task<int> CreateOrReplace(
        Guid blobId,
        Stream stream,
        CancellationToken cancellationToken = default
    )
    {
        await using (stream)
        {
            if (blobId == default)
                throw new ArgumentException(
                    $"{nameof(blobId)} must be not default",
                    nameof(blobId)
                );

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await using var blobStream = await CreateOrReplace(blobId, cancellationToken);
                return await stream.CopyToAndCountAsync(blobStream, CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (CodedException<IBlobStorage.ErrorCodes>)
            {
                throw;
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
    }

    /// <inheritdoc/>
    public virtual async Task<int> Create(
        Guid blobId,
        Stream stream,
        CancellationToken cancellationToken = default
    )
    {
        await using (stream)
        {
            if (blobId == default)
                throw new ArgumentException(
                    $"{nameof(blobId)} must be not default",
                    nameof(blobId)
                );

            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await using var blobStream = await Create(blobId, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    await Remove(blobId, CancellationToken.None);
                    throw new OperationCanceledException();
                }

                return await stream.CopyToAndCountAsync(blobStream, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await RemoveIfExist(blobId, CancellationToken.None);
                throw;
            }
            catch (CodedException<IBlobStorage.ErrorCodes>)
            {
                throw;
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
    }

    /// <inheritdoc/>
    public abstract Task<bool> Exist(Guid blobId, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task Remove(Guid blobId, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<bool> RemoveIfExist(
        Guid blobId,
        CancellationToken cancellationToken = default
    )
    {
        if (blobId == default)
            throw new ArgumentException($"{nameof(blobId)} must be not default", nameof(blobId));

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (await Exist(blobId, cancellationToken))
            {
                await Remove(blobId, cancellationToken);
                return true;
            }
            else
                return false;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CodedException<IBlobStorage.ErrorCodes>)
        {
            throw;
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
    public virtual async Task Move(
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
            var readStream = await OpenRead(oldBlobId, cancellationToken);
            await Create(newBlobId, readStream, cancellationToken);
            try
            {
                await Remove(oldBlobId, cancellationToken);
            }
            catch (Exception)
            {
                await Remove(newBlobId, CancellationToken.None);
                throw;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (CodedException<IBlobStorage.ErrorCodes>)
        {
            throw;
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
}

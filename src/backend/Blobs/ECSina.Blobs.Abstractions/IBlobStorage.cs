namespace ECSina.Blobs.Abstractions;

/// <summary>
/// The interface of a service, that is responsible for storage of BLOBs
/// <br/>
/// <br/>
/// When an error is occurred, a <see cref="CodedException{T}"/> with <see cref="ErrorCodes"/> as T is raised
/// </summary>
public interface IBlobStorage
{
    public enum ErrorCodes
    {
        /// <summary>
        /// A blob was not found, but it is needed to perform operation.
        /// </summary>
        BlobNotFound,
        /// <summary>
        /// A blob already exists and it's causes a conflict.
        /// </summary>
        BlobAlreadyExists,
        /// <summary>
        /// An unexpected error was occurred, when an operation aws performing.
        /// Saved blobs can be corrupted.
        /// </summary>
        Internal
    }

    /// <summary>
    /// Opens the blob with <paramref name="blobId"/> to read.
    /// </summary>
    /// <param name="blobId">The id of blob to open</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The read-only stream of the blob body.
    /// Must be disposed after using.
    /// </returns>
    Task<Stream> OpenRead(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Opens the blob with <paramref name="blobId"/> to read and write.
    /// </summary>
    /// <param name="blobId">The id of blob to open</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The stream of the blob body.
    /// Must be disposed after using.
    /// </returns>
    Task<Stream> OpenReadWrite(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create an empty blob with <paramref name="blobId"/>
    /// </summary>
    /// <param name="blobId">The id of blob to create</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The stream to write the blob body.
    /// Must be disposed after using.
    /// </returns>
    Task<Stream> Create(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a blob with <paramref name="blobId"/>
    /// and write a <b>rest</b> of <paramref name="stream"/> as the blob's body.
    /// </summary>
    /// <param name="blobId">The id of blob to create</param>
    /// <param name="stream">The stream with the blob's body to save. Stream will be disposed at the end</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The length in bytes of the created blob.
    /// </returns>
    Task<int> Create(Guid blobId, Stream stream, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a new empty blob with <paramref name="blobId"/> or replace an existed blob 
    /// </summary>
    /// <param name="blobId">The id of blob to create/replace</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The stream to write the blob body.
    /// Must be disposed after using.
    /// </returns>
    Task<Stream> CreateOrReplace(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create a blob with <paramref name="blobId"/> or replace an existed blob 
    /// and write a <b>rest</b> of <paramref name="stream"/> as the blob's body.
    /// </summary>
    /// <param name="blobId">The id of blob to create/replace</param>
    /// <param name="stream">The stream with the blob's body to save. Stream will be disposed at the end</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The length in bytes of the new blob.
    /// </returns>
    Task<int> CreateOrReplace(Guid blobId, Stream stream, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check that a blob with <paramref name="blobId"/> exists
    /// </summary>
    /// <param name="blobId">The id of blob to check</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    Task<bool> Exist(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove a blob with <paramref name="blobId"/>
    /// </summary>
    /// <param name="blobId">The id of blob to remove</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    Task Remove(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove a blob with <paramref name="blobId"/> if it exists
    /// </summary>
    /// <param name="blobId">The id of blob to remove</param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    /// <returns>
    /// The blob existed and was removed
    /// </returns>
    Task<bool> RemoveIfExist(Guid blobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Change a blob's id from <paramref name="oldBlobId"/> to <paramref name="newBlobId"/>
    /// </summary>
    /// <param name="oldBlobId"></param>
    /// <param name="newBlobId"></param>
    /// <param name="cancellationToken">
    /// If the cancellation is requested, the operation will be stopped without effects on saved blobs
    /// and a <see cref="OperationCanceledException"/> will be threw.
    /// </param>
    Task Move(Guid oldBlobId, Guid newBlobId, CancellationToken cancellationToken = default);
}
using ECSina.Blobs.Abstractions;
using ECSina.Common.Core.Exceptions;
using ECSina.Common.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ECSina.Blobs.Fs.Tests;

public sealed class TestsFixture : IDisposable
{
    internal readonly FsBlobStorage Storage;
    private readonly string _rootDirPath = Path.Combine(
        Path.GetTempPath(),
        Path.GetRandomFileName()
    );
    internal Guid? CreatedBlobId;

    public TestsFixture()
    {
        var options = new Mock<IOptions<FsBlobStorage.Options>>();
        options
            .Setup(x => x.Value)
            .Returns(new FsBlobStorage.Options { RootDirPath = _rootDirPath });

        var logger = new Mock<ILogger<FsBlobStorage>>().Object;

        Storage = new FsBlobStorage(options.Object, logger);
    }

    public void Dispose()
    {
        Directory.Delete(_rootDirPath, true);
    }
}

public sealed class FsBlobStorageTests : IClassFixture<TestsFixture>
{
    private readonly TestsFixture _fixture;

    public FsBlobStorageTests(TestsFixture fixture)
    {
        _fixture = fixture;
    }

    #region Create

    [Fact]
    public async Task Create_WithStreamPart()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var blobId = Guid.NewGuid();

        var sourceStream = LoremIpsum.GetReadOnlyStream();
        var wholeStreamLength = sourceStream.Length;
        sourceStream.Position = 20;
        var streamLengthToSave = wholeStreamLength - sourceStream.Position;

        // Act
        var savedByteCount = await blobStorage.Create(blobId, sourceStream);

        // Assert
        Assert.Equal(streamLengthToSave, savedByteCount);
        Assert.True(await blobStorage.Exist(blobId));
    }

    #endregion

    #region CreateOrReplace

    [Fact]
    public async Task CreateOrReplace_NotExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var blobId = Guid.NewGuid();
        var sourceStream = LoremIpsum.GetReadOnlyStream();
        var sourceStreamLength = sourceStream.Length;

        // Act
        var savedByteCount = await blobStorage.CreateOrReplace(blobId, sourceStream);

        // Assert
        Assert.Equal(sourceStreamLength, savedByteCount);
        Assert.True(await blobStorage.Exist(blobId));
    }

    [Fact]
    public async Task CreateOrReplace_ExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var blobId = Guid.NewGuid();

        var sourceStream = LoremIpsum.GetReadOnlyStream();
        await blobStorage.Create(blobId, sourceStream);

        sourceStream = LoremIpsum.GetReadOnlyStream();
        var wholeStreamLength = sourceStream.Length;
        sourceStream.Position = 20;
        var streamLengthToSave = wholeStreamLength - sourceStream.Position;

        // Act
        var savedByteCount = await blobStorage.CreateOrReplace(blobId, sourceStream);

        // Assert
        Assert.Equal(streamLengthToSave, savedByteCount);
        Assert.True(await blobStorage.Exist(blobId));

        await using var readStream = await blobStorage.OpenRead(blobId);
        Assert.Equal(streamLengthToSave, readStream.Length);
    }

    #endregion

    #region OpenRead

    [Fact]
    public async Task OpenRead_NotExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var blobId = Guid.NewGuid();

        // Assert
        await Assert.ThrowsAsync<CodedException<IBlobStorage.ErrorCodes>>(
            () => blobStorage.OpenRead(blobId)
        );
    }

    [Fact]
    public async Task OpenRead_ExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var sourceStream = LoremIpsum.GetReadOnlyStream();
        var blobId = Guid.NewGuid();

        // Act
        await blobStorage.Create(blobId, sourceStream);
        await using var readStream = await blobStorage.OpenRead(blobId);

        // Assert
        Assert.True(readStream.CanRead);
        Assert.False(readStream.CanWrite);

        var streamReader = new StreamReader(readStream);
        var readBlob = await streamReader.ReadToEndAsync();
        streamReader.Dispose();

        Assert.Equal(LoremIpsum, readBlob);
    }

    #endregion

    #region OpenWrite

    [Fact]
    public async Task OpenWrite_BeforeCreate()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var blobId = Guid.NewGuid();

        // Assert
        await Assert.ThrowsAsync<CodedException<IBlobStorage.ErrorCodes>>(
            () => blobStorage.OpenReadWrite(blobId)
        );
    }

    [Fact]
    public async Task OpenWrite_ExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var sourceStream = LoremIpsum.GetReadOnlyStream();
        var blobId = Guid.NewGuid();

        // Act
        await blobStorage.Create(blobId, sourceStream);
        await using var readStream = await blobStorage.OpenReadWrite(blobId);

        // Assert
        Assert.True(readStream.CanRead);
        Assert.True(readStream.CanWrite);

        var streamReader = new StreamReader(readStream);
        var readBlob = await streamReader.ReadToEndAsync();
        streamReader.Dispose();

        Assert.Equal(LoremIpsum, readBlob);
    }

    #endregion

    #region Remove

    [Fact]
    public async Task Remove_NotExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;
        var blobId = Guid.NewGuid();

        // Assert
        await Assert.ThrowsAsync<CodedException<IBlobStorage.ErrorCodes>>(
            () => blobStorage.Remove(blobId)
        );
    }

    [Fact]
    public async Task Remove_ExistedBlob()
    {
        // Arrange
        var blobStorage = _fixture.Storage;

        var sourceStream = LoremIpsum.GetReadOnlyStream();
        var blobId = Guid.NewGuid();
        await blobStorage.Create(blobId, sourceStream);

        // Act
        await blobStorage.Remove(blobId);

        // Assert
        Assert.False(await blobStorage.Exist(blobId));
    }

    #endregion

    #region Move

    [Fact]
    public async Task Move_SourceBlobExistsAndTargetDoNot()
    {
        // Arrange
        var blobStorage = _fixture.Storage;

        var sourceBlobId = Guid.NewGuid();
        var sourceStream = LoremIpsum.GetReadOnlyStream();
        await blobStorage.Create(sourceBlobId, sourceStream);

        var targetBlobId = Guid.NewGuid();

        // Act
        await blobStorage.Move(sourceBlobId, targetBlobId);

        // Assert
        Assert.True(await blobStorage.Exist(targetBlobId));
        Assert.False(await blobStorage.Exist(sourceBlobId));
    }

    [Fact]
    public async Task Move_SourceBlobDoNotExists()
    {
        // Arrange
        var blobStorage = _fixture.Storage;

        var sourceBlobId = Guid.NewGuid();
        var targetBlobId = Guid.NewGuid();

        // Assert
        await Assert.ThrowsAsync<CodedException<IBlobStorage.ErrorCodes>>(
            () => blobStorage.Move(sourceBlobId, targetBlobId)
        );
    }

    [Fact]
    public async Task Move_SourceAndTargetBlobsExist()
    {
        // Arrange
        var blobStorage = _fixture.Storage;

        var sourceBlobId = Guid.NewGuid();
        var blobStream = LoremIpsum.GetReadOnlyStream();
        await blobStorage.Create(sourceBlobId, blobStream);

        var targetBlobId = Guid.NewGuid();
        blobStream = LoremIpsum.GetReadOnlyStream();
        await blobStorage.Create(targetBlobId, blobStream);

        // Assert
        await Assert.ThrowsAsync<CodedException<IBlobStorage.ErrorCodes>>(
            () => blobStorage.Move(sourceBlobId, targetBlobId)
        );
    }

    #endregion

    private const string LoremIpsum =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce mattis turpis ipsum, non egestas lorem tincidunt eu. Maecenas ac arcu euismod, convallis est eget, ultrices orci. Mauris pellentesque neque a purus pretium, sit amet consectetur risus malesuada. Vestibulum sed lectus sagittis, semper ex at, laoreet mi. Pellentesque mauris ligula, mollis vitae velit nec, finibus consectetur lacus. Vestibulum mollis, ipsum at tincidunt vestibulum, dolor metus elementum leo, non porta ipsum velit id lectus. In hac habitasse platea dictumst. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis consectetur eros faucibus massa finibus, eget imperdiet justo tristique. Ut pretium leo quis elit tincidunt lobortis. Aliquam non erat ullamcorper, tincidunt libero vel, cursus metus. Nam lacus ante, semper nec luctus quis, cursus ac leo.";
}

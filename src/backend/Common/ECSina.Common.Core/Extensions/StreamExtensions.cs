using System.Buffers;

namespace ECSina.Common.Core.Extensions;

public static class StreamExtensions
{
    /// <summary>
    /// It's like <see cref="Stream.CopyToAsync(Stream, CancellationToken)"/> but also counts the copied bytes.
    /// </summary>
    public static async Task<int> CopyToAndCountAsync(
        this Stream source,
        Stream destination,
        CancellationToken cancellationToken = default
    )
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024 * 8);
        try
        {
            var totalBytesRead = 0;
            int bytesRead;
            while (
                (
                    bytesRead = await source
                        .ReadAsync(new Memory<byte>(buffer), cancellationToken)
                        .ConfigureAwait(false)
                ) != 0
            )
            {
                await destination
                    .WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken)
                    .ConfigureAwait(false);
                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Builds a read-only stream, that contains the <see cref="str"/>
    /// </summary>
    public static Stream GetReadOnlyStream(this string str)
    {
        // TODO: Try find a final buffer length for MemoryStream before new instance creation
        // to avoid memory reallocation.

        // TODO: Find a way without copying the string.
        // Something with Span and Memory.

        // I took the code from this answer:
        // https://stackoverflow.com/a/1879470
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(str);
        writer.Flush();
        stream.Position = 0;

        // Creating new instance is needed to make stream read-only.
        // Old stream isn't disposed, because it is not necessary:
        // https://docs.microsoft.com/en-us/dotnet/api/system.io.memorystream?view=net-6.0#remarks
        return new MemoryStream(stream.GetBuffer(), false);
    }

    // This method is based on this implementation: https://github.com/neosmart/StreamCompare/blob/master/StreamCompare/StreamCompare.cs
    public static async Task<bool> IsEqual(
        this Stream first,
        Stream second,
        CancellationToken cancel = default
    )
    {
        if (first == second)
        {
            // This is not merely an optimization, as incrementing one stream's position
            // should not affect the position of the other.
            return true;
        }

        if (first.CanSeek && second.CanSeek && first.Length != second.Length)
            return false;

        // (MAYBE) TODO: switch to some sort of ring buffer to simplify the logic when reads
        // between the two sources don't line up. Another alternative is switching to memory-
        // mapped file access, but we're purposely trying to minimize the time spent in the
        // kernel to keep this from bogging down the system.
        var buffer1 = ArrayPool<byte>.Shared.Rent(1024 * 4);
        using var defer1 = new Defer(() => ArrayPool<byte>.Shared.Return(buffer1));

        var buffer2 = ArrayPool<byte>.Shared.Rent(1024 * 4);
        using var defer2 = new Defer(() => ArrayPool<byte>.Shared.Return(buffer2));

        while (true)
        {
            var task1 = first.ReadAsync(buffer1, 0, buffer1.Length, cancel);
            var task2 = second.ReadAsync(buffer2, 0, buffer2.Length, cancel);
            var bytesRead = await Task.WhenAll(task1, task2).ConfigureAwait(false);
            var bytesRead1 = bytesRead[0];
            var bytesRead2 = bytesRead[1];

            if (bytesRead1 == 0 && bytesRead2 == 0)
                break;

            // Compare however much we were able to read from *both* arrays
            var sharedCount = Math.Min(bytesRead1, bytesRead2);
            if (!buffer1.AsSpan(0, sharedCount).SequenceEqual(buffer2.AsSpan(0, sharedCount)))
                return false;

            if (bytesRead1 != bytesRead2)
            {
                // Instead of duplicating the code for reading fewer bytes from file1 than file2
                // for fewer bytes from file2 than file1, abstract that detail away.
                var (lessRead, moreRead, moreCount, lessStream) =
                    bytesRead1 < bytesRead2
                        ? (buffer1, buffer2, bytesRead2 - sharedCount, first)
                        : (buffer2, buffer1, bytesRead1 - sharedCount, second);

                while (moreCount > 0)
                {
                    // Try reading more from `lessRead`
                    var lessCount = await lessStream
                        .ReadAsync(lessRead, 0, moreCount, cancel)
                        .ConfigureAwait(false);

                    if (lessCount == 0)
                    {
                        // One stream was exhausted before the other
                        return false;
                    }

                    if (
                        !lessRead
                            .AsSpan(0, lessCount)
                            .SequenceEqual(moreRead.AsSpan(sharedCount, lessCount))
                    )
                    {
                        return false;
                    }

                    moreCount -= lessCount;
                    sharedCount += lessCount;
                }
            }
        }

        return true;
    }
}

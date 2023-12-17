namespace ECSina.Common.AspNetCore;

public static class ContentTypeProviderExtensions
{
    /// <summary>
    /// Get MIME type of the file with name <see cref="fileName"/>.
    /// </summary>
    /// <returns>
    /// A found type or "application/octet-stream".
    /// </returns>
    public static string GetContentType(this IContentTypeProvider provider, string fileName)
    {
        if (!provider.TryGetContentType(fileName, out var contentType))
            contentType = "application/octet-stream";

        return contentType;
    }
}

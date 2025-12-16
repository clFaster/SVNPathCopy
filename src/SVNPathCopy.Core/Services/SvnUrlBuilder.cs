using SVNPathCopy.Core.Models;

namespace SVNPathCopy.Core.Services;

/// <summary>
///     Utility class for building SVN URLs.
/// </summary>
public static class SvnUrlBuilder
{
    /// <summary>
    ///     Builds an SVN URL from the given info with optional revision.
    /// </summary>
    /// <param name="info">The SVN item info.</param>
    /// <param name="includeRevision">Whether to include revision in the URL.</param>
    /// <param name="encodingStyle">The URL encoding style to apply.</param>
    /// <returns>The formatted SVN URL.</returns>
    public static string BuildUrl(SvnItemInfo info, bool includeRevision, UrlEncodingStyle encodingStyle)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        if (string.IsNullOrEmpty(info.Uri))
        {
            throw new ArgumentException("SVN URI cannot be null or empty.", nameof(info));
        }

        // Uri is verified to be non-null above
        string url = ApplyEncoding(info.Uri!, encodingStyle);

        if (includeRevision && info.LastChangeRevision > 0)
        {
            url += $"?p={info.LastChangeRevision}";
        }

        return url;
    }

    private static string ApplyEncoding(string url, UrlEncodingStyle style)
    {
        return style switch
        {
            UrlEncodingStyle.None => url,
            UrlEncodingStyle.Path => EncodePathSegments(url),
            UrlEncodingStyle.Full => Uri.EscapeDataString(url),
            _ => url
        };
    }

    private static string EncodePathSegments(string url)
    {
        // Parse the URL to encode only the path portion
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return url;
        }

        // Uri class already encodes properly during construction
        // Return the AbsoluteUri which has proper encoding
        return uri.AbsoluteUri;
    }
}

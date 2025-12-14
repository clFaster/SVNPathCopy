namespace SVNPathCopy.Core.Models;

/// <summary>
///     Configuration settings for the SVN Path Copy extension.
/// </summary>
public sealed class SvnPathCopySettings
{
    /// <summary>
    ///     Gets or sets whether the context menu extension is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets whether to show the "Copy SVN URL with Revision" menu item.
    /// </summary>
    public bool ShowCopyWithRevision { get; set; } = true;

    /// <summary>
    ///     Gets or sets whether to show the "Copy SVN URL" menu item (without revision).
    /// </summary>
    public bool ShowCopyWithoutRevision { get; set; } = true;

    /// <summary>
    ///     Gets or sets the URL encoding style to use.
    /// </summary>
    public UrlEncodingStyle UrlEncodingStyle { get; set; } = UrlEncodingStyle.Path;
}

/// <summary>
///     Specifies the URL encoding style for SVN URLs.
/// </summary>
public enum UrlEncodingStyle
{
    /// <summary>
    ///     No encoding applied.
    /// </summary>
    None,

    /// <summary>
    ///     Path encoding (spaces become %20, etc.).
    /// </summary>
    Path,

    /// <summary>
    ///     Full URL encoding.
    /// </summary>
    Full
}

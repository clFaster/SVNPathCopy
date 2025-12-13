namespace SVNPathCopy.Core.Models;

/// <summary>
///     Contains information about an SVN item (file or folder).
/// </summary>
public sealed class SvnItemInfo
{
    /// <summary>
    ///     Gets or sets the SVN repository URL for this item.
    /// </summary>
    public string? Uri { get; set; }

    /// <summary>
    ///     Gets or sets the last change revision number.
    /// </summary>
    public long LastChangeRevision { get; set; }

    /// <summary>
    ///     Gets or sets the local file path.
    /// </summary>
    public string? LocalPath { get; set; }

    /// <summary>
    ///     Gets or sets the repository root URL.
    /// </summary>
    public string? RepositoryRoot { get; set; }
}

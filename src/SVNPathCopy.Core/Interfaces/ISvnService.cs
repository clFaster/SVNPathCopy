using SVNPathCopy.Core.Models;

namespace SVNPathCopy.Core.Interfaces;

/// <summary>
///     Interface for SVN operations.
/// </summary>
public interface ISvnService
{
    /// <summary>
    ///     Checks if the specified path is in an SVN working copy.
    /// </summary>
    /// <param name="path">The file or folder path to check.</param>
    /// <returns>True if the path is in an SVN working copy; otherwise, false.</returns>
    bool IsInSvnWorkingCopy(string path);

    /// <summary>
    ///     Gets the status of an SVN item.
    /// </summary>
    /// <param name="path">The file or folder path.</param>
    /// <returns>The SVN status of the item.</returns>
    SvnItemStatus GetStatus(string path);

    /// <summary>
    ///     Gets SVN information for the specified path.
    /// </summary>
    /// <param name="path">The file or folder path.</param>
    /// <returns>SVN information for the item.</returns>
    SvnItemInfo GetInfo(string path);

    /// <summary>
    ///     Validates whether copying the SVN path is possible.
    /// </summary>
    /// <param name="path">The file or folder path.</param>
    /// <param name="withRevision">Whether revision info is required.</param>
    /// <returns>A validation result with success status and error message if applicable.</returns>
    (bool IsValid, string? ErrorMessage) ValidateCopyOperation(string path, bool withRevision);
}

namespace SVNPathCopy.Core.Models;

/// <summary>
///     Represents the version control status of an SVN item.
/// </summary>
public sealed class SvnItemStatus
{
    /// <summary>
    ///     Gets or sets the local path of the item.
    /// </summary>
    public string? LocalPath { get; set; }

    /// <summary>
    ///     Gets or sets whether the item is under version control.
    /// </summary>
    public bool IsVersioned { get; set; }

    /// <summary>
    ///     Gets or sets whether the item has local modifications.
    /// </summary>
    public bool HasLocalModifications { get; set; }

    /// <summary>
    ///     Gets or sets whether the item is scheduled for addition.
    /// </summary>
    public bool IsScheduledForAddition { get; set; }

    /// <summary>
    ///     Gets or sets whether the item exists in the repository.
    /// </summary>
    public bool ExistsInRepository { get; set; }
}

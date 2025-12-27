using System.Collections.ObjectModel;
using SharpSvn;
using SVNPathCopy.Core.Interfaces;
using SVNPathCopy.Core.Models;

namespace SVNPathCopy.ShellExtension.Services;

/// <summary>
///     SVN service implementation using SharpSvn.
/// </summary>
public sealed class SharpSvnService : ISvnService, IDisposable
{
    private bool _disposed;

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        // SharpSvn clients are disposed per-operation, nothing to clean up here
    }

    /// <inheritdoc />
    public bool IsInSvnWorkingCopy(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        try
        {
            using var client = new SvnClient();
            var args = new SvnStatusArgs { Depth = SvnDepth.Empty };
            client.GetStatus(path, args, out _);
            return true;
        }
        catch (SvnException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public SvnItemStatus GetStatus(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        var result = new SvnItemStatus { LocalPath = path };

        try
        {
            using var client = new SvnClient();
            var args = new SvnStatusArgs { Depth = SvnDepth.Infinity };
            client.GetStatus(path, args, out Collection<SvnStatusEventArgs> statuses);

            if (statuses.Count == 0)
            {
                result.IsVersioned = true;
                result.ExistsInRepository = true;
                return result;
            }

            var status = statuses[0];
            result.IsVersioned = status.LocalContentStatus != SvnStatus.NotVersioned;
            result.HasLocalModifications = status.LocalContentStatus == SvnStatus.Modified;
            result.IsScheduledForAddition = status.LocalContentStatus == SvnStatus.Added;
            result.ExistsInRepository =
                status.IsRemoteUpdated || status.LocalContentStatus != SvnStatus.Added;
        }
        catch (SvnException)
        {
            result.IsVersioned = false;
        }

        return result;
    }

    /// <inheritdoc />
    public SvnItemInfo GetInfo(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        using var client = new SvnClient();
        client.GetInfo(path, out var info);

        return new SvnItemInfo
        {
            Uri = info.Uri?.ToString(),
            LastChangeRevision = info.LastChangeRevision,
            LocalPath = path,
            RepositoryRoot = info.RepositoryRoot?.ToString(),
        };
    }

    /// <inheritdoc />
    public (bool IsValid, string? ErrorMessage) ValidateCopyOperation(
        string path,
        bool withRevision
    )
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        try
        {
            var status = GetStatus(path);

            if (!status.IsVersioned)
            {
                return (
                    false,
                    "Item is not under version control. Please add and commit your changes."
                );
            }

            if (withRevision && status.IsScheduledForAddition)
            {
                return (false, "Item is scheduled for addition. Please commit your changes first.");
            }

            return (true, null);
        }
        catch (SvnException ex)
        {
            return (false, $"SVN error: {ex.Message}");
        }
    }
}

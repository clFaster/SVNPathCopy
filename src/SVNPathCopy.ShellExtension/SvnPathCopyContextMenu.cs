using System.Reflection;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using SVNPathCopy.Core.Interfaces;
using SVNPathCopy.Core.Models;
using SVNPathCopy.Core.Services;
using SVNPathCopy.ShellExtension.Services;

namespace SVNPathCopy.ShellExtension;

/// <summary>
///     Windows Explorer context menu extension for copying SVN URLs.
/// </summary>
[ComVisible(true)]
[Guid("ED4DD0F3-E4E3-4F8A-AD97-7B76FC3E0965")]
[COMServerAssociation(AssociationType.AllFilesAndFolders)]
public sealed class SvnPathCopyContextMenu : SharpContextMenu
{
    private readonly IConfigurationService _configService;
    private readonly Lazy<Image?> _menuIcon;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SvnPathCopyContextMenu" /> class.
    /// </summary>
    public SvnPathCopyContextMenu()
    {
        _configService = new RegistryConfigurationService();
        _menuIcon = new Lazy<Image?>(LoadMenuIcon);
    }

    /// <inheritdoc />
    protected override bool CanShowMenu()
    {
        // Check if extension is enabled
        SvnPathCopySettings settings = _configService.GetSettings();
        if (!settings.Enabled)
        {
            return false;
        }

        // Check if exactly one item is selected
        if (SelectedItemPaths.Count() != 1)
        {
            return false;
        }

        // Check if the item is in an SVN working copy
        using var svnService = new SharpSvnService();
        return svnService.IsInSvnWorkingCopy(SelectedItemPaths.First());
    }

    /// <inheritdoc />
    protected override ContextMenuStrip CreateMenu()
    {
        var menu = new ContextMenuStrip();
        SvnPathCopySettings settings = _configService.GetSettings();
        Image? icon = _menuIcon.Value;

        // Add separator at the top
        menu.Items.Add(new ToolStripSeparator());

        // Create "Copy SVN URL with Revision" menu item
        if (settings.ShowCopyWithRevision)
        {
            var itemWithRevision = new ToolStripMenuItem { Text = "Copy SVN URL with REV", Image = icon };
            itemWithRevision.Click += (_, _) => CopySvnPath(true);
            menu.Items.Add(itemWithRevision);
        }

        // Create "Copy SVN URL" menu item (without revision)
        if (settings.ShowCopyWithoutRevision)
        {
            var itemWithoutRevision = new ToolStripMenuItem { Text = "Copy SVN URL", Image = icon };
            itemWithoutRevision.Click += (_, _) => CopySvnPath(false);
            menu.Items.Add(itemWithoutRevision);
        }

        return menu;
    }

    private void CopySvnPath(bool withRevision)
    {
        string? path = SelectedItemPaths.FirstOrDefault();
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        try
        {
            using var svnService = new SharpSvnService();
            SvnPathCopySettings settings = _configService.GetSettings();

            // Validate the operation
            (bool isValid, string? errorMessage) = svnService.ValidateCopyOperation(path, withRevision);
            if (!isValid)
            {
                ShowError(errorMessage ?? "Cannot copy SVN URL.");
                Clipboard.Clear();
                return;
            }

            // Get SVN info and build URL
            SvnItemInfo info = svnService.GetInfo(path);
            string url = SvnUrlBuilder.BuildUrl(info, withRevision, settings.UrlEncodingStyle);

            // Copy to clipboard
            Clipboard.SetText(url);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to copy SVN URL: {ex.Message}");
            Clipboard.Clear();
        }
    }

    private static void ShowError(string message)
    {
        MessageBox.Show(
            message,
            "SVN Path Copy",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private static Image? LoadMenuIcon()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "SVNPathCopy.ShellExtension.Resources.share_svn.png";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is not null)
            {
                return Image.FromStream(stream);
            }
        }
        catch
        {
            // Ignore icon loading errors
        }

        return null;
    }
}

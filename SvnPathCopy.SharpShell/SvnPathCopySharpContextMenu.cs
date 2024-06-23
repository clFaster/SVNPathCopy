using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using SharpSvn;
using System.Runtime.InteropServices;
using System.Web;

namespace SvnPathCopy.SharpShell;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[COMServerAssociation(AssociationType.AllFilesAndFolders)]
[Guid("12345678-90AB-CDEF-1234-567890ABCDEF")]
public class SvnPathCopySharpContextMenu : SharpContextMenu
{
    // Check if the Context Menu entry should be shown
    protected override bool CanShowMenu()
    {
        // Check if only one file or folder is selected and file or folder exists in SVN
        return SelectedItemPaths.Count() == 1 && IsFileOrFolderInSvnRepo();
    }

    // Create the Context Menu entry
    protected override ContextMenuStrip CreateMenu()
    {
        var attr = File.GetAttributes(SelectedItemPaths.First());

        var menu = new ContextMenuStrip();
        menu.Items.Add(new ToolStripSeparator());

        // Create Menu Entry with Revision - create only for files
        var itemCopySvnPathWithRevision = new ToolStripMenuItem
        {
            Text = "Copy SVN URL with REV",
            Image = Properties.Resources.share_svn
        };

        // Set action
        itemCopySvnPathWithRevision.Click += (sender, args) => CopySvnPath(true);

        // Add items to menu
        menu.Items.Add(itemCopySvnPathWithRevision);


        // Create Menu Entry without Revision
        var itemCopySvnPathWithoutRevision = new ToolStripMenuItem
        {
            Text = "Copy SVN URL",
            Image = Properties.Resources.share_svn
        };

        // Set action
        itemCopySvnPathWithoutRevision.Click += (sender, args) => CopySvnPath(false);

        // Add items to menu
        menu.Items.Add(itemCopySvnPathWithoutRevision);

        return menu;
    }
    
    private bool IsFileOrFolderInSvnRepo()
    {
        var selectedFilePath = SelectedItemPaths.First();
        try
        {
            using var svnClient = new SvnClient();
            var svnStatusArgs = new SvnStatusArgs
            {
                Depth = SvnDepth.Empty
            };
            svnClient.GetStatus(selectedFilePath, svnStatusArgs, out var states);
        }
        catch
        {
            return false;
        }
        return true;
    }

    private bool IsCopySvnPathPossible(bool withRevision)
    {
        using (var svnClient = new SvnClient())
        {
            var svnStatusArgs = new SvnStatusArgs
            {
                Depth = SvnDepth.Infinity
            };
            svnClient.GetStatus(SelectedItemPaths.First(), svnStatusArgs, out var states);
            if (states.Count == 0)
            {
                return true;
            }
            if (SvnStatus.NotVersioned == states[0].LocalContentStatus)
            {
                MessageBox.Show("Item is not under version control - please add and commit your changes", "Error");
                return false;
            }
            if (states.Count != 0 && !states.First().IsRemoteUpdated)
            {
                if (withRevision == false)
                {
                    // Copy SVN Path without revision even if not committed changes!
                    return true;
                }
                MessageBox.Show("Item is scheduled for addition - please commit your changes", "Error");
                return false;
            }
        }
        MessageBox.Show("Unknown error occurred", "Error");
        return false;
    }

    private SvnInfoEventArgs GetSvnInfo()
    {
        var workingCopyClient = new SvnClient();
        workingCopyClient.GetInfo(SelectedItemPaths.First(), out var info);
        return info;
    }

    private string GetSvnUri(bool withRevision)
    {
        var svnInfo = GetSvnInfo();
        var svnUri = HttpUtility.UrlPathEncode(svnInfo.Uri.ToString());
        if (withRevision)
        {
            svnUri += "?p=" + svnInfo.LastChangeRevision.ToString();
        }
        return svnUri;
    }

    private void CopySvnPath(bool withRevision)
    {
        if (IsCopySvnPathPossible(withRevision))
        {
            Clipboard.SetText(GetSvnUri(withRevision));
        }
        else
        {
            Clipboard.Clear();
        }
    }

    [ComRegisterFunction]
    public static void Register(Type t)
    {
        RegisterContextMenu(t, @"*\shellex\ContextMenuHandlers\");
        RegisterContextMenu(t, @"Directory\shellex\ContextMenuHandlers\");
        RegisterContextMenu(t, @"Directory\Background\shellex\ContextMenuHandlers\");
    }

    private static void RegisterContextMenu(Type t, string basePath)
    {
        string keyPath = basePath + "SimpleContextMenu";
        using (var key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(keyPath))
        {
            if (key == null)
            {
                Console.WriteLine($"Failed to create registry key: {keyPath}");
            }
            else
            {
                key.SetValue(null, t.GUID.ToString("B"));
            }
        }
    }

    [ComUnregisterFunction]
    public static void Unregister(Type t)
    {
        UnregisterContextMenu(@"*\shellex\ContextMenuHandlers\");
        UnregisterContextMenu(@"Directory\shellex\ContextMenuHandlers\");
        UnregisterContextMenu(@"Directory\Background\shellex\ContextMenuHandlers\");
    }

    private static void UnregisterContextMenu(string basePath)
    {
        string keyPath = basePath + "SimpleContextMenu";
        try
        {
            Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(keyPath, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unregistering COM server from {keyPath}: {ex.Message}");
        }
    }
}
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using SharpSvn;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SVNPathCopy
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class SVNPathCopy : SharpContextMenu
    {
        // Check if File or Folder exists in SVN and if the local revision is pushed to the server!
        // Show Popup if it isn't released
        private bool IsFileOrFolderInSVNRepo()
        {
            var selectedFilePath = SelectedItemPaths.First();
            try
            {
                using (SvnClient svnClient = new SvnClient())
                {
                    SvnStatusArgs svnStatusArgs = new SvnStatusArgs
                    {
                        Depth = SvnDepth.Empty
                    };

                    svnClient.GetStatus(selectedFilePath, svnStatusArgs, out Collection<SvnStatusEventArgs> states);
                }
            }
            catch
            {
                // File is not in a svn repository
                return false;
            }
            return true;
        }

        private bool ShowedCopySvnPath()
        {
            var selectedFilePath = SelectedItemPaths.First();

            using (SvnClient svnClient = new SvnClient())
            {
                SvnStatusArgs svnStatusArgs = new SvnStatusArgs
                {
                    Depth = SvnDepth.Empty
                };

                svnClient.GetStatus(selectedFilePath, svnStatusArgs, out Collection<SvnStatusEventArgs> states);

                if (states.Count == 0)
                {
                    return true;
                }
                if (SvnStatus.NotVersioned == states[0].LocalContentStatus)
                {
                    // File wasn't added to SVN repository
                    MessageBox.Show("Item is not under version control - please add and commit your changes", "Error");
                    return false;
                }
                if (states.Count != 0 && !states.First().IsRemoteUpdated)
                {
                    MessageBox.Show("Item is scheduled for addition - please commit your changes", "Error");
                    return false;
                }
            }
            MessageBox.Show("Unknown error occurred", "Error");
            return false;
        }

        private SvnInfoEventArgs GetSVNInfo()
        {
            var workingCopyClient = new SvnClient();
            workingCopyClient.GetInfo(SelectedItemPaths.First(), out SvnInfoEventArgs info);
            return info;
        }

        private String GetSVNURI(bool withRevision)
        {
            var svnInfo = GetSVNInfo();
            String svnUri = Uri.EscapeUriString(svnInfo.Uri.ToString());
            if (withRevision)
            {
                svnUri += "?p=" + svnInfo.LastChangeRevision.ToString();
            }
            return svnUri;
        }

        private void CopySVNPath(bool withRevision)
        {
            if (ShowedCopySvnPath())
            {
                Clipboard.SetText(GetSVNURI(withRevision));
            }
            else
            {
                Clipboard.Clear();
            }
        }

        // Check if the Context Menu entry should be shown
        protected override bool CanShowMenu()
        {
            // Check if only one file or folder is selected and file or folder exists in SVN
            if (SelectedItemPaths.Count() == 1 && IsFileOrFolderInSVNRepo())
            {
                return true;
            }
            return false;
        }

        // Create the Context Menu entry
        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            // Create Menu Entry with Revision
            var itemCopySVNPathWithRevision = new ToolStripMenuItem
            {
                Text = "Copy SVN URL with REV",
                Image = Properties.Resources.share_svn
            };

            // Create Menu Entry without Revision
            var itemCopySVNPathWithoutRevision = new ToolStripMenuItem
            {
                Text = "Copy SVN URL",
                Image = Properties.Resources.share_svn
            };

            // Set action when clicked
            itemCopySVNPathWithRevision.Click += (sender, args) => CopySVNPath(true);
            itemCopySVNPathWithoutRevision.Click += (sender, args) => CopySVNPath(false);

            // Add items to menu
            menu.Items.Add(itemCopySVNPathWithRevision);
            menu.Items.Add(itemCopySVNPathWithoutRevision);

            return menu;
        }
    }
}

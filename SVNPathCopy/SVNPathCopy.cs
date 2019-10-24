using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using SharpSvn;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Web;

namespace SVNPathCopy
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class SVNPathCopy : SharpContextMenu
    {
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
                return false;
            }
            return true;
        }

        private bool IsCopySVNPathPossible()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                SvnStatusArgs svnStatusArgs = new SvnStatusArgs
                {
                    Depth = SvnDepth.Infinity
                };
                svnClient.GetStatus(SelectedItemPaths.First(), svnStatusArgs, out Collection<SvnStatusEventArgs> states);
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
            String svnUri = HttpUtility.UrlPathEncode(svnInfo.Uri.ToString());
            if (withRevision)
            {
                svnUri += "?p=" + svnInfo.LastChangeRevision.ToString();
            }
            return svnUri;
        }

        private void CopySVNPath(bool withRevision)
        {
            if (IsCopySVNPathPossible())
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
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemCopySVNPathWithRevision);
            menu.Items.Add(itemCopySVNPathWithoutRevision);

            return menu;
        }
    }
}

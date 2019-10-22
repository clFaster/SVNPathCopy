using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using SharpSvn;
using System;
using System.Web;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SVNPathCopy
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class SVNPathCopy : SharpContextMenu
    {
        private bool IsFileOrFolderInSVNRepo()
        {
            try
            {
                var selectedFilePath = SelectedItemPaths.First();
                MessageBox.Show(selectedFilePath);
                var workingCopyClient = new SvnClient();

                SvnInfoEventArgs info;

                workingCopyClient.GetInfo(selectedFilePath, out info);

                MessageBox.Show(info.LastChangeRevision.ToString());
            }
            catch
            {
                return false;
            }
            return true;
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
            var svnUri = GetSVNURI(withRevision);
            Clipboard.SetText(svnUri);
        }

        protected override bool CanShowMenu()
        {
            // Todo: Should only be shown in SVN Repos!
            // Can only be shown when one item is selected
            if (SelectedItemPaths.Count() != 1)
            {
                return false;
            }

            return IsFileOrFolderInSVNRepo();
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            // Create Menu Entry with Revision
            var itemCopySVNPathWithRevision = new ToolStripMenuItem
            {
                Text = "Copy SVN Path with Revision",
                Image = Properties.Resources.share_svn
            };

            // Create Menu Entry without Revision
            var itemCopySVNPathWithoutRevision = new ToolStripMenuItem
            {
                Text = "Copy SVN Path",
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

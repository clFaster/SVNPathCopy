using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SVNPathCopy
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class SVNPathCopy : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            // Todo: Should only be shown in SVN Repos!
            // Can only be shown when one item is selected
            return SelectedItemPaths.Count() == 1;
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

        private void CopySVNPath(bool with_revision)
        {
            MessageBox.Show("SVN Path with revision should be copied to clipboard");
        }

    }
}

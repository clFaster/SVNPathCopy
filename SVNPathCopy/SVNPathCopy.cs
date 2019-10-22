using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SVNPathCopy
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".txt")]
    public class SVNPathCopy : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            // Todo: Should only be shown in SVN Repos!
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            // Create Menu Entry
            var itemCopySVNPathWithRevision = new ToolStripMenuItem
            {
                Text = "Copy SVN Path with Revision",
                Image = Properties.Resources.share_svn
            };

            // Set action when clicked
            itemCopySVNPathWithRevision.Click += (sender, args) => CopySVNPath(true);

            // Add items to menu
            menu.Items.Add(itemCopySVNPathWithRevision);

            return menu;
        }

        private void CopySVNPath(bool with_revision)
        {
            MessageBox.Show("SVN Path with revision should be copied to clipboard");
        }

    }
}

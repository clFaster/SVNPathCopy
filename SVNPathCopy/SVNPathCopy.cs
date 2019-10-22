using SharpShell.SharpContextMenu;
using System;
using System.Windows.Forms;

namespace SVNPathCopy
{
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

            var itemCopySVNPathWithRevision = new ToolStripMenuItem
            {
                Text = "Copy SVN Path with Revision",
                Image = Properties.Resources.share_svn
            };
            throw new NotImplementedException();
        }

    }
}

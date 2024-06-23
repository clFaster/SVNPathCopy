using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.Runtime.InteropServices;

namespace SharpShellNet8Demo
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.DirectoryBackground)]
    [Guid("B3B9C5B6-5C92-4D1B-85E6-2F80B72F6E28")]
    public class SimpleContextMenu : SharpContextMenu
    {
        protected override bool CanShowMenu() => true;

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var mainItem = new ToolStripMenuItem
            {
                Text = "Simple Extension Action"
            };

            var actionItem = new ToolStripMenuItem
            {
                Text = "Perform Action"
            };
            actionItem.Click += (sender, e) => MessageBox.Show("Action performed!");

            mainItem.DropDownItems.Add(actionItem);
            menu.Items.Add(mainItem);

            return menu;
        }

        [ComRegisterFunction]
        public static void Register(Type t)
        {
            RegisterContextMenu(t, @"*\shellex\ContextMenuHandlers\");
            RegisterContextMenu(t, @"Directory\shellex\ContextMenuHandlers\");
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
}
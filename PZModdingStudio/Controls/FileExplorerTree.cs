using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;
// Asegúrate de tener referencia a Microsoft.VisualBasic
using Microsoft.VisualBasic.FileIO;
using PZModdingStudio.Forms;
using PZModdingStudio.Helpers;

public class FileExplorerTree : UserControl
{
    private TreeView tree;
    private ImageList imgs;
    private ContextMenuStrip ctx;
    private const string DUMMY = "DUMMY";

    public static string TextFolder { get; set; } = "Folder";
    public static string TextFolder2 { get; set; } = "folder";
    public static string TextFile { get; set; } = "File";
    public static string TextFile2 { get; set; } = "file";
    public static string TextProperties { get; set; } = "Properties";
    public static string TextDelete { get; set; } = "Delete";
    public static string TextNew { get; set; } = "New";
    public static string TextOpen { get; set; } = "Open";
    public static string TextOpenFileExplorer { get; set; } = "Open in File Explorer";
    public static string TextCantDeleteRoot { get; set; } = "Cannot remove root.";
    public static string TextCantRenameRoot { get; set; } = "Cannot rename root.";
    public static string TextWarning { get; set; } = "Waring";
    public static string TextError { get; set; } = "Error";
    public static string TextCantDeleteError { get; set; } = "Could not be deleted:";
    public static string TextCantRenameError { get; set; } = "Could not rename:";
    public static string TextConfirmDelete { get; set; } = "Send {type} '{name}' to Trash?";
    public static string TextConfirmDeleteTitle { get; set; } = "Confirm deletion";
    public static string TextDialogNewFolderInput { get; set; } = "Name of the new folder:";
    public static string TextNewFolder { get; set; } = "New Folder";
    public static string TextCantCreateFolderError { get; set; } = "Could not create folder:";
    public static string TextCantCreateFileError { get; set; } = "Could not create file:";
    public static string TextDialogNewFileInput { get; set; } = "File name (include extension, e.g. new.txt):";
    public static string TextNewFile { get; set; } = "New file";
    public static string TextAccept { get; set; } = "Accept";
    public static string TextCancel { get; set; } = "Cancel";
    public static string TextInput { get; set; } = "Input";
    public static string TextRename { get; set; } = "Rename";
    public static string TextFolderAlreadyExists { get; set; } = "A file or folder named '{newName}' already exists.";
    public static string TextDialogRenameFolderInput { get; set; } = "New folder name:";
    public static string TextDialogRenameFilenput { get; set; } = "New file name:";


    public FileExplorerTree()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.tree = new TreeView();
        this.imgs = new ImageList();
        this.ctx = new ContextMenuStrip();

        imgs.ImageSize = new Size(16, 16);
        imgs.ColorDepth = ColorDepth.Depth32Bit;

        tree.ImageList = imgs;
        tree.Dock = DockStyle.Fill;
        tree.BorderStyle = BorderStyle.None;
        tree.ShowLines = false;
        tree.ShowRootLines = false;
        tree.ShowPlusMinus = false;
        tree.ItemHeight = 18;
        tree.Indent = 18;
        tree.HideSelection = false;

        tree.DrawMode = TreeViewDrawMode.OwnerDrawAll;
        tree.DrawNode += Tree_DrawNode;
        tree.BeforeExpand += Tree_BeforeExpand;
        tree.NodeMouseDoubleClick += Tree_NodeMouseDoubleClick;
        tree.NodeMouseClick += Tree_NodeMouseClick;

        this.Controls.Add(tree);

        AddSystemIconToList("folder", true);
        AddSystemIconToList(".txt", false);
    }

    #region Public Actions
    public void ReloadRoots()
    {
        tree.Nodes.Clear();
        LoadDrives();
    }
    #endregion

    #region Populate (lazy)
    private void LoadDrives()
    {
        tree.BeginUpdate();
        tree.Nodes.Clear();
        foreach (var d in DriveInfo.GetDrives())
        {
            TreeNode n = new TreeNode(d.Name) { Tag = d.RootDirectory.FullName, ImageKey = "folder", SelectedImageKey = "folder" };
            if (HasChildren(d.RootDirectory.FullName)) n.Nodes.Add(DUMMY);
            tree.Nodes.Add(n);
        }
        tree.EndUpdate();
    }

    private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
        if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == DUMMY)
        {
            e.Node.Nodes.Clear();
            AddDirectoryChildren(e.Node);
        }
    }

    private void AddDirectoryChildren(TreeNode node)
    {
        string path = node.Tag as string;
        if (path == null) return;
        try
        {
            var dirs = Directory.GetDirectories(path);
            Array.Sort(dirs, StringComparer.InvariantCultureIgnoreCase);
            foreach (var d in dirs)
            {
                var dn = new DirectoryInfo(d);
                TreeNode tn = new TreeNode(dn.Name) { Tag = d };
                tn.ImageKey = "folder";
                tn.SelectedImageKey = "folder";

                if (HasChildren(d)) tn.Nodes.Add(DUMMY);
                node.Nodes.Add(tn);
            }

            var files = Directory.GetFiles(path);
            Array.Sort(files, StringComparer.InvariantCultureIgnoreCase);
            foreach (var f in files)
            {
                var fi = new FileInfo(f);
                string ext = fi.Extension.ToLowerInvariant();
                string key = EnsureFileIcon(ext, f);
                TreeNode fn = new TreeNode(fi.Name) { Tag = f, ImageKey = key, SelectedImageKey = key };
                node.Nodes.Add(fn);
            }
        }
        catch { }
    }

    private new bool HasChildren(string path)
    {
        try
        {
            if (Directory.GetDirectories(path).Length > 0) return true;
            if (Directory.GetFiles(path).Length > 0) return true;
        }
        catch { }
        return false;
    }
    #endregion

    #region Node drawing
    public bool ShowCaret { get; set; } = true;
    public int CaretOffset { get; set; } = -12;
    public int CaretSize { get; set; } = 10;

    private void Tree_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
        Graphics g = e.Graphics;
        Rectangle bounds = e.Bounds;
        TreeNode node = e.Node;
        bool selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;

        if (selected)
            g.FillRectangle(SystemBrushes.Highlight, bounds);
        else
            g.FillRectangle(SystemBrushes.Window, bounds);

        int baseIndent = node.Level * tree.Indent;
        int caretX = bounds.Left + baseIndent + CaretOffset;
        if (caretX < 2) caretX = 2;
        int iconLeft = bounds.Left + baseIndent + 2;
        int iconTop = bounds.Top + (bounds.Height - imgs.ImageSize.Height) / 2;

        if (ShowCaret && node.Nodes.Count > 0)
        {
            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Brush caretBrush = selected ? SystemBrushes.HighlightText : SystemBrushes.ControlText;
            int size = CaretSize;
            int caretY = bounds.Top + (bounds.Height - size) / 2;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            if (node.IsExpanded)
            {
                Point p1 = new Point(caretX, caretY);
                Point p2 = new Point(caretX + size, caretY);
                Point p3 = new Point(caretX + size / 2, caretY + size);
                path.AddPolygon(new Point[] { p1, p2, p3 });
            }
            else
            {
                Point p1 = new Point(caretX, caretY);
                Point p2 = new Point(caretX, caretY + size);
                Point p3 = new Point(caretX + size, caretY + size / 2);
                path.AddPolygon(new Point[] { p1, p2, p3 });
            }

            g.FillPath(caretBrush, path);
            g.SmoothingMode = oldSmoothing;
        }

        Image icon = null;
        if (node.ImageKey != null && imgs.Images.ContainsKey(node.ImageKey))
            icon = imgs.Images[node.ImageKey];
        else if (node.ImageIndex >= 0 && node.ImageIndex < imgs.Images.Count)
            icon = imgs.Images[node.ImageIndex];

        if (icon != null)
        {
            g.DrawImage(icon, new Rectangle(iconLeft, iconTop, imgs.ImageSize.Width, imgs.ImageSize.Height));
        }

        int textLeft = iconLeft + imgs.ImageSize.Width + 4;
        Rectangle textRect = new Rectangle(textLeft, bounds.Top, bounds.Right - textLeft, bounds.Height);
        TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
        if (selected)
            TextRenderer.DrawText(g, node.Text, tree.Font, textRect, SystemColors.HighlightText, flags);
        else
            TextRenderer.DrawText(g, node.Text, tree.Font, textRect, tree.ForeColor, flags);

        if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused)
        {
            ControlPaint.DrawFocusRectangle(g, bounds);
        }
    }
    #endregion

    #region Mouse actions
    private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        tree.SelectedNode = e.Node;

        if (e.Button == MouseButtons.Right)
        {
            // Reconstruimos ContextMenu según el tipo de nodo (carpeta / archivo)
            ctx.Items.Clear();
            string path = e.Node.Tag as string;
            bool isDir = !string.IsNullOrEmpty(path) && Directory.Exists(path);

            if (isDir)
            {
                var nuevo = new ToolStripMenuItem(TextNew);
                var carpeta = new ToolStripMenuItem(TextFolder, null, (s, ev) => CreateNewFolder(path, e.Node));
                var archivo = new ToolStripMenuItem(TextFile, null, (s, ev) => CreateNewFile(path, e.Node));
                nuevo.DropDownItems.Add(carpeta);
                nuevo.DropDownItems.Add(archivo);
                ctx.Items.Add(nuevo);
                ctx.Items.Add(new ToolStripSeparator());
                ctx.Items.Add(TextOpenFileExplorer, null, (s, ev) =>
                {
                    try { System.Diagnostics.Process.Start(path); }
                    catch { }
                });

                // Agregar Rename para carpetas
                ctx.Items.Add(TextRename, null, (s, ev) => RenamePath(path, e.Node));

                ctx.Items.Add(TextProperties, null, (s, ev) => ShowProperties());
                ctx.Items.Add(new ToolStripSeparator());
                ctx.Items.Add(TextDelete, null, (s, ev) => DeletePath(path, e.Node));
            }
            else
            {
                ctx.Items.Add(TextOpen, null, (s, ev) => OpenSelected());

                // Agregar Rename para archivos
                ctx.Items.Add(TextRename, null, (s, ev) => RenamePath(path, e.Node));

                ctx.Items.Add(TextProperties, null, (s, ev) => ShowProperties());
                ctx.Items.Add(new ToolStripSeparator());
                ctx.Items.Add(TextDelete, null, (s, ev) => DeletePath(path, e.Node));
            }

            ctx.Show(tree, e.Location);
            return;
        }

        if (e.Button == MouseButtons.Left)
        {
            try
            {
                if (e.Node.Nodes.Count > 0)
                {
                    if (e.Node.IsExpanded) e.Node.Collapse();
                    else e.Node.Expand();
                }
            }
            catch { }
        }
    }

    private void Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node.Tag is string path && File.Exists(path))
        {
            Form form = this.FindForm();
            if (form is FrmBase mainMenu && ((FrmBase)mainMenu).ParentMenuForm != null)
            {
                FileExtension.OpenFile(path);
                return;
            }
            FileExtension.OpenFileWithDefaultProgram(path);
        }
    }

    private void OpenSelected()
    {
        if (tree.SelectedNode?.Tag is string path && File.Exists(path))
        {
            Form form = this.FindForm();
            if(form is FrmBase mainMenu && ((FrmBase)mainMenu).ParentMenuForm != null)
            {
                FileExtension.OpenFile(path);
                return;
            }
            FileExtension.OpenFileWithDefaultProgram(path);
        }
    }

    private void ShowProperties()
    {
        if (tree.SelectedNode?.Tag is string path)
            PZModdingStudio.Helpers.FilePropertiesHelper.ShowFileProperties(path, this.ParentForm.Handle);
        //MessageBox.Show(path, "Propiedades", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    #endregion

    #region Rename implementation
    private void RenamePath(string path, TreeNode node)
    {
        if (string.IsNullOrEmpty(path) || node == null) return;

        // No permitir renombrar la raíz de una unidad
        string root = Path.GetPathRoot(path) ?? string.Empty;
        if (string.Equals(path.TrimEnd(Path.DirectorySeparatorChar), root.TrimEnd(Path.DirectorySeparatorChar), StringComparison.InvariantCultureIgnoreCase))
        {
            MessageBox.Show(TextCantRenameRoot, TextWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (GetCurrentWorkspaceState().RootPath != null && GetCurrentWorkspaceState().RootPath == path)
        {
            MessageBox.Show(TextCantRenameRoot, TextWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool isDir = Directory.Exists(path);
        string currentName = Path.GetFileName(path);
        string input = ShowInputDialog(isDir ? TextDialogRenameFolderInput : TextDialogRenameFilenput, currentName);
        if (string.IsNullOrWhiteSpace(input)) return;

        string newName = SanitizeFileName(input.Trim());
        if (string.IsNullOrEmpty(newName)) return;

        string parent = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(parent)) return;

        string newPath = Path.Combine(parent, newName);
        if (string.Equals(newPath, path, StringComparison.InvariantCultureIgnoreCase)) return; // sin cambios

        // Si ya existe un archivo/carpet con ese nombre, avisar
        if (Directory.Exists(newPath) || File.Exists(newPath))
        {
            string msg = TextFolderAlreadyExists.Replace("{newName}", newName);
            MessageBox.Show(msg, TextWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (isDir)
            {
                Directory.Move(path, newPath);

                // Actualizar tags de todos los nodos descendientes para mantener rutas correctas
                UpdateNodePathRecursive(node, path, newPath);

                node.Tag = newPath;
                node.Text = Path.GetFileName(newPath);
            }
            else
            {
                File.Move(path, newPath);

                node.Tag = newPath;
                node.Text = Path.GetFileName(newPath);

                // Si cambió la extensión, actualizar icono
                string ext = Path.GetExtension(newPath).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext)) ext = ".file";
                string key = EnsureFileIcon(ext, newPath);
                node.ImageKey = key;
                node.SelectedImageKey = key;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(TextCantRenameError + " " + ex.Message, TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateNodePathRecursive(TreeNode node, string oldPrefix, string newPrefix)
    {
        if (node == null) return;
        if (node.Tag is string t && !string.IsNullOrEmpty(t))
        {
            // Reemplazar solo si comienza con el prefijo antiguo (ignorando mayúsculas)
            if (t.StartsWith(oldPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                string remainder = t.Substring(oldPrefix.Length);
                // Asegurar separador
                string newTag = newPrefix + remainder;
                node.Tag = newTag;
            }
        }

        foreach (TreeNode child in node.Nodes)
        {
            UpdateNodePathRecursive(child, oldPrefix, newPrefix);
        }
    }
    #endregion

    #region Delete (send to Recycle Bin)
    private void DeletePath(string path, TreeNode node)
    {
        if (string.IsNullOrEmpty(path)) return;

        // No permitir eliminar la raíz de una unidad
        string root = Path.GetPathRoot(path) ?? string.Empty;
        if (string.Equals(path.TrimEnd(Path.DirectorySeparatorChar), root.TrimEnd(Path.DirectorySeparatorChar), StringComparison.InvariantCultureIgnoreCase))
        {
            MessageBox.Show(TextCantDeleteRoot, TextWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (GetCurrentWorkspaceState().RootPath != null && GetCurrentWorkspaceState().RootPath == path)
        {
            MessageBox.Show(TextCantDeleteRoot, TextWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool isDir = Directory.Exists(path);
        string tipo = isDir ? TextFolder2 : TextFile2;
        string nombre = Path.GetFileName(path);
        string confirmacion = TextConfirmDelete.Replace("{type}", tipo).Replace("{name}", nombre);

        var confirm = MessageBox.Show(confirmacion, TextConfirmDeleteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        try
        {
            if (isDir)
            {
                //UIOption.OnlyErrorDialogs para que no muestre diálogos adicionales (ya mostramos confirmación)
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }

            // Remover nodo del tree
            if (node != null)
            {
                if (node.Parent != null)
                    node.Parent.Nodes.Remove(node);
                else
                    tree.Nodes.Remove(node);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(TextCantDeleteError + " " + ex.Message, TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    #endregion

    #region Create new folder/file
    private void CreateNewFolder(string parentPath, TreeNode parentNode)
    {
        string name = ShowInputDialog(TextDialogNewFolderInput, TextNewFolder);
        if (string.IsNullOrWhiteSpace(name)) return;
        name = SanitizeFileName(name.Trim());
        if (string.IsNullOrEmpty(name)) return;

        string newPath = Path.Combine(parentPath, name);
        int i = 1;
        string baseName = name;
        while (Directory.Exists(newPath) || File.Exists(newPath))
        {
            name = $"{baseName} ({i++})";
            newPath = Path.Combine(parentPath, name);
        }

        try
        {
            Directory.CreateDirectory(newPath);

            // Crear el nodo visual
            TreeNode tn = new TreeNode(Path.GetFileName(newPath))
            {
                Tag = newPath,
                ImageKey = "folder",
                SelectedImageKey = "folder"
            };
            tn.Nodes.Add(DUMMY); // para lazy
            parentNode.Nodes.Add(tn);
            try { parentNode.Expand(); } catch { }
        }
        catch (Exception ex)
        {
            MessageBox.Show(TextCantCreateFolderError + " " + ex.Message, TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CreateNewFile(string parentPath, TreeNode parentNode)
    {
        string name = ShowInputDialog(TextDialogNewFileInput, TextNewFile + ".txt");
        if (string.IsNullOrWhiteSpace(name)) return;
        name = SanitizeFileName(name.Trim());
        if (string.IsNullOrEmpty(name)) return;

        // Si no tiene extensión, darle .txt por defecto
        if (Path.GetExtension(name) == string.Empty)
            name = name + ".txt";

        string newPath = Path.Combine(parentPath, name);
        int i = 1;
        string baseName = Path.GetFileNameWithoutExtension(name);
        string ext = Path.GetExtension(name);
        while (File.Exists(newPath) || Directory.Exists(newPath))
        {
            string candidate = $"{baseName} ({i++}){ext}";
            newPath = Path.Combine(parentPath, candidate);
        }

        try
        {
            using (File.Create(newPath)) { }

            // Asegurar icono
            string key = EnsureFileIcon(ext.ToLowerInvariant(), newPath);

            TreeNode fn = new TreeNode(Path.GetFileName(newPath))
            {
                Tag = newPath,
                ImageKey = key,
                SelectedImageKey = key
            };
            parentNode.Nodes.Add(fn);
            try { parentNode.Expand(); } catch { }
        }
        catch (Exception ex)
        {
            MessageBox.Show(TextCantCreateFileError + " " + ex.Message, TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string SanitizeFileName(string name)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        foreach (var c in invalid)
            name = name.Replace(c.ToString(), "");
        return name;
    }

    private string ShowInputDialog(string prompt, string defaultText = "")
    {
        using (Form form = new Form())
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.ShowInTaskbar = false;
            form.ClientSize = new Size(400, 110);
            form.Text = TextInput;

            Label lbl = new Label() { Left = 9, Top = 10, Text = prompt, AutoSize = true };
            TextBox txt = new TextBox() { Left = 12, Top = 35, Width = 370, Text = defaultText };
            Button btnOk = new Button() { Text = TextAccept, Left = 220, Width = 80, Top = 70, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = TextCancel, Left = 305, Width = 80, Top = 70, DialogResult = DialogResult.Cancel };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog(this) == DialogResult.OK)
                return txt.Text;
            else
                return null;
        }
    }
    #endregion

    #region System icons
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    private const uint SHGFI_ICON = 0x000000100;
    private const uint SHGFI_SMALLICON = 0x000000001;
    private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
    private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

    private void AddSystemIconToList(string key, bool isFolder)
    {
        if (imgs.Images.ContainsKey(key)) return;
        Icon ico = GetSystemIcon(isFolder ? null : key, isFolder);
        if (ico != null)
        {
            imgs.Images.Add(key, ico.ToBitmap());
            ico.Dispose();
        }
    }

    private string EnsureFileIcon(string ext, string fullpath)
    {
        if (string.IsNullOrEmpty(ext)) ext = Path.GetExtension(fullpath);
        if (string.IsNullOrEmpty(ext)) ext = ".file";

        if (!imgs.Images.ContainsKey(ext))
        {
            Icon ico = GetSystemIcon(fullpath, false);
            if (ico != null)
            {
                imgs.Images.Add(ext, ico.ToBitmap());
                ico.Dispose();
            }
            else
            {
                imgs.Images.Add(ext, SystemIcons.WinLogo.ToBitmap());
            }
        }
        return ext;
    }

    private Icon GetSystemIcon(string pathOrExt, bool folder)
    {
        SHFILEINFO shinfo = new SHFILEINFO();
        uint flags = SHGFI_ICON | SHGFI_SMALLICON;
        uint attr = 0;
        string path = pathOrExt;

        if (folder)
        {
            flags |= SHGFI_USEFILEATTRIBUTES;
            attr = FILE_ATTRIBUTE_DIRECTORY;
            path = "folder";
        }
        else
        {
            if (string.IsNullOrEmpty(pathOrExt))
            {
                path = "file";
                flags |= SHGFI_USEFILEATTRIBUTES;
            }
            else if (!File.Exists(pathOrExt))
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
            }
        }

        IntPtr h = SHGetFileInfo(path, attr, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
        if (shinfo.hIcon != IntPtr.Zero)
        {
            Icon ico = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
            DestroyIcon(shinfo.hIcon);
            return ico;
        }
        return null;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);
    #endregion

    #region Root & Create Nodes
    public void SetRoot(string path, bool expandAll = false, int? expandDepth = null)
    {
        tree.BeginUpdate();
        try
        {
            tree.Nodes.Clear();

            if (!Directory.Exists(path)) return;

            var root = CreateDirectoryNode(path);
            tree.Nodes.Add(root);

            if (expandAll && expandDepth == null)
            {
                root.ExpandAll();
            }
            else if (expandDepth.HasValue)
            {
                ExpandNodesToDepth(root, expandDepth.Value);
            }
            else if (expandAll && expandDepth == 0)
            {
                root.Expand();
            }
        }
        finally
        {
            tree.EndUpdate();
        }
    }

    private void ExpandNodesToDepth(TreeNode node, int depth)
    {
        if (node == null) return;
        if (depth < 0) return;

        node.Expand();

        if (depth == 0) return;

        foreach (TreeNode child in node.Nodes)
        {
            try { ExpandNodesToDepth(child, depth - 1); } catch { }
        }
    }

    private TreeNode CreateDirectoryNode(string path)
    {
        var di = new DirectoryInfo(path);
        string displayName = string.IsNullOrEmpty(di.Name) ? path : di.Name;

        var directoryNode = new TreeNode(displayName)
        {
            Tag = path,
            ImageKey = "folder",
            SelectedImageKey = "folder"
        };

        try
        {
            string[] dirs = Directory.GetDirectories(path);
            Array.Sort(dirs, StringComparer.InvariantCultureIgnoreCase);
            foreach (var dir in dirs)
            {
                try { directoryNode.Nodes.Add(CreateDirectoryNode(dir)); } catch { }
            }

            string[] files = Directory.GetFiles(path);
            Array.Sort(files, StringComparer.InvariantCultureIgnoreCase);
            foreach (var f in files)
            {
                try
                {
                    var fi = new FileInfo(f);
                    string ext = fi.Extension.ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext)) ext = ".file";
                    string key = EnsureFileIcon(ext, f);

                    directoryNode.Nodes.Add(new TreeNode(fi.Name)
                    {
                        Tag = f,
                        ImageKey = key,
                        SelectedImageKey = key
                    });
                }
                catch { }
            }
        }
        catch { }

        return directoryNode;
    }
    #endregion

    #region Estado en string
    [Serializable]
    public class ExplorerState
    {
        public string RootPath { get; set; }
        public List<string> ExpandedPaths { get; set; }
        public string SelectedPath { get; set; }
        public string TopNodePath { get; set; }
        public ExplorerState() { ExpandedPaths = new List<string>(); }
    }

    public string GetStateString()
    {
        var state = CaptureState();
        var serializer = new XmlSerializer(typeof(ExplorerState));
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, state);
            return sw.ToString();
        }
    }

    public void SetStateFromString(string xml)
    {
        if (string.IsNullOrEmpty(xml)) return;

        var serializer = new XmlSerializer(typeof(ExplorerState));
        using (var sr = new StringReader(xml))
        {
            ExplorerState state = (ExplorerState)serializer.Deserialize(sr);
            RestoreState(state);
        }
    }

    public ExplorerState CaptureState()
    {
        var s = new ExplorerState();

        if (tree.Nodes.Count == 1 && tree.Nodes[0].Tag is string rootTag && Directory.Exists(rootTag))
            s.RootPath = rootTag;

        foreach (TreeNode n in GetAllNodes(tree.Nodes))
        {
            if (n.IsExpanded && n.Tag is string t && !string.IsNullOrEmpty(t))
                s.ExpandedPaths.Add(t);
        }

        if (tree.SelectedNode?.Tag is string sel) s.SelectedPath = sel;
        if (tree.TopNode?.Tag is string top) s.TopNodePath = top;

        return s;
    }

    public void RestoreState(ExplorerState state)
    {
        if (state == null) return;

        tree.BeginUpdate();
        try
        {
            if (!string.IsNullOrEmpty(state.RootPath) && Directory.Exists(state.RootPath))
                SetRoot(state.RootPath, expandAll: false, expandDepth: 0);
            else
                ReloadRoots();

            foreach (var p in state.ExpandedPaths)
            {
                try { EnsureNodeForPath(p, expand: true); } catch { }
            }

            if (!string.IsNullOrEmpty(state.SelectedPath))
            {
                var selNode = FindNodeByPath(state.SelectedPath);
                if (selNode != null) { tree.SelectedNode = selNode; selNode.EnsureVisible(); }
            }

            if (!string.IsNullOrEmpty(state.TopNodePath))
            {
                var top = FindNodeByPath(state.TopNodePath);
                if (top != null) { try { tree.TopNode = top; } catch { } }
            }
        }
        finally { tree.EndUpdate(); }
    }

    private TreeNode FindNodeByPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        foreach (TreeNode n in GetAllNodes(tree.Nodes))
            if (n.Tag is string t && string.Equals(t, path, StringComparison.InvariantCultureIgnoreCase))
                return n;
        return null;
    }

    private IEnumerable<TreeNode> GetAllNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode n in nodes)
        {
            yield return n;
            foreach (var child in GetAllNodes(n.Nodes))
                yield return child;
        }
    }

    private TreeNode EnsureNodeForPath(string path, bool expand = false)
    {
        if (string.IsNullOrEmpty(path)) return null;

        var existing = FindNodeByPath(path);
        if (existing != null)
        {
            if (expand && !existing.IsExpanded) existing.Expand();
            return existing;
        }

        var parentDir = Directory.GetParent(path);
        if (parentDir == null) return null;

        string parentPath = parentDir.FullName;
        var parentNode = FindNodeByPath(parentPath);
        if (parentNode == null)
        {
            EnsureNodeForPath(parentPath, expand: true);
            parentNode = FindNodeByPath(parentPath);
        }

        if (parentNode != null)
        {
            if (parentNode.Nodes.Count == 1 && parentNode.Nodes[0].Text == DUMMY)
            {
                try { AddDirectoryChildren(parentNode); } catch { }
            }

            var child = FindNodeByPath(path);
            if (child != null)
            {
                if (expand && !child.IsExpanded) child.Expand();
                return child;
            }

            try { parentNode.Expand(); } catch { }
            var child2 = FindNodeByPath(path);
            if (child2 != null)
            {
                if (expand && !child2.IsExpanded) child2.Expand();
                return child2;
            }
        }

        return null;
    }
    #endregion

    /// <summary>
    /// Restablece el control al estado inicial (como justo después de InitializeComponents).
    /// - Limpia nodos, limpia el ImageList y vuelve a añadir los íconos básicos,
    /// - limpia el ContextMenuStrip y restablece propiedades visuales a sus valores por defecto.
    /// No intenta cargar unidades; deja el TreeView vacío (igual que el constructor).
    /// </summary>
    public void ResetWorkspace()
    {
        tree.BeginUpdate();
        try
        {
            // Limpiar nodos y selección
            tree.Nodes.Clear();
            tree.SelectedNode = null;
            try { tree.TopNode = null; } catch { }

            // Limpiar y recrear ImageList mínimo
            imgs.Images.Clear();
            AddSystemIconToList("folder", true);
            AddSystemIconToList(".txt", false);

            // Limpiar menú contextual
            ctx.Items.Clear();

            // Restaurar propiedades públicas relacionadas con la apariencia
            ShowCaret = true;
            CaretOffset = -12;
            CaretSize = 10;

            // Si tenías algún estado guardado internamente, podrías limpiarlo aquí también.
            // Por ejemplo: eliminar caches, etc. (no hay ninguno en la versión actual)
        }
        finally
        {
            tree.EndUpdate();
        }
    }

    /// <summary>
    /// Recarga el workspace actual desde disco. 
    /// - Captura el estado actual (root, nodos expandidos, selección),
    /// - vuelve a construir la vista desde disco y re-aplica la expansión/selección.
    /// Si el control estaba mostrando una única raíz (SetRoot), recargará esa raíz;
    /// si estaba mostrando drives (ReloadRoots), volverá a listar las unidades.
    /// </summary>
    public void RefreshWorkspace()
    {
        // Capturamos el estado actual
        var state = CaptureState();

        tree.BeginUpdate();
        try
        {
            // Si teníamos una raíz explícita, la restauramos (esto reconstruirá su contenido).
            if (!string.IsNullOrEmpty(state.RootPath) && Directory.Exists(state.RootPath))
            {
                // Reconstruye la raíz (se volverán a leer directorios y archivos desde disco)
                SetRoot(state.RootPath, expandAll: false, expandDepth: 0);
            }
            else
            {
                // Si no hay raíz explícita, recargamos las unidades (equivalente a inicio)
                ReloadRoots();
            }

            // Reaplicar nodos expandidos (RestoreState ya hace esto, pero aquí lo hacemos manualmente para
            // que se reintente la expansión después de la reconstrucción).
            foreach (var p in state.ExpandedPaths)
            {
                try { EnsureNodeForPath(p, expand: true); } catch { }
            }

            // Reaplicar selección
            if (!string.IsNullOrEmpty(state.SelectedPath))
            {
                var selNode = FindNodeByPath(state.SelectedPath);
                if (selNode != null) { tree.SelectedNode = selNode; selNode.EnsureVisible(); }
            }

            // Reaplicar top node si existe
            if (!string.IsNullOrEmpty(state.TopNodePath))
            {
                var top = FindNodeByPath(state.TopNodePath);
                if (top != null) { try { tree.TopNode = top; } catch { } }
            }
        }
        finally
        {
            tree.EndUpdate();
        }
    }

    /// <summary>
    /// Devuelve el estado completo actual del workspace (rápido - no toca disco).
    /// Contiene RootPath, lista de rutas expandidas, selección y top node.
    /// </summary>
    public ExplorerState GetCurrentWorkspaceState()
    {
        return CaptureState();
    }

    /// <summary>
    /// Devuelve el workspace actual serializado a XML (útil para guardar en disco/config).
    /// </summary>
    public string GetCurrentWorkspaceAsXml()
    {
        return GetStateString();
    }

    /// <summary>
    /// Indica si el control está mostrando una única raíz (SetRoot) o la vista de unidades (ReloadRoots).
    /// Si devuelve true hay una raíz explícita en RootPath; si false probablemente se están mostrando las unidades.
    /// </summary>
    public bool IsSingleRootWorkspace()
    {
        var s = CaptureState();
        return !string.IsNullOrEmpty(s.RootPath);
    }


}

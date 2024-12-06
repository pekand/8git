﻿using _8Git.Forms;
using _8Git.Lib;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using System;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Packaging;
using System.Management.Automation.Runspaces;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

#nullable disable

namespace _8Git
{
    public partial class Form8Git : Form
    {
        public string path;

        public Dictionary<string, FormNote> noteForms = new Dictionary<string, FormNote>();

        public bool updated = false;

        public long tick = 0;
        public long lastSaveTick = 0;

        public TreeData copyNode = null;
        public TreeData cutNode = null;

        public bool internalDragAndDrop = false;
        public bool externalDragAndDrop = false;
        public bool mouseDownPress = false;
        public int mouseDownPressX = 0;
        public int mouseDownPressY = 0;

        private bool isDoubleClick = false;

        public int FormX = 0;
        public int FormY = 0;
        public int FormW = 0;
        public int FormH = 0;
        public bool FormVisible = true;

        public Dictionary<string, ToolStripMenuItem> contextMenuItems = new Dictionary<string, ToolStripMenuItem>();

        public bool isInicialized = false;
        public bool isPositionSet = false;

        // CONSTRUCTOR
        public Form8Git(string path)
        {
            this.DoubleBuffered = true;

            this.path = path;

            InitializeComponent();

            CreateContextmenuItems();

            treeView.LabelEdit = true;
            treeView.AllowDrop = true;

            treeView.ItemDrag += TreeView_ItemDrag;
            treeView.DragEnter += TreeView_DragEnter;
            treeView.DragOver += TreeView_DragOver;
            treeView.DragDrop += TreeView_DragDrop;
            treeView.NodeMouseDoubleClick += TreeView_NodeMouseDoubleClick;

            Program.tree = new Tree(this.treeView);

            Program.gitManager.OnRepositoryChange += RepositoryChanged;

            RestoreState();

            isInicialized = true;
        }

        // LOAD
        private void Form8Git_Load(object sender, EventArgs e)
        {
            isPositionSet = false;
            this.Left = this.FormX;
            this.Top = this.FormY;
            this.Width = this.FormW;
            this.Height = this.FormH;
            isPositionSet = true;
        }

        // TIMER
        private void timer_Tick(object sender, EventArgs e)
        {
            tick++;

            if (updated && (tick - lastSaveTick) > 120)
            {
                this.SaveState();
            }
        }

        // CLOSE
        private void Form8Git_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveState();
        }

        // MOUSE
        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            isDoubleClick = e.Clicks > 1;

            /*mouseDownPress = true;
             mouseDownPressX = e.X;
             mouseDownPressY = e.X;

             TreeNode clickedNode = treeView.GetNodeAt(e.X, e.Y);

             if (clickedNode != null)
             {
                 treeView.SelectedNode = clickedNode;
             }
             else {
                 treeView.SelectedNode = tree.rootNode.node;
             }*/
        }

        // MOUSE
        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            /*if (mouseDownPress)
            {
                if (treeView.SelectedNode != null && treeView.SelectedNode != tree.rootNode.node)
                {
                    if (Math.Abs(mouseDownPressX-e.X) > 10 || Math.Abs(mouseDownPressY - e.Y)>10)
                    {
                        DragDropEffects effect = DragDropEffects.Move;

                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            effect = DragDropEffects.Copy;
                        }

                        DoDragDrop(treeView.SelectedNode, effect);
                    }
                }
            }*/
        }

        // MOUSE
        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            //mouseDownPress = false;

            if (e.Button == MouseButtons.Right)
            {
                TreeNode clickedNode = treeView.GetNodeAt(e.X, e.Y);

                if (clickedNode != null)
                {
                    treeView.SelectedNode = clickedNode;

                    contextMenu.Show(treeView, e.Location);
                }
            }
        }

        // MOUSE
        public void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeData node = e.Node.Tag as TreeData;
            DoNodeAction(node);
            e.Node.Toggle();
        }

        // KEYBOARD
        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {

        }

        // KEYBOARD
        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && treeView.SelectedNode != null)
            {
                treeView.SelectedNode.BeginEdit();
            }
            else if (e.KeyCode == Keys.F4 && treeView.SelectedNode != null)
            {
                OpenNote();
            }
            else if (e.KeyCode == Keys.Delete && treeView.SelectedNode != null)
            {
                Program.tree.RemoveNodeHard(treeView.SelectedNode);
            }
        }

        // DRAG
        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.internalDragAndDrop = true;
            this.externalDragAndDrop = false;

            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        // DRAG
        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        // DRAG
        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

            treeView.SelectedNode = treeView.GetNodeAt(targetPoint);

            TreeNode targetNode = treeView.SelectedNode;

            if (targetNode == null)
            {
                return;
            }

            Rectangle targetNodeBounds = treeView.SelectedNode.Bounds;

            int blockSize = targetNodeBounds.Height / 3;

            bool addNodeUp = (targetPoint.Y < targetNodeBounds.Y + blockSize);
            bool addNodeIn = (targetNodeBounds.Y + blockSize <= targetPoint.Y && targetPoint.Y < targetNodeBounds.Y + 2 * blockSize);
            bool addNodeDown = (targetNodeBounds.Y + 2 * blockSize <= targetPoint.Y);

            Graphics g = treeView.CreateGraphics();
            Pen customPen = new Pen(Color.DimGray, 3) { DashStyle = DashStyle.Dash };
            Pen customPen2 = new Pen(treeView.BackColor, 3);


            if (addNodeUp)
            {
                g.DrawLine(customPen2, new Point(0, targetNode.Bounds.Bottom - 1), new Point(treeView.Width - 4, targetNode.Bounds.Bottom - 1));
                g.DrawLine(customPen, new Point(0, targetNode.Bounds.Top + 1), new Point(treeView.Width - 4, targetNode.Bounds.Top + 1));

            }

            if (addNodeIn)
            {
                g.DrawLine(customPen2, new Point(0, targetNode.Bounds.Top + 1), new Point(treeView.Width - 4, targetNode.Bounds.Top + 1));
                g.DrawLine(customPen2, new Point(0, targetNode.Bounds.Bottom - 1), new Point(treeView.Width - 4, targetNode.Bounds.Bottom - 1));
            }

            if (addNodeDown)
            {
                g.DrawLine(customPen2, new Point(0, targetNode.Bounds.Top + 1), new Point(treeView.Width - 4, targetNode.Bounds.Top + 1));
                g.DrawLine(customPen, new Point(0, targetNode.Bounds.Bottom - 1), new Point(treeView.Width - 4, targetNode.Bounds.Bottom - 1));
            }

            customPen.Dispose();
            customPen2.Dispose();

            g.Dispose();
        }

        // DRAG
        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {

            this.internalDragAndDrop = false;
            this.externalDragAndDrop = false;

            Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));


            TreeNode targetNode = treeView.GetNodeAt(targetPoint);

            if (targetNode == null)
            {
                targetNode = Program.tree.rootNode.node;
            }

            TreeData targetNodeData = (TreeData)targetNode.Tag;

            Rectangle targetNodeBounds = targetNode.Bounds;

            int blockSize = targetNodeBounds.Height / 3;

            bool addNodeUp = (targetPoint.Y < targetNodeBounds.Y + blockSize);
            bool addNodeIn = (targetNodeBounds.Y + blockSize <= targetPoint.Y && targetPoint.Y < targetNodeBounds.Y + 2 * blockSize);
            bool addNodeDown = (targetNodeBounds.Y + 2 * blockSize <= targetPoint.Y);

            /*var formats = e.Data.GetFormats();
            foreach (var format in formats)
            {
                Console.WriteLine(format);
            }*/

            string text = "";

            if (e.Data.GetDataPresent(DataFormats.StringFormat, false))
            {
                text = (string)(e.Data.GetData(DataFormats.StringFormat, false));
            }

            if (e.Data.GetDataPresent(DataFormats.Text, false))
            {
                text = (string)(e.Data.GetData(DataFormats.Text, false));
            }

            if (e.Data.GetDataPresent(DataFormats.Html, false))
            {
                text = (string)(e.Data.GetData(DataFormats.Html, false));
            }

            if (e.Data.GetDataPresent(DataFormats.UnicodeText, false))
            {
                text = (string)(e.Data.GetData(DataFormats.UnicodeText, false));
            }


            if (text != "")
            {

                TreeData data;

                if (Common.IsValidUrl(text))
                {
                    data = Program.tree.CreateNode(Common.GetShortUrl(text), null, "URL");
                    data.path = text;
                    Common.DownloadAndExtractTitleAsync(data);
                }
                else
                {
                    data = Program.tree.CreateNode("Note", null, "NOTE");
                    data.content = text;
                }

                if (addNodeUp && !targetNodeData.isRoot)
                {
                    Program.tree.MoveNodeBeforeNode(data.node, targetNode);
                    updated = true;
                }

                if (addNodeIn)
                {
                    Program.tree.MoveNodeInsideNode(data.node, targetNode);
                    updated = true;
                }

                if (addNodeDown && !targetNodeData.isRoot)
                {
                    Program.tree.MoveNodeAfterNode(data.node, targetNode);
                    updated = true;
                }
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] systemDrop = (string[])(e.Data.GetData(DataFormats.FileDrop, false));

                foreach (string path in systemDrop)
                {
                    if (!File.Exists(path) && !Directory.Exists(path))
                    {
                        continue;
                    }

                    string name = "File";

                    TreeData data = null;

                    if (Directory.Exists(path))
                    {
                        name = new DirectoryInfo(path).Name;
                        if (Git.IsRepository(path))
                        {
                            data = Program.gitManager.CreateRepository(name, path);
                        }
                        else
                        {
                            data = Program.tree.CreateNode(name, null, "DIRECTORY");
                        }
                        data.path = path;
                    }

                    if (File.Exists(path))
                    {
                        name = new FileInfo(path).Name;
                        data = Program.tree.CreateNode(name, null, "FILE");
                        data.path = path;
                    }

                    if (data == null)
                    {
                        continue;
                    }

                    if (addNodeUp && !targetNodeData.isRoot)
                    {
                        Program.tree.MoveNodeBeforeNode(data.node, targetNode);
                        updated = true;
                    }

                    if (addNodeIn)
                    {
                        Program.tree.MoveNodeInsideNode(data.node, targetNode);
                        updated = true;
                    }

                    if (addNodeDown && !targetNodeData.isRoot)
                    {
                        Program.tree.MoveNodeAfterNode(data.node, targetNode);
                        updated = true;
                    }
                }

                return;
            }


            if (targetNode == null)
            {
                treeView.Invalidate();
                return;
            }

            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (draggedNode == null)
            {
                treeView.Invalidate();
                return;
            }

            TreeData draggedNodeData = draggedNode.Tag as TreeData;

            if (draggedNodeData == null)
            {
                treeView.Invalidate();
                return;
            }

            if (Program.tree.IsParent(draggedNode, targetNode))
            {
                treeView.Invalidate();
                return;
            }

            if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                if (addNodeUp && !targetNodeData.isRoot)
                {
                    int previousNodeIndex = targetNode.Parent.Nodes.IndexOf(targetNode) - 1;

                    // move before target node
                    if (!draggedNode.Equals(targetNode))
                    {
                        Program.tree.MoveNodeBeforeNode(draggedNode, targetNode);
                        updated = true;
                    }
                }

                // move to target node
                if (addNodeIn && (!draggedNode.Equals(targetNode)))
                {
                    Program.tree.MoveNodeInsideNode(draggedNode, targetNode);
                    updated = true;
                }

                if (addNodeDown && !targetNodeData.isRoot)
                {

                    // move after target node
                    if (!draggedNode.Equals(targetNode))
                    {
                        Program.tree.MoveNodeAfterNode(draggedNode, targetNode);
                        updated = true;
                    }
                }
            }

            treeView.Invalidate();

        }

        // COLAPSE
        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeData note = e.Node.Tag as TreeData;
            if (!note.isFolder && isDoubleClick && e.Action == TreeViewAction.Expand)
                e.Cancel = true;
        }

        // COLAPSE
        private void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            Program.tree.ExpandNode(e.Node);
        }

        // COLLAPSE
        private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            TreeData note = e.Node.Tag as TreeData;
            if (!note.isFolder && isDoubleClick && e.Action == TreeViewAction.Collapse)
                e.Cancel = true;
        }

        // COLAPSE
        private void TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            Program.tree.CollapseNode(e.Node);
        }

        // RENAME
        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            Program.tree.RenameNode(e.Node, e.Label);

            TreeData data = e.Node.Tag as TreeData;
            data.name = e.Label;
            updated = true;
        }

        // RESIZE
        private void Form8Git_Resize(object sender, EventArgs e)
        {
            if (isPositionSet)
            {
                if (this.WindowState == FormWindowState.Maximized ||
                    this.WindowState == FormWindowState.Minimized)
                {
                    return;
                }

                FormX = this.Left;
                FormY = this.Top;
                FormW = this.Width;
                FormH = this.Height;
            }
        }

        // MOVE
        private void Form8Git_Move(object sender, EventArgs e)
        {
            if (isPositionSet)
            {
                if (this.WindowState == FormWindowState.Maximized ||
                this.WindowState == FormWindowState.Minimized)
                {
                    return;
                }

                FormX = this.Left;
                FormY = this.Top;
                FormW = this.Width;
                FormH = this.Height;
            }
        }

        // FORM VISIBILITY
        private void Form8Git_VisibleChanged(object sender, EventArgs e)
        {
            if (isPositionSet)
            {
                this.FormVisible = this.Visible;
            }
        }

        // CLOSING
        private void Form8Git_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        // STATE
        public void SaveState()
        {
            if (path == "")
            {
                return;
            }

            try
            {
                var xml = new XElement("Root",
                    new XElement("Left", FormX),
                    new XElement("Top", FormY),
                    new XElement("Width", FormW),
                    new XElement("Height", FormH),
                    new XElement("TopMost", this.TopMost),
                    new XElement("Visible", this.FormVisible)
                );

                var nodes = new XElement("Nodes");
                xml.Add(nodes);

                foreach (var data in Program.tree.nodes)
                {
                    var node = new XElement("Node",
                        new XElement("name", data.Value.name),
                        new XElement("id", data.Value.Id),
                        new XElement("parent", data.Value.parent != null ? data.Value.parent.Id : ""),
                        new XElement("path", data.Value.path),
                        new XElement("content", data.Value.content),
                        new XElement("expanded", data.Value.expanded.ToString()),
                        new XElement("isRoot", data.Value.isRoot.ToString()),
                        new XElement("isDirectory", data.Value.isDirectory.ToString()),
                        new XElement("isFile", data.Value.isFile.ToString()),
                        new XElement("isFolder", data.Value.isFolder.ToString()),
                        new XElement("isNote", data.Value.isNote.ToString()),
                        new XElement("isUrl", data.Value.isUrl.ToString()),
                        new XElement("isCommand", data.Value.isCommand.ToString()),
                        new XElement("isRepository", data.Value.isRepository.ToString())
                        );
                    nodes.Add(node);

                    var childs = new XElement("childs");
                    node.Add(childs);

                    foreach (var child in data.Value.nodes)
                    {
                        var childEl = new XElement("child", child.Id);
                        childs.Add(childEl);
                    }
                }

                xml.Save(this.path);

                updated = false;
            }
            catch (Exception ex)
            {
                Program.message(ex.Message);
            }
        }

        // STATE
        public void RestoreState()
        {
            if (path == "" || !File.Exists(this.path))
            {
                return;
            }

            try
            {
                Program.tree?.Clear();

                var xml = XElement.Load(this.path);

                this.StartPosition = FormStartPosition.Manual;

                foreach (XElement node1 in xml.Nodes())
                {
                    if (node1.Name == "Left")
                    {
                        this.FormX = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "Top")
                    {
                        this.FormY = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "Width")
                    {
                        this.FormW = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "Height")
                    {
                        this.FormH = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "TopMost")
                    {
                        this.TopMost = Common.GetBool(node1.Value);
                    }

                    if (node1.Name == "Visible")
                    {
                        this.FormVisible = Common.GetBool(node1.Value, true);
                    }

                    if (node1.Name == "Nodes")
                    {
                        foreach (XElement node2 in node1.Nodes())
                        {
                            if (node2.Name == "Node")
                            {
                                TreeData data = new TreeData();

                                foreach (XElement node3 in node2.Nodes())
                                {
                                    if (node3.Name == "id")
                                    {
                                        data.Id = node3.Value;
                                    }

                                    if (node3.Name == "name")
                                    {
                                        data.name = node3.Value;
                                    }

                                    if (node3.Name == "parent")
                                    {
                                        data.parentId = node3.Value;
                                    }

                                    if (node3.Name == "path")
                                    {
                                        data.path = node3.Value;
                                    }

                                    if (node3.Name == "content")
                                    {
                                        data.content = node3.Value;
                                    }

                                    if (node3.Name == "expanded")
                                    {
                                        data.expanded = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isRoot")
                                    {
                                        data.isRoot = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isDirectory")
                                    {
                                        data.isDirectory = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isFile")
                                    {
                                        data.isFile = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isFolder")
                                    {
                                        data.isFolder = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isNote")
                                    {
                                        data.isNote = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isUrl")
                                    {
                                        data.isUrl = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isCommand")
                                    {
                                        data.isCommand = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "isRepository")
                                    {
                                        data.isRepository = Common.GetBool(node3.Value);
                                    }

                                    if (node3.Name == "childs")
                                    {
                                        foreach (XElement node4 in node3.Nodes())
                                        {
                                            if (node4.Name == "child")
                                            {
                                                string childId = node4.Value;
                                                if (Common.IsGuid(data.Id))
                                                {
                                                    data.nodesIds.Add(childId);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (Common.IsGuid(data.Id))
                                {
                                    Program.tree.nodes[data.Id] = data;
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Program.message(ex.Message);
            }

            Program.tree.Init();

            updated = false;
        }

        // @CONTEXTMENU
        public void CreateContextmenuItems()
        {

            CreateContextmenuItem("/Repository", null, Icons.CreateUnicodeImage("📦", "#000000", "", 32));
            CreateContextmenuItem("/Repository/Pull", (s, e) => GitCommand("pull"));
            CreateContextmenuItem("/Repository/Fetch", (s, e) => GitCommand("fetch"));
            CreateContextmenuItem("/Repository/Push", (s, e) => GitCommand("push"));
            CreateContextmenuItem("/Repository/-");
            CreateContextmenuItem("/Repository/Diff", (s, e) => GitCommand("diff"));
            CreateContextmenuItem("/Repository/Log", (s, e) => GitCommand("log"));
            CreateContextmenuItem("/Repository/Revision graph", (s, e) => GitCommand("revisiongraph"));
            CreateContextmenuItem("/Repository/Tag", (s, e) => GitCommand("tag"));
            CreateContextmenuItem("/Repository/Branch", (s, e) => GitCommand("branch"));
            CreateContextmenuItem("/Repository/-");
            CreateContextmenuItem("/Repository/Stashsave", (s, e) => GitCommand("stashsave"));            
            CreateContextmenuItem("/Repository/Repo status", (s, e) => GitCommand("repostatus"));
            CreateContextmenuItem("/Repository/Repo browser", (s, e) => GitCommand("repobrowser"));
            CreateContextmenuItem("/Repository/Work tree list", (s, e) => GitCommand("worktreelist"));
            CreateContextmenuItem("/Repository/-");
            CreateContextmenuItem("/Repository/Other");
            CreateContextmenuItem("/Repository/Other/Submodule add", (s, e) => GitCommand("subadd"));
            CreateContextmenuItem("/Repository/Other/Submodule update", (s, e) => GitCommand("subupdate"));
            CreateContextmenuItem("/Repository/Other/Reflog", (s, e) => GitCommand("reflog"));
            CreateContextmenuItem("/Repository/Other/Refbrowse", (s, e) => GitCommand("refbrowse"));
            CreateContextmenuItem("/Repository/Other/Daemon", (s, e) => GitCommand("daemon"));
            CreateContextmenuItem("/Repository/Other/lfslocks", (s, e) => GitCommand("lfslocks"));
            CreateContextmenuItem("/Repository/TortoiseGit");
            CreateContextmenuItem("/Repository/TortoiseGit/Settings", (s, e) => GitCommand("settings"));
            CreateContextmenuItem("/Repository/TortoiseGit/Help", (s, e) => GitCommand("help"));
            CreateContextmenuItem("/Repository/TortoiseGit/Update check", (s, e) => GitCommand("updatecheck"));            
            CreateContextmenuItem("/Repository/TortoiseGit/About", (s, e) => GitCommand("about"));

            CreateContextmenuItem("/Open", (s, e) => OpenNode());
            CreateContextmenuItem("/Edit", (s, e) => EditNode());

            CreateContextmenuItem("/Create");
            CreateContextmenuItem("/Create/Folder", (s, e) => CreateFolder(), Icons.CreateUnicodeImage("📁", "#000000", "", 32));
            CreateContextmenuItem("/Create/Note", (s, e) => CreateNote(), Icons.CreateUnicodeImage("📒", "#000000", "", 32));
            CreateContextmenuItem("/Create/Command", (s, e) => CreateCommand(), Icons.CreateUnicodeImage("⚠", "#000000", "", 32));

            CreateContextmenuItem("/Toggle");
            CreateContextmenuItem("/Toggle/Toggle all", (s, e) => ToggleAll());
            CreateContextmenuItem("/Toggle/Expand all", (s, e) => ExpandAll(GetSelectedNode()));
            CreateContextmenuItem("/Toggle/Collapse all", (s, e) => CollapseAll(GetSelectedNode()));
            
            CreateContextmenuItem("/Node");
            CreateContextmenuItem("/Node/Copy", (s, e) => Copy(), Icons.CreateUnicodeImage("📋", "#000000", "", 32));
            CreateContextmenuItem("/Node/Cut", (s, e) => Cut(), Icons.CreateUnicodeImage("🔪", "#000000", "", 32));
            CreateContextmenuItem("/Node/Paste", (s, e) => Paste(), Icons.CreateUnicodeImage("📥", "#000000", "", 32));
            CreateContextmenuItem("/Node/Rename", (s, e) => RenameNode());
            CreateContextmenuItem("/Node/Delete", (s, e) => DeleteNode(), Icons.CreateUnicodeImage("⛔", "#FF0000", "", 32));

            CreateContextmenuItem("/File");
            CreateContextmenuItem("/File/Save file", (s, e) => Save());
            CreateContextmenuItem("/File/Save file as", (s, e) => SaveAs());
            CreateContextmenuItem("/File/Open file", (s, e) => Open());

            CreateContextmenuItem("/Tools");
            CreateContextmenuItem("/Tools/Open terminal", (s, e) => OpenTerminal());
            CreateContextmenuItem("/Tools/Open location", (s, e) => OpenLocation());

            CreateContextmenuItem("/Options");
            CreateContextmenuItem("/Options/Most top", (s, e) => ToggleMostTop());

            CreateContextmenuItem("/Exit", (s, e) => CloseApplication(), Icons.CreateUnicodeImage("✖", "#FF0000", "", 32));
        }

        private void GitCommand(string command)
        {
            TreeData node = this.GetSelectedNode();

            if (node == null || !node.isRepository) {
                return;
            }

            Command.OpenTortoiseGit(node.path, command, true);
        }

        // CONTEXTMENU
        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                return;
            }

            TreeData node = treeView.SelectedNode.Tag as TreeData;

            contextMenuItems["/Options/Most top"].Checked = this.TopMost;
            contextMenuItems["/Node/Delete"].Visible = true;
            contextMenuItems["/Open"].Visible = false;
            contextMenuItems["/Repository"].Visible = false;

            if (node.isRoot)
            {
                contextMenuItems["/Node/Delete"].Visible = false;
            }

            if (node.isFolder)
            {

            }

            if (node.isDirectory)
            {
                contextMenuItems["/Open"].Visible = true;
            }

            if (node.isFile)
            {
                contextMenuItems["/Open"].Visible = true;
            }

            if (node.isNote)
            {
                contextMenuItems["/Open"].Visible = true;
            }

            if (node.isUrl)
            {
                contextMenuItems["/Open"].Visible = true;
            }

            if (node.isCommand)
            {
                contextMenuItems["/Open"].Visible = true;
            }

            if (node.isRepository)
            {
                contextMenuItems["/Open"].Visible = true;
                contextMenuItems["/Repository"].Visible = true;
            }
        }

        // CLOSE
        public void CloseApplication()
        {
            Program.clossingApplication = true;
            System.Windows.Forms.Application.Exit();
        }

        // CONTEXTMENU
        public void ToggleAll()
        {
            if (treeView.SelectedNode == null)
            {
                return;
            }

            List<TreeData> nodes = new List<TreeData>();


            TreeData node = treeView.SelectedNode.Tag as TreeData;

            if (IsAllCollapsed(node))
            {
                ExpandAll(node);
            }
            else
            {
                CollapseAll(node);
            }
        }


        // COLLAPSE
        public bool IsAllCollapsed(TreeData node)
        {

            if (node.node.IsExpanded)
            {
                return false;
            }

            if (node.nodes.Count > 0)
            {
                foreach (TreeData child in node.nodes)
                {
                    if (child.node.IsExpanded)
                    {
                        return false;
                    }

                    if (!IsAllCollapsed(child))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // COLLAPSE
        public void ExpandAll(TreeData node)
        {
            if (node == null)
            {
                return;
            }

            node.node.Expand();
            node.expanded = true;
            foreach (TreeData child in node.nodes)
            {
                ExpandAll(child);
            }
        }

        // COLLAPSE
        public void CollapseAll(TreeData node)
        {
            if (node == null)
            {
                return;
            }

            node.node.Collapse();
            node.expanded = false;
            foreach (TreeData child in node.nodes)
            {
                CollapseAll(child);
            }
        }

        // NODE
        public TreeData GetSelectedNode()
        {
            if (treeView.SelectedNode == null)
            {
                return null;
            }

            return treeView.SelectedNode.Tag as TreeData;
        }

        // CONTEXTMENU
        public void OpenLocation()
        {
            if (treeView.SelectedNode == null)
            {
                return;
            }

            TreeData node = treeView.SelectedNode.Tag as TreeData;


            string path = node.path;
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }


            if (Directory.Exists(path))
            {
                Common.OpenDirectoryInExplorer(path);
            }
        }

        // CONTEXTMENU
        public void OpenTerminal()
        {
            if (treeView.SelectedNode == null)
            {
                return;
            }

            TreeData node = treeView.SelectedNode.Tag as TreeData;

            string path = node.path;
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }


            if (Directory.Exists(path))
            {
                Command.OpenPowershellTerminal(path);
            }
        }

        public ToolStripMenuItem CreateContextmenuItem(string path, EventHandler onclick = null, System.Drawing.Image icon = null)
        {
            string parent = path.Substring(0, path.LastIndexOf('/'));
            string child = path;
            string name = path.Substring(path.LastIndexOf('/')+1);

            if (name == "-") {

                var separator = new ToolStripSeparator();

                if (parent == "")
                {
                    contextMenu.Items.Add(separator);
                }
                else
                {
                    contextMenuItems[parent].DropDownItems.Add(separator);
                }
                return null;
            }

            if (parent != "" && !contextMenuItems.ContainsKey(parent))
            {
                return null;
            }

            if (contextMenuItems.ContainsKey(child))
            {
                return null;
            }

            contextMenuItems[child] = new ToolStripMenuItem(name, icon, onclick);

            if (parent == "")
            {
                contextMenu.Items.Add(contextMenuItems[child]);
            }
            else
            {
                contextMenuItems[parent].DropDownItems.Add(contextMenuItems[child]);
            }

            return contextMenuItems[child];
        }



        // FORM
        public void ToggleMostTop()
        {
            this.TopMost = !this.TopMost;
            contextMenuItems["TopMost"].Checked = this.TopMost;
        }

        // NODE
        public void OpenNode()
        {
            if (treeView.SelectedNode != null)
            {
                TreeData node = treeView.SelectedNode.Tag as TreeData;
                this.DoNodeAction(node);
            }
        }

        // NODE
        public void EditNode()
        {
            if (treeView.SelectedNode != null)
            {
                TreeData node = treeView.SelectedNode.Tag as TreeData;
                this.DoNodeAction(node, "EDIT");
            }
        }

        // NODE
        public void CreateFolder()
        {
            if (Program.tree.CreateFolder() != null)
            {
                updated = true;
            }
        }

        // NODE
        public void CreateNote()
        {
            if (Program.tree.CreateNote() != null)
            {
                updated = true;
            }
        }

        // NODE
        public void CreateCommand()
        {
            if (Program.tree.CreateCommand() != null)
            {
                updated = true;
            }
        }

        // NODE
        private void RenameNode()
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.BeginEdit();
                updated = true;
            }
        }

        // NODE
        private void DeleteNode()
        {
            if (treeView.SelectedNode != null)
            {
                Program.tree.RemoveNodeHard(treeView.SelectedNode);
                updated = true;
            }

        }

        // SAVE
        public void Save()
        {

            if (path == "")
            {
                this.SaveAs();
            }
            else
            {
                SaveState();
            }
        }

        // SAVE
        public void SaveAs()
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "MtExpSolver Files (*.8Git)|*.8Git|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "8Git";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.path = saveFileDialog.FileName;
                    SaveState();
                }
            }
        }

        // OPEN
        public void Open()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.8Git)|*.8Git|All Files (*.*)|*.*";
                openFileDialog.DefaultExt = "8Git";
                openFileDialog.AddExtension = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.path = openFileDialog.FileName;
                    RestoreState();
                }
            }
        }

        // NODE
        public void DoNodeAction(TreeData node, string actyonType = "OPEN")
        {

            if (actyonType == "OPEN")
            {
                if (node.isFolder || node.isRoot)
                {

                }

                if (node.isUrl)
                {
                    Common.OpenUrlInDefaultBrowser(node.path);
                }

                if (node.isFile)
                {
                    Common.OpenFileInDefaultApplication(node.path);
                }

                if (node.isDirectory)
                {
                    Common.OpenDirectoryInExplorer(node.path);
                }

                if (node.isRepository)
                {
                    //Common.OpenDirectoryInExplorer(node.path);
                    
                    if (!Command.OpenTortoiseGit(node.path)) {
                        if (!Command.OpenPowershellTerminal(node.path)) {
                            Command.OpenTerminal(node.path);
                        }
                    }
                }

                if (node.isNote)
                {
                    OpenNote();
                }

                if (node.isCommand)
                {

                    RunCommand(node);

                }
            }

            if (actyonType == "EDIT")
            {
                OpenNote();
            }

        }

        // COMMAND
        public void RunCommand(TreeData node)
        {
            if (node.isCommand)
            {
                Command.RunPowerShellCommand(node.content);
            }
        }

        // NODE
        public void OpenNote()
        {
            if (treeView.SelectedNode != null)
            {
                TreeData node = treeView.SelectedNode.Tag as TreeData;

                if (!noteForms.ContainsKey(node.Id) || noteForms[node.Id] == null || noteForms[node.Id].IsDisposed)
                {
                    FormNote note = new FormNote(node);
                    noteForms[node.Id] = note;
                    noteForms[node.Id].Show();
                }
                else
                {
                    noteForms[node.Id].BringToFront();
                }

            }
        }

        // NODE
        public void CloseNode(TreeData node)
        {
            if (noteForms.ContainsKey(node.Id))
            {
                noteForms.Remove(node.Id);
            }
        }

        // NODE
        public void Copy()
        {
            if (treeView.SelectedNode == null)
            {
                treeView.SelectedNode = Program.tree.rootNode.node;
            }

            copyNode = treeView.SelectedNode.Tag as TreeData;
            cutNode = null;

            string customFormat = Program.AppName + "_COPY_NODE";
            string customData = "";

            Clipboard.SetData(customFormat, customData);
        }

        // NODE
        public void Cut()
        {
            if (treeView.SelectedNode == null)
            {
                treeView.SelectedNode = Program.tree.rootNode.node;
            }

            copyNode = null;
            cutNode = treeView.SelectedNode.Tag as TreeData;

            string customFormat = Program.AppName + "_CUT_NODE";
            string customData = "";

            Clipboard.SetData(customFormat, customData);
        }

        // NODE
        public void Paste()
        {
            if (treeView.SelectedNode == null)
            {
                treeView.SelectedNode = Program.tree.rootNode.node;
            }

            if (Clipboard.ContainsData(Program.AppName + "_COPY_NODE"))
            {
                string retrievedData = Clipboard.GetData(Program.AppName + "_COPY_NODE") as string;
                updated = true;
            }

            if (Clipboard.ContainsData(Program.AppName + "_CUT_NODE"))
            {
                string retrievedData = Clipboard.GetData(Program.AppName + "_CUT_NODE") as string;
                updated = true;
            }

            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();

                if (Common.IsValidUrl(clipboardText))
                {
                    TreeData data = Program.tree.CreateNode(Common.GetShortUrl(clipboardText), null, "URL");
                    data.path = clipboardText;
                    Program.tree.MoveNodeInsideNode(data.node, treeView.SelectedNode);
                    treeView.SelectedNode.Expand();
                    updated = true;
                }
                else
                {
                    TreeData data = Program.tree.CreateNode("Note", null, "NOTE");
                    data.content = clipboardText;
                    Program.tree.MoveNodeInsideNode(data.node, treeView.SelectedNode);
                    treeView.SelectedNode.Expand();
                    updated = true;
                }
            }

            if (Clipboard.ContainsFileDropList())
            {
                var filePaths = Clipboard.GetFileDropList();
                foreach (string filePath in filePaths)
                {
                    if (Directory.Exists(path))
                    {
                        string name = new DirectoryInfo(path).Name;
                        TreeData data = Program.tree.CreateNode(name, null, "DIRECTORY");
                        data.path = path;

                        Program.tree.MoveNodeInsideNode(data.node, treeView.SelectedNode);
                        treeView.SelectedNode.Expand();
                        updated = true;
                    }

                    if (File.Exists(path))
                    {
                        string name = new FileInfo(path).Name;
                        TreeData data = Program.tree.CreateNode(name, null, "FILE");
                        data.path = path;

                        Program.tree.MoveNodeInsideNode(data.node, treeView.SelectedNode);
                        treeView.SelectedNode.Expand();
                        updated = true;
                    }
                }
            }
        }

        // TREEVIEW
        public System.Windows.Forms.TreeView GetTreeView()
        {
            return this.treeView;
        }

        // TREEVIEW
        public void IconUpdate(TreeData node, string type = "")
        {
            if (type == "repositoryChange")
            {
                node.node.ImageKey = "repositoryChange";
                node.node.SelectedImageKey = "repositoryChange";
            }

            if (type == "repository")
            {
                node.node.ImageKey = "repository";
                node.node.SelectedImageKey = "repository";
            }
        }

        public void IconUpdateInvoke(TreeData node, string type = "")
        {
            try
            {
                if (treeView.InvokeRequired)
                {
                    treeView.Invoke((MethodInvoker)(() =>
                    {
                        this.IconUpdate(node, type);

                    }));
                }
                else
                {
                    this.IconUpdate(node, type);
                }
            }
            catch
            {

                
            }
        }

        // TREEVIEW
        public void SetNodeTitle(TreeData node, string title)
        {
            treeView.Invoke(new Action(() =>
            {
                node.name = title;
                node.node.Text = title;
                treeView.Refresh();
            }));
        }

        // REPOSITORY EVENT
        void RepositoryChanged(TreeData node)
        {
            if (node.isRepositoryChanged)
            {
                IconUpdateInvoke(node, "repositoryChange");
            }
            else
            {
                IconUpdateInvoke(node, "repository");
            }
        }
    }
}
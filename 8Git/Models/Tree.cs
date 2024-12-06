using System.Windows;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

#nullable disable

namespace _8Git
{
    public class Tree
    {

        public string path;
        public bool updated = false;

        public TreeView treeView = null;
        Form8Git form8Git = null;

        public TreeData rootNode = null;
        public Dictionary<string, TreeData> nodes = new Dictionary<string, TreeData>();

        public Tree(TreeView treeView, Form8Git form8Git, string path) {
            this.path = path;
            this.form8Git = form8Git;
            this.treeView = treeView;
            treeView.ImageList = Icons.GetImageList();   
        }


        /**************************************************************************/

        // @STATE SAVE
        public void SaveState()
        {
            if (path == "")
            {
                return;
            }

            try
            {
                var xml = new XElement("Root",
                    new XElement("Left", this.form8Git.FormX),
                    new XElement("Top", this.form8Git.FormY),
                    new XElement("Width", this.form8Git.FormW),
                    new XElement("Height", this.form8Git.FormH),
                    new XElement("TopMost", this.form8Git.TopMost),
                    new XElement("Visible", this.form8Git.FormVisible)
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

        public void ClearState()
        {
            Program.CloseAllNoteForms();                         
            Program.gitManager.repositories.Clear();
            treeView.Nodes.Clear();
            nodes.Clear();
            rootNode = null;
        }

        // @STATE RESTORE
        public void RestoreState()
        {
            if (path == "" || !File.Exists(this.path))
            {
                return;
            }

            try
            {
                this.ClearState();

                var xml = XElement.Load(this.path);

                this.form8Git.StartPosition = FormStartPosition.Manual;

                foreach (XElement node1 in xml.Nodes())
                {
                    if (node1.Name == "Left")
                    {
                        this.form8Git.FormX = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "Top")
                    {
                        this.form8Git.FormY = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "Width")
                    {
                        this.form8Git.FormW = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "Height")
                    {
                        this.form8Git.FormH = Common.GetInt(node1.Value);
                    }

                    if (node1.Name == "TopMost")
                    {
                        this.form8Git.TopMost = Common.GetBool(node1.Value);
                    }

                    if (node1.Name == "Visible")
                    {
                        this.form8Git.FormVisible = Common.GetBool(node1.Value, true);
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

        public void Init()
        {

            bool isValid = true;

            if (this.nodes != null && this.nodes.Count > 0)
            {
                foreach (var node in this.nodes)
                {
                    if (node.Value.isRoot)
                    {
                        rootNode = node.Value;
                    }

                    if (node.Value.parentId != "")
                    {
                        if (node.Value.parentId != null && this.nodes[node.Value.parentId] != null)
                        {
                            node.Value.parent = this.nodes[node.Value.parentId];
                        }
                    }

                    if (node.Value.nodesIds.Count > 0)
                    {
                        foreach (var childId in node.Value.nodesIds)
                        {
                            if (this.nodes.ContainsKey(childId) &&  this.nodes[childId] != null)
                            {
                                node.Value.nodes.Add(this.nodes[childId]);
                            }
                        }

                    }
                }

                TreeNode root = BuildTree(rootNode);
                treeView.Nodes.Add(root);

                RemoveDangling(rootNode);
            }

            if (this.nodes.Count == 0 || rootNode == null)
            {
                isValid = false;
            }

            if (!isValid)
            {
                nodes.Clear();
                treeView.Nodes.Clear();
                rootNode = CreateNode("8Git", null, "ROOT");
                treeView.Nodes.Add(rootNode.node);
            }

            Program.gitManager.Init();
        }

        public TreeNode BuildTree(TreeData node)
        {
            TreeNode tnode = new TreeNode(node.name);
            tnode.Tag = node;
            node.node = tnode;

            tnode.ImageKey = "folder";
            tnode.SelectedImageKey = "folder";


            if (node.isFolder)
            {
                tnode.ImageKey = "folder";
                tnode.SelectedImageKey = "folder";
            }

            if (node.isRoot)
            {
                tnode.ImageKey = "root";
                tnode.SelectedImageKey = "root";
            }

            if (node.isDirectory)
            {
                tnode.ImageKey = "directory";
                tnode.SelectedImageKey = "directory";
            }

            if (node.isRepository)
            {
                tnode.ImageKey = "repository";
                tnode.SelectedImageKey = "repository";
            }

            if (node.isFile)
            {
                tnode.ImageKey = "file";
                tnode.SelectedImageKey = "file";
            }

            if (node.isNote)
            {
                tnode.ImageKey = "note";
                tnode.SelectedImageKey = "note";
            }

            if (node.isUrl)
            {
                tnode.ImageKey = "url";
                tnode.SelectedImageKey = "url";
            }

            if (node.isCommand)
            {
                tnode.ImageKey = "command";
                tnode.SelectedImageKey = "command";
            }

            foreach (var child in node.nodes) {
                BuildTree(child);
            }

            if (node.parent != null)
            {
                node.parent.node.Nodes.Add(tnode);
            }

            if (node.expanded) {
                tnode.Expand();
            }

            return tnode;
        }

        public void RemoveDangling(TreeData rootNode) {
            List<TreeData> toRemove = new List<TreeData>();

            foreach (var node in nodes)
            {
                TreeData treeData = node.Value;

                /*if (treeData.isRoot && treeData.Id != rootNode.Id) {
                   toRemove.Add(treeData);
                   continue;
                }*/

                TreeData parent = treeData.parent;
                int level = 100;
                if (parent == null)
                {
                    if (treeData.Id != rootNode.Id) {
                        toRemove.Add(treeData);
                    }
                }
                else
                {
                    bool isLoop = false;
                    while (level > 0)
                    {
                        level--;

                        if (parent != null && parent.Id == treeData.Id) // loop detected (node is self perent)
                        {
                            isLoop = true;
                            break;
                        }

                        if (parent.parent != null) {
                            parent = parent.parent;
                        }

                        if (parent.parent == null) { 
                            break;
                        }                                                
                    }

                    if (level > 0) {
                        if (isLoop || (parent != null && parent.parent == null && parent.Id != rootNode.Id))
                        {
                            toRemove.Add(treeData);
                        }
                    }
                }
            }

            foreach (var node in toRemove)
            {
               this.nodes.Remove(node.Id);
            }
        }

        public TreeData CreateNode(string name, TreeData parent = null, string type = "FOLDER", bool expand = false)
        {
            TreeNode node = new TreeNode(name);
            TreeData data = new TreeData();
            node.Tag = data;
            data.Id = Common.GetId();
            data.name = name;
            data.node = node;
            data.parent = parent;

            node.ImageKey = "folder";
            node.SelectedImageKey = "folder";

            if (type == "ROOT")
            {
                data.isRoot = true;
                data.isFolder = true;
                data.parent = null;
                node.ImageKey = "root";
                node.SelectedImageKey = "root";
            }

            if (type == "FOLDER")
            {            
                data.isFolder = true;
                node.ImageKey = "folder";
                node.SelectedImageKey = "folder";
            }

            if (type == "DIRECTORY")
            {
                data.isDirectory = true;
                node.ImageKey = "directory";
                node.SelectedImageKey = "directory";
            }

            if (type == "REPOSITORY")
            {
                data.isRepository = true;
                node.ImageKey = "repository";
                node.SelectedImageKey = "repository";
            }

            if (type == "FILE")
            {
                data.isFile = true;
                node.ImageKey = "file";
                node.SelectedImageKey = "file";
            }

            if (type == "NOTE")
            {
                data.isNote = true;
                node.ImageKey = "note";
                node.SelectedImageKey = "note";
            }

            if (type == "URL")
            {
                data.isUrl = true;
                node.ImageKey = "url";
                node.SelectedImageKey = "url";
            }
            if (type == "COMMAND")
            {
                data.isCommand = true;
                node.ImageKey = "command";
                node.SelectedImageKey = "command";
            }


            nodes[data.Id] = data;

            if (data.parent != null)
            {
                data.parent.nodes.Add(data);
                parent.node.Nodes.Add(node);

                if (expand)
                {
                    data.parent.expanded = true;
                    data.parent.node.Expand();
                }

            }

            return data;
        }

        public TreeData SelectedNode()
        {            
            if (this.treeView.SelectedNode == null) { 
                return null;
            }

            return this.treeView.SelectedNode.Tag as TreeData;
        }

        public TreeData CreateFolder(string name = "Folder")
        {
            TreeData parent = SelectedNode();

            if (parent == null)
            {
                return null;
            }

            return CreateNode(name, parent, "FOLDER", true);
        }

        public TreeData CreateNote(string name = "Note")
        {
            TreeData parent = SelectedNode();

            if (parent == null)
            {
                return null;
            }

            return CreateNode(name, parent, "NOTE", true);
        }

        public TreeData CreateCommand(string name = "Command")
        {
            TreeData parent = SelectedNode();

            if (parent == null)
            {
                return null;
            }

            return CreateNode(name, parent, "COMMAND", true);
        }

        public void ExpandNode(TreeNode node)
        {
            TreeData data = node.Tag as TreeData;
            data.expanded = true;
            node.Expand();
        }

        public void CollapseNode(TreeNode node)
        {
            TreeData data = node.Tag as TreeData;
            data.expanded = false;
            node.Collapse();
        }

        public void RenameNode(TreeNode node, string newName)
        {
            TreeData data = node.Tag as TreeData;
            node.Name = newName;
            data.name = newName;            
        }

        public void Clear()
        {
            treeView?.Nodes.Clear();
        }

        public bool IsDescendant(TreeNode parent, TreeNode potentialDescendant)
        {
            TreeNode current = potentialDescendant;
            while (current != null)
            {
                if (current == parent)
                {
                    return true;
                }
                current = current.Parent;
            }
            return false;
        }

        public bool IsParent(TreeNode maybeParent, TreeNode child)
        {
            if (child.Parent == null) return false;
            if (child.Parent.Equals(maybeParent)) return true;

            return IsParent(maybeParent, child.Parent);
        }

        public bool RemoveNodeSoft(TreeNode targetNode)
        {
            if (targetNode == null)
            {
                return false;
            }

            TreeData targetNodeData = targetNode.Tag as TreeData;

            if (targetNodeData != null)
            {
                if (targetNodeData.parent != null)
                {
                    TreeData parentNodeData = targetNodeData.parent;

                    parentNodeData.nodesIds.Remove(targetNodeData.Id);
                    parentNodeData.nodes.Remove(targetNodeData);
                    targetNodeData.parentId = null;
                    targetNodeData.parent = null;

                    parentNodeData.node.Nodes.Remove(targetNodeData.node);

                    this.nodes.Remove(targetNodeData.Id);

                }
                else
                {
                    if (treeView.Nodes.Contains(targetNodeData.node)) {
                        treeView.Nodes.Remove(targetNodeData.node);
                    }
                }
            }

            return true;
        }

        public bool RemoveNodeHard(TreeNode targetNode)
        {
            if (targetNode == null)
            {
                return false;
            }

            if (targetNode == rootNode.node)
            {
                return false;
            }


            TreeData targetNodeData = targetNode.Tag as TreeData;

            if (targetNodeData != null) {                
                foreach (TreeData data in new List<TreeData>(targetNodeData.nodes)) {
                    RemoveNodeHard(data.node);
                }
            }

            if (targetNodeData.isRepository) {
                Program.gitManager.RemoveRepository(targetNodeData);
            }

            return RemoveNodeSoft(targetNode); ;
        }

        public bool MoveNodeBeforeNode(TreeNode sourceNode, TreeNode targetNode) {

            if (sourceNode == null || targetNode == null || sourceNode == targetNode)
            {
                return false;
            }

            TreeData sourceNodeData = sourceNode.Tag as TreeData;
            TreeData targetNodeData = targetNode.Tag as TreeData;

            if (targetNodeData.parent != null)
            {
                TreeData parentTargetNodeData = targetNodeData.parent;

                RemoveNodeSoft(sourceNode);

                sourceNodeData.parent = parentTargetNodeData;
                sourceNodeData.parentId = parentTargetNodeData.Id;
                
                Common.InsertBefore(parentTargetNodeData.nodesIds, targetNodeData.Id, sourceNodeData.Id);
                Common.InsertBefore(parentTargetNodeData.nodes, targetNodeData, sourceNodeData);
                Common.InsertBefore(parentTargetNodeData.node.Nodes, targetNodeData.node, sourceNodeData.node);

                this.nodes[sourceNodeData.Id] = sourceNodeData;
            }
            else {
                return false;
            }

            return true;
        }

        public bool MoveNodeAfterNode(TreeNode sourceNode, TreeNode targetNode)
        {
            if (sourceNode == null || targetNode == null || sourceNode == targetNode)
            {
                return false;
            }

            TreeData sourceNodeData = sourceNode.Tag as TreeData;
            TreeData targetNodeData = targetNode.Tag as TreeData;

            if (targetNodeData.parent != null)
            {
                TreeData parentTargetNodeData = targetNodeData.parent;

                RemoveNodeSoft(sourceNode);

                sourceNodeData.parent = parentTargetNodeData;
                sourceNodeData.parentId = parentTargetNodeData.Id;

                Common.InsertAfter(parentTargetNodeData.nodesIds, targetNodeData.Id, sourceNodeData.Id);
                Common.InsertAfter(parentTargetNodeData.nodes, targetNodeData, sourceNodeData);
                Common.InsertAfter(parentTargetNodeData.node.Nodes, targetNodeData.node, sourceNodeData.node);

                this.nodes[sourceNodeData.Id] = sourceNodeData;
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool MoveNodeInsideNode(TreeNode sourceNode, TreeNode targetNode)
        {
            if (sourceNode == null || targetNode == null || sourceNode == targetNode) {
                return false;
            }

            TreeData sourceNodeData = sourceNode.Tag as TreeData;
            TreeData targetNodeData = targetNode.Tag as TreeData;

            RemoveNodeSoft(sourceNode);

            sourceNodeData.parent = targetNodeData;
            sourceNodeData.parentId = targetNodeData.Id;
            targetNodeData.nodes.Add(sourceNodeData);
            targetNodeData.nodesIds.Add(sourceNodeData.Id);
            targetNodeData.node.Nodes.Add(sourceNodeData.node);

            this.nodes[sourceNodeData.Id] = sourceNodeData;

            return true;
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

        // NODE
        public void Copy()
        {
            try
            {
                TreeData copyNode = this.GetSelectedNode();

                if (copyNode == null)
                {
                    return;
                }

                Dictionary<string, TreeData> list = new Dictionary<string, TreeData>();
                Dictionary<string, string> ids = new Dictionary<string, string>();
                Dictionary<string, TreeData> newList = new Dictionary<string, TreeData>();

                this.GetTreeNodes(copyNode, ref list);


                foreach (var child in list) {
                    ids[child.Value.Id] = Common.GetId();
                }

                foreach (var child in list)
                {
                    TreeData data = new TreeData();
                    data.Id = ids[child.Value.Id];
                    data.name = child.Value.name;
                    data.parentId = ids.ContainsKey(child.Value.parentId) ? ids[child.Value.parentId] : null;
                    data.nodes = new List<TreeData>();
                    data.nodesIds = new List<string>();
                    data.expanded = child.Value.expanded;
                    data.isRoot = child.Value.Id == copyNode.Id; // set as temporary subtree root for copy operation
                    data.isFolder = child.Value.isFolder;
                    data.isFile = child.Value.isFile;
                    data.isDirectory = child.Value.isDirectory;
                    data.isNote = child.Value.isNote;
                    data.isUrl = child.Value.isUrl;
                    data.isCommand = child.Value.isCommand;
                    data.path = child.Value.path;
                    data.content = child.Value.content;
                    data.isRepository = child.Value.isRepository;
                    newList[data.Id] = data;
                }

                foreach (var child in list)
                {
                    string newId = ids[child.Value.Id];
                    string newPerentId = ids.ContainsKey(child.Value.parentId) ? ids[child.Value.parentId] : null;

                    newList[newId].parent = newPerentId != null ? newList[newPerentId] : null;

                    foreach (var id in child.Value.nodesIds) {
                        if (ids.ContainsKey(id)) {
                            string newChildId = ids[id];
                            newList[newId].nodesIds.Add(newChildId);
                            newList[newId].nodes.Add(newList[newChildId]);
                        }
                    }
                }

                var nodes = new XElement("Nodes");
                
                foreach (var data in newList)
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

                string customFormat = Program.AppName + "_PASTE_NODE";
                string customData = nodes.ToString();

                System.Windows.Forms.Clipboard.SetData(customFormat, customData);
            }
            catch (Exception ex)
            {
                Program.message(ex.Message);
            }
        }

        // NODE
        public void Cut()
        {
            TreeData copyNode = this.GetSelectedNode();

            if (copyNode == null)
            {
                return;
            }

            this.Copy();

            this.RemoveNodeHard(copyNode.node);

        }

        public void PasteNode(string retrievedData) {
            try
            {
                TreeData pasteNode = this.GetSelectedNode();

                if (pasteNode == null)
                {
                    return;
                }

                var xml = XElement.Parse(retrievedData);

                Dictionary<string, TreeData> nodes = new Dictionary<string, TreeData>();
                Dictionary<string, TreeData> repositories = new Dictionary<string, TreeData>();

                foreach (XElement node2 in xml.Nodes())
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
                            nodes[data.Id] = data;
                        }
                    }
                }

                TreeData rootNode = null;

                foreach (var node in nodes)
                {
                    node.Value.parent = (node.Value.parentId != null && nodes.ContainsKey(node.Value.parentId)) ? nodes[node.Value.parentId] : null;

                    TreeNode treeNode = new TreeNode(node.Value.name);
                    node.Value.node = treeNode;
                    treeNode.Tag = node.Value;

                    string type = "FOLDER";
                    treeNode.ImageKey = "folder";
                    treeNode.SelectedImageKey = "folder";

                    if (node.Value.isRoot) {
                        type = "FOLDER";                        
                        rootNode = node.Value;
                        node.Value.isRoot = false;
                    }

                    if (node.Value.isFile)
                    {
                        type = "FILE";
                        rootNode = node.Value;
                        treeNode.ImageKey = "file";
                        treeNode.SelectedImageKey = "file";
                    }

                    if (node.Value.isDirectory)
                    {
                        type = "DIRECTORY";
                        rootNode = node.Value;
                        treeNode.ImageKey = "directory";
                        treeNode.SelectedImageKey = "directory";
                    }

                    if (node.Value.isNote)
                    {
                        type = "NOTE";
                        treeNode.ImageKey = "note";
                        treeNode.SelectedImageKey = "note";
                    }

                    if (node.Value.isUrl)
                    {
                        type = "URL";
                        treeNode.ImageKey = "url";
                        treeNode.SelectedImageKey = "url";
                    }

                    if (node.Value.isCommand)
                    {
                        type = "COMMAND";
                        treeNode.ImageKey = "command";
                        treeNode.SelectedImageKey = "command";
                    }

                    if (node.Value.isRepository)
                    {
                        repositories[node.Value.Id] = node.Value;
                        type = "REPOSITORY";
                        treeNode.ImageKey = "repository";
                        treeNode.SelectedImageKey = "repository";
                    }
                }

                foreach (var node in nodes)
                {
                    foreach (var child in node.Value.nodesIds)
                    {
                        TreeData childNode = (nodes.ContainsKey(child)) ? nodes[child] : null;

                        if (childNode != null)
                        {
                            node.Value.nodes.Add(childNode);
                        }
                    }
                }

                foreach (var node in nodes)
                {
                    foreach (var child in node.Value.nodes)
                    {
                        if (node.Value.node != null && child.node != null) {
                            node.Value.node.Nodes.Add(child.node);
                        }
                    }
                }

                if (rootNode != null) {
                    foreach (var node in nodes)
                    {
                        this.nodes[node.Value.Id] = node.Value;
                    }

                    rootNode.parent = pasteNode;
                    rootNode.parentId = pasteNode.Id;
                    pasteNode.nodesIds.Add(rootNode.Id);
                    pasteNode.node.Nodes.Add(rootNode.node);
                    pasteNode.nodes.Add(rootNode);
                    this.updated = true;

                    foreach (var repository in repositories) {
                        Program.gitManager.AddRepository(repository.Value);
                        Program.gitManager.CheckRepositorieState(repository.Value);
                    }
                }

            }
            catch (Exception ex)
            {
                Program.message(ex.Message);
            }
        }

        // NODE
        public void Paste()
        {
            if (treeView.SelectedNode == null)
            {
                treeView.SelectedNode = Program.tree.rootNode.node;
            }

            if (System.Windows.Forms.Clipboard.ContainsData(Program.AppName + "_PASTE_NODE"))
            {
                string retrievedData = System.Windows.Forms.Clipboard.GetData(Program.AppName + "_PASTE_NODE") as string;
                PasteNode(retrievedData);
                updated = true;
            }

            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                string clipboardText = System.Windows.Forms.Clipboard.GetText();

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

            if (System.Windows.Forms.Clipboard.ContainsFileDropList())
            {
                var filePaths = System.Windows.Forms.Clipboard.GetFileDropList();
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

        public void GetTreeNodes(TreeData node, ref Dictionary<string,TreeData> list)
        {
            if (node == null)
            {
                return;
            }

            list[node.Id] = node;
            
            foreach (TreeData child in node.nodes)
            {
                GetTreeNodes(child, ref list);
            }
        }

    }
}

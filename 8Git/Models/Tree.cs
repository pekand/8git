using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;
using System.Drawing;

#nullable disable

namespace _8Git
{
    public class Tree
    {
        public TreeView treeView = null;

        public Icon root = null;
        public Icon folder = null;        
        public Icon directory = null;
        public Icon file = null;
        public Icon note = null;
        public Icon url = null;
        public Icon command = null;
        public Icon repository = null;
        public Icon repositoryChange = null;
        
        private ImageList imageList;

        public TreeData rootNode = null;
        public Dictionary<string, TreeData> nodes = new Dictionary<string, TreeData>();

        public Tree(TreeView treeView) {
            this.treeView = treeView;
            treeView.ImageList = GetImageList();   
        }

        public void Init() {

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
                        if (node.Value.parentId!= null && this.nodes[node.Value.parentId] != null)
                        {
                            node.Value.parent = this.nodes[node.Value.parentId];
                        }
                    }

                    if (node.Value.nodesIds.Count > 0)
                    {
                        foreach (var childId in node.Value.nodesIds)
                        {
                            if (this.nodes[childId] != null)
                            {
                                node.Value.nodes.Add(this.nodes[childId]);
                            }
                        }

                    }
                }

                TreeNode root = BuildTree(rootNode);
                treeView.Nodes.Add(root);
            }

            if (this.nodes.Count == 0 ||rootNode == null) {
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

        public ImageList GetImageList() {

            if (imageList != null) { 
                return imageList;
            }

            imageList = new ImageList
            {
                ImageSize = new Size(32, 32)
            };

            root = Icons.CreateUnicodeIcon("❽", "#000000", "", 32);
            imageList.Images.Add("root", root);

            folder = Icons.CreateUnicodeIcon("📁", "#000000", "", 32);
            imageList.Images.Add("folder", folder);

            directory = Icons.CreateUnicodeIcon("📂", "#000000", "", 32);
            imageList.Images.Add("directory", directory);

            file = Icons.CreateUnicodeIcon("📄", "#000000", "", 32);
            imageList.Images.Add("file", file);

            //✅❎⬡⭐
            note = Icons.CreateUnicodeIcon("📒", "#000000", "", 32); 
            imageList.Images.Add("note", note);

            url = Icons.CreateUnicodeIcon("⚓", "#000000", "", 32);
            imageList.Images.Add("url", url);
            
            command = Icons.CreateUnicodeIcon("⚠", "#000000", "", 32);
            imageList.Images.Add("command", command);

            repository = Icons.CreateUnicodeIcon("📦", "#000000", "", 32);
            imageList.Images.Add("repository", repository);

            repositoryChange = Icons.CreateUnicodeIcon("📦", "#FF0000", "", 32);
            imageList.Images.Add("repositoryChange", repositoryChange);

            return imageList;
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


    }
}

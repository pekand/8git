using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace _8Git
{
    public class TreeData
    {
        public string Id = null;
        public string name = null;

        public TreeNode node = null;
        public string parentId = null;
        public TreeData parent = null;

        public List<TreeData> nodes = new List<TreeData>();
        public List<string> nodesIds = new List<string>();

        public bool expanded = false;

        public bool isRoot = false;
        public bool isFolder = false;

        public bool isFile = false;
        public bool isDirectory = false;
        public bool isNote = false;
        public bool isUrl = false;
        public bool isCommand = false;

        public string path = "";
        public string content = "";

        public bool isRepository = false;
        public bool isRepositoryChanged = false;
        public DirectoryMonitor directoryMonitor = null;

        public bool Contain(string NodeId) {
            foreach (TreeData node in nodes)
            {
                if (node.Id == NodeId) { 
                    return true;
                }
            }
            return false;
        }

        public TreeData GetChild(string NodeId)
        {
            foreach (TreeData node in nodes)
            {
                if (node.Id == NodeId)
                {
                    return node;
                }
            }
            return null;
        }

        public bool RemoveChild(string NodeId)
        {
            TreeData treeData = null;

            foreach (TreeData node in nodes)
            {
                if (node.Id == NodeId)
                {
                    treeData = node;
                    break;
                }
            }

            if (treeData != null) { 
                this.nodes.Remove(treeData);
                return true;
            }

            return false;
        }
    }
}

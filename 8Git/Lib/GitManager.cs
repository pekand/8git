using _8Git.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;

#nullable disable

namespace _8Git.Lib
{
    public class GitManager
    {
        private static System.Timers.Timer timer;

        private static long elapsedSeconds = 0;

        public Dictionary<string, TreeData> repositories = new Dictionary<string, TreeData>();

        public delegate void RepositoryChanged(TreeData node);

        public event RepositoryChanged OnRepositoryChange;

        public GitManager() {
            

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
            //timer.Stop();
            //timer.Dispose();
        }

        public TreeData CreateRepository(string name, string path)
        {
            TreeData node = Program.tree.CreateNode(name, null, "REPOSITORY");
            repositories.Add(node.Id, node);
            this.InitRepository(node);
            return node;
        }

        public void InitRepository(TreeData node)
        {
            if (node.isRepository && Directory.Exists(node.path) && node.directoryMonitor == null)
            {
                Program.message("REPOSITORY INIT:"+ node.path);
                CheckRepositorieState(node);
                node.directoryMonitor = new DirectoryMonitor(node.path);
                node.directoryMonitor.OnChange += (changedPath) =>
                {  
                    RepositoryChange(node);
                };

            }
        }

        public void DestroyRepository(TreeData node)
        {
            if (node.isRepository && node.directoryMonitor != null)
            {
                Program.message("REPOSITORY DESTROY:" + node.path);
                node.directoryMonitor.Dispose();
                node.directoryMonitor = null;
            }
        }

        public void RepositoryChange(TreeData node) {
            CheckRepositorieState(node);
        }

        public void AddRepository(TreeData node) {
            if (node.isRepository && !repositories.ContainsKey(node.Id)) {
                repositories.Add(node.Id, node);
                this.InitRepository(node);
            }
        }

        public void RemoveRepository(TreeData node)
        {
            if (repositories.ContainsKey(node.Id)) {
                DestroyRepository(node);
                repositories.Remove(node.Id);
            }
        }

        public void CheckRepositorieState(TreeData node)
        {
            try
            {
                if (node.isRepository && Directory.Exists(node.path) &&
                    Program.clossingApplication == false)
                {
                    Program.message("REPOSITORY CHECK:" + node.path);
                    if (Git.HasUncommitedChanges(node.path))
                    {
                        if (node.isRepositoryChanged == false) {
                            node.isRepositoryChanged = true;
                            OnRepositoryChange.Invoke(node);
                        }
                    }
                    else
                    {
                        if (node.isRepositoryChanged == true)
                        {
                            node.isRepositoryChanged = false;
                            OnRepositoryChange.Invoke(node);
                        }
                    }
                }
            }
            catch
            {

            }
            
        }

        public void SetRepositorieIconsInTree()
        {
            try
            {
                foreach (var node in repositories)
                {
                    if (node.Value.isRepositoryChanged)
                    {
                        Program.form8Git.IconUpdate(node.Value, "repositoryChange");
                    }
                    else {
                        Program.form8Git.IconUpdate(node.Value, "repository");
                    }
                }
            }
            catch
            {

            }

        }


        public void CheckRepositoriesState() {
            if (repositories.Count > 0) {
                foreach (var repository in repositories)
                {
                    CheckRepositorieState(repository.Value);
                }
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            elapsedSeconds++;

            /*if ((elapsedSeconds-lastRepositoryCheck)>30) {
                lastRepositoryCheck = elapsedSeconds;
                CheckRepositoriesState();
            }*/
        }

        public void AddRepositories()
        {
            if (Program.tree.nodes.Count > 0)
            {
                foreach (var node in Program.tree.nodes)
                {
                    if (node.Value.isRepository)
                    {
                        AddRepository(node.Value);
                    }
                }
            }
        }

        public void RemoveRepositories()
        {
            if (this.repositories.Count > 0)
            {
                foreach (var node in repositories)
                {
                    if (node.Value.isRepository)
                    {
                        RemoveRepository(node.Value);
                    }
                }
            }
        }

        public void Init()
        {
            Thread thread = new Thread(AddRepositories);
            thread.Start();
        }
    }
}

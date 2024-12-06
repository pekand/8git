using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Timers;

#nullable disable

namespace _8Git
{
    public class DirectoryMonitor
    {
        public string Path { get; private set; }
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _debounceTimer;
        private string _lastChangedPath;
        private bool _eventPending;

        public event Action<string> OnChange;

        public DirectoryMonitor(string path, int debounceInterval = 5000)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory '{path}' does not exist.");

            Path = path;

            _watcher = new FileSystemWatcher(Path)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true, // Set to true if you want to monitor subdirectories
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
            _watcher.Deleted += OnFileChanged;
            _watcher.Renamed += OnFileRenamed;

            _debounceTimer = new System.Timers.Timer(debounceInterval)
            {
                AutoReset = false
            };
            _debounceTimer.Elapsed += DebounceElapsed;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Program.message("CHANGE  : " + e.FullPath);

            string path = e.FullPath.StartsWith(this.Path)
        ? e.FullPath.Substring(this.Path.Length+1)
        : e.FullPath;

            string[] parts = path.Split(System.IO.Path.DirectorySeparatorChar);

            if (parts.Count() == 0) {
                Program.message("EXCLUDED: " + e.FullPath);
                return;
            }

            if (Directory.Exists(e.FullPath))
            {
                Program.message("EXCLUDED: " + e.FullPath);//test12356
                return;
            }

            string[] gitParts = { "hooks", "info", "logs", "objects", "refs"};

            if (parts.Count() >=2 && parts[0] == ".git" && gitParts.Contains(parts[1])) {
                Program.message("ACCEPT  : " + e.FullPath);
                return;
            }

            string[] excludeDirs = {
            ".git",
            ".vs", 
            ".idea", 
            ".vscode", 
            ".terraform", 
            ".mvn", 
            ".gradle", 
            ".next",
            ".cache",
            ".coverage",
            ".DS_Store",
            "venv", 
            ".pytest_cache", 
            "node_modules",
            "composer",
            "bin", 
            "obj" 
            };

            foreach (var exclude in excludeDirs) {
                if (parts.Contains(exclude))
                {
                    Program.message("EXCLUDED: " + e.FullPath);
                    return;
                }
            }

            Program.message("ACCEPT  : " + e.FullPath);
            ScheduleEvent(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            ScheduleEvent(e.FullPath);
        }

        private void ScheduleEvent(string path)
        {
            _lastChangedPath = path;
            if (!_eventPending)
            {
                _eventPending = true;
                _debounceTimer.Start();
            }
        }

        private void DebounceElapsed(object sender, ElapsedEventArgs e)
        {
            _eventPending = false;
            OnChange?.Invoke(_lastChangedPath);
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Changed -= OnFileChanged;
                _watcher.Created -= OnFileChanged;
                _watcher.Deleted -= OnFileChanged;
                _watcher.Renamed -= OnFileRenamed;
                _watcher.Dispose();
                _watcher = null;
            }

            if (_debounceTimer != null)
            {
                _debounceTimer.Elapsed -= DebounceElapsed;
                _debounceTimer.Dispose();
                _debounceTimer = null;
            }
        }
    }

}

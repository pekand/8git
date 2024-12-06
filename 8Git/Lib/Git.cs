using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace _8Git
{
    public class Git
    {
        public static string gitPath = "";
        public static string gitVersion = "";

        public static void Init()
        {
            gitPath = FindGitPath();
            gitVersion = GetGitVersion();
        }

        public static string FindGitPath()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "where" : "which",
                    Arguments = "git",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        return "";
                    }

                    string[] lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                    foreach (string line in lines)
                    {
                        if (File.Exists(line))
                        {
                            return Path.GetFullPath(line);
                        }
                    }
                }
            }
            catch
            {
               
            }

            return "";
        }

        public static string RunGitCommand(string command, string workdir = "")
        {
            if (gitPath == "")
            {
                return "";
            }

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = gitPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };


                if (workdir != "" && Directory.Exists(workdir))
                {
                    processStartInfo.WorkingDirectory = workdir;
                }

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        return "";
                    }

                    return output;
                }
            }
            catch
            {

            }

            return "";
        }

        public static string GetGitVersion()
        {
            string output = RunGitCommand("--version");
            return output.Replace("git version", "").Trim();
        }

        public static bool IsRepository(string path)
        {
            string output = RunGitCommand("rev-parse --is-inside-work-tree", path);
            if (output.ToLower() == "true")
            {
                string output2 = RunGitCommand("rev-parse --show-toplevel", path);
                if (Common.NormalizePath(output2) == path)
                {
                    return true;
                }                
            }

            return false;
        }

        public static bool HasUncommitedChanges(string path)
        {
            string output = RunGitCommand("status --porcelain", path);
            return output.Trim() != "";
        }
    }
}

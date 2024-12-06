using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable disable

namespace _8Git
{
    public class Command
    {
        public static string powershell = null;
        public static string tortoiseGit = null;

        public static string IsCommandAvailable(string cmd = "")
        {
            string pathEnv = Environment.GetEnvironmentVariable("PATH");

            if (string.IsNullOrEmpty(pathEnv))
                return null;

            string[] paths = pathEnv.Split(Path.PathSeparator);

            foreach (string path in paths)
            {
                string cmdPath = Path.Combine(path, cmd);

                if (File.Exists(cmdPath))
                {
                    return cmdPath;
                }
            }

            return null;
        }

        public static void FindPowershell()
        {
            powershell = IsCommandAvailable("pwsh.exe");

            if (powershell == null)
            {
                powershell = IsCommandAvailable("powershell.exe");
            }
        }

        public static bool OpenPowershellTerminal(string directory)
        {
            if (Command.powershell == null)
            {
                FindPowershell();
            }

            if (Command.powershell == null)
            {
                return false;
            }

            string targetDirectory = directory;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Command.powershell,
                WorkingDirectory = targetDirectory,
                UseShellExecute = true
            };

            Process.Start(psi);
            return true;
        }

        public static bool OpenTerminal(string directory)
        {
            string targetDirectory = directory;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WorkingDirectory = targetDirectory,
                UseShellExecute = true
            };

            Process.Start(psi);
            return true;
        }

        public static bool OpenTortoiseGit(string directory, string command = "commit", bool runInBackground = false)
        {
            if (Command.tortoiseGit == null)
            {
                Command.tortoiseGit = IsCommandAvailable("TortoiseGitProc.exe");
            }

            if (Command.tortoiseGit == null)
            {
                return false;
            }

            if (runInBackground) {
                Task.Run(() => OpenTortoiseGit(directory, command, false));
                return true;
            }

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = Command.tortoiseGit;
            startInfo.Arguments = $"/command:"+ command + " /path:"+ Common.AddQuotesIfContainsSpaces(directory);

            startInfo.WorkingDirectory = directory;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = false;

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;

            process.StartInfo = startInfo;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            bool exited = process.WaitForExit(60*1000*10);

            if (!exited)
            {
                Program.message("Process timed out. Killing process...");
                process.Kill();
                process.WaitForExit();

            }

            return true;
        }

        public static void RunPowerShellCommand(string command, bool silentCommand = true)
        {
            
            if (Command.powershell == null)
            {
                FindPowershell();
            }
                
            if (Command.powershell == null)
            {
                Program.message("Powershell not available. Install powershell.");
                return;
            }

            Job.DoJob(
                new DoWorkEventHandler(
                    delegate (object o, DoWorkEventArgs args)
                    {
                        var job = (BackgroundJob)args.Argument;

                        if (Common.IsValidUrl(command))
                        {
                            Process.Start(new ProcessStartInfo(command) { UseShellExecute = true });
                        }
                        else if (Directory.Exists(command))
                        {
                            System.Diagnostics.Process.Start("explorer.exe", command);
                        }
                        else if (File.Exists(command))
                        {
                            System.Diagnostics.Process.Start(command);
                        }
                        else
                        {
                            try
                            {
                                if (silentCommand)
                                {
                                    try
                                    {
                                        using (PowerShell ps = PowerShell.Create())
                                        {
                                            ps.AddScript(command);

                                            try
                                            {
                                                var result = ps.BeginInvoke();
                                                Program.message(result.ToString());
                                                while (!result.IsCompleted)
                                                {
                                                    if (job.token.IsCancellationRequested)
                                                    {
                                                        ps.Stop();
                                                        job.token.ThrowIfCancellationRequested();
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                Program.message(ex.Message);
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                        Program.message(ex.Message);
                                    }

                                }
                                else
                                {
                                    Process process = new Process();
                                    ProcessStartInfo startInfo = new ProcessStartInfo();

                                    startInfo.FileName = Command.powershell;
                                    string escapedCmd = Common.EscapeString(command);
                                    if (silentCommand)
                                    {
                                        startInfo.Arguments = $"-NoProfile -Command \"{escapedCmd}\"";
                                    }
                                    else
                                    {
                                        startInfo.Arguments = $"-NoExit -NoProfile -Command \"{escapedCmd}\"";
                                    }

                                    //startInfo.WorkingDirectory = workdir;
                                    startInfo.UseShellExecute = true;
                                    startInfo.RedirectStandardOutput = false;
                                    startInfo.RedirectStandardError = false;
                                    startInfo.CreateNoWindow = false;
                                    if (silentCommand)
                                    {
                                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                        startInfo.CreateNoWindow = true;
                                    }
                                    process.StartInfo = startInfo;
                                    process.Start();
                                }
                            }
                            catch (Exception ex)
                            {
                                Program.message("exception: " + ex.Message);
                            }
                        }

                    }
                ),
                new RunWorkerCompletedEventHandler(
                    delegate (object o, RunWorkerCompletedEventArgs args)
                    {

                    }
                )
            );
        }
    }
}

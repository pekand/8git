using _8Git.Forms;
using _8Git.Lib;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

#nullable disable

namespace _8Git
{
    internal static class Program
    {
        public static string AppName = "8Git";

        public static string roamingPath = "";

        public static FormMain formMain = null;

        public static Form8Git form8Git = null;

        public static Dictionary<string, FormNote> noteForms = new Dictionary<string, FormNote>();

        public static Tree tree;

        public static GitManager gitManager;


        public static bool clossingApplication = false;

#if DEBUG
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
#endif


        public static void message(string message)
        {
#if DEBUG
            Console.WriteLine(Common.UnixTime()+" "+message);
#endif
        }

        private static void MakeFileBackup(string originalPath)
        {
            if (!File.Exists(originalPath)) {
                return;
            }

            string directory = Path.GetDirectoryName(originalPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);

            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string backupPath = Path.Combine(directory, $"{fileNameWithoutExtension}_{unixTimestamp}{extension}");

            if (!File.Exists(backupPath))
            {
                File.Copy(originalPath, backupPath);
            }
        }

        // NOTIFICATION
        public static void ShowBubble(string text, Form parent)
        {
            Rectangle workingArea = Screen.FromControl(parent).WorkingArea;
            Bubble bubble = new Bubble();
            bubble.ShowBubble(workingArea.Right - bubble.Width-50, workingArea.Bottom - bubble.Height - 70, text);
        }

        // PIPE SERVER
        public static void OnMessageReceived(string message)
        {
            Program.message("Message Received: " + message);
            if (message == "Second instance run") {
                Program.form8Git.Invoke(() => Program.form8Git.Show());
            }
        }

        // PIPE SERVER
        public static void InitPipeServer(string path)
        {
            PipeServer.MessageReceived += OnMessageReceived;
            PipeServer.SetPipeName(Program.AppName + "-" + Common.CalcHash(path));
        }

        public static bool StartPipeServer()
        {
            if (!PipeServer.StartServerAsync())
            {
                return false;
            }

            return true;
        }

        public static void CloseAllNoteForms()
        {
            foreach (var form in noteForms) {
                form.Value.Close();
            }
            noteForms.Clear();
        }

        [STAThread]
        static void Main(string[] args)
        {

#if DEBUG
            AllocConsole();
#endif

            string path = "";

            Program.message("Program start");

            roamingPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppName
            );

            if (!Directory.Exists(roamingPath))
            {
                Directory.CreateDirectory(roamingPath);
            }

            path = Path.Combine(roamingPath, "config.8Git");


            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    path = arg;
                }
            }

            InitPipeServer(path);

            if (!Common.createMutex(path))
            {
                PipeServer.SendMessageAsync("Second instance run");
                return;
            }

            StartPipeServer();

            

#if DEBUG
            MakeFileBackup(path);

#endif
            Git.Init();

            ApplicationConfiguration.Initialize();

            Program.message(path);

            formMain = new FormMain();
            gitManager = new GitManager();
            form8Git = new Form8Git(path);                        
            

            Application.Run(formMain);

            Program.gitManager.RemoveRepositories();

            Program.message("Program end");
        }
    }
}
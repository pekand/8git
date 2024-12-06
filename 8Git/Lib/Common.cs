using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace _8Git
{
    public class Common
    {
        public static Mutex mutex = null;

        public static string CalcHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes); // Converts bytes to a hex string.
            }
        }

        public static bool createMutex(string path)
        {

            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }

            string mutexName = "Global\\" + Program.AppName + "-" + CalcHash(path);

            bool createdNew;
            mutex = new Mutex(true, mutexName, out createdNew);

            if (!createdNew)
            {
                return false;
            }

            return true;
        }

        public static Color GetColor(string color)
        {
            try
            {
                return ColorTranslator.FromHtml(color);
            }
            catch (Exception)
            {


            }

            return Color.Black;
        }

        public static string GetId()
        {
            return Guid.NewGuid().ToString();
        }
        public static bool IsGuid(string input)
        {
            string pattern = @"^[{(]?[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}[)}]?$";
            return Regex.IsMatch(input, pattern);
        }

        public static int GetInt(string value, int defaultValue = 0) {
            try
            {
                return int.Parse(value);
            }
            catch (Exception)
            {

                return defaultValue;
            }
        }

        public static bool GetBool(string value, bool defaultValue = false)
        {
            try
            {
                return bool.Parse(value);
            }
            catch (Exception)
            {

                return defaultValue;
            }
        }

        public static void InsertBefore<T>(List<T> list, T existingItem, T newItem)
        {
            int index = list.IndexOf(existingItem);
            if (index != -1)
            {
                list.Insert(index, newItem);
            }
        }

        public static void InsertAfter<T>(List<T> list, T existingItem, T newItem)
        {
            int index = list.IndexOf(existingItem);
            if (index != -1)
            {
                list.Insert(index + 1, newItem);
            }
        }

        internal static void InsertBefore(TreeNodeCollection list, TreeNode existingItem, TreeNode newItem)
        {
            int index = list.IndexOf(existingItem);
            if (index != -1)
            {
                list.Insert(index, newItem);
            }
        }

        public static void InsertAfter(TreeNodeCollection list, TreeNode existingItem, TreeNode newItem)
        {
            int index = list.IndexOf(existingItem);
            if (index != -1)
            {
                list.Insert(index + 1, newItem);
            }
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static void OpenUrlInDefaultBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {

            }
        }

        public static void OpenFileInDefaultApplication(string filePath)
        {
            if (!File.Exists(filePath)) {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
                Console.WriteLine($"Opened file: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
            }
        }

        public static void OpenDirectoryInExplorer(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = directoryPath,
                    UseShellExecute = true
                });
                
            }
            catch
            {
                
            }
        }

        public static string GetShortUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string domain = uri.Host.Replace("www.", "");
                return domain;
            }
            catch (Exception)
            {

                
            }

            return "URL";
        }
       
        public static string NormalizePath(string path)
        {
            char separator = Path.DirectorySeparatorChar; // System-specific separator
            char oppositeSeparator = separator == '/' ? '\\' : '/';

            // Replace the opposite separator with the system-specific one
            return path.Replace(oppositeSeparator, separator);
        }

        public static string UnixTime() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }

        public static string EscapeString(string input)
        {
            StringBuilder escaped = new StringBuilder();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '\\':
                        escaped.Append(@"\\");
                        break;
                    case '\"':
                        escaped.Append("\\\"");
                        break;
                    case '\'':
                        escaped.Append("\\'");
                        break;
                    case '\n':
                        escaped.Append(" ; ");
                        break;
                    case '\r':
                        escaped.Append("");
                        break;
                    default:
                        escaped.Append(c);
                        break;
                }
            }
            return escaped.ToString();
        }

        public static void DownloadAndExtractTitleAsync(TreeData node)
        {            
           Task.Run(() => GetTitleFromUrlAsync(node));                
        }

        public static string GetTitleFromUrlAsync(TreeData node)
        {
            string url = node.path;

            if (IsValidUrl(url))
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string htmlContent = client.GetStringAsync(url).Result;

                        string title = ExtractTitle(htmlContent);
                        if (title != "") {
                            Program.form8Git.SetNodeTitle(node, title);
                        }
                    }
                }
                catch
                {

                }
            }

            return "";
        }

        public static string ExtractTitle(string html)
        {
            Match match = Regex.Match(html, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "No title found";
        }
        public static string AddQuotesIfContainsSpaces(string input)
        {
            if (input.Contains(" "))
            {
                return $"\"{input}\"";
            }
            return input;
        }
    }
}

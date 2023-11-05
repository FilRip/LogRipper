using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using LogRipper.Constants;
using LogRipper.Exceptions;

namespace LogRipper.Helpers
{
    internal static class AutoUpdater
    {
        private const string UpdateFolder = "AutoUpdate";
        private const string UpdateExtension = ".update";
        private static readonly string AutoUpdateDirectory = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), UpdateFolder);

        internal static string LatestVersion(bool beta)
        {
            WebClient client = null;
            string result = null;
            try
            {
                client = new WebClient();
                string readme = client.DownloadString(new Uri($"https://github.com/FilRip/LogRipper/blob/master/README.md"));
                if (!string.IsNullOrWhiteSpace(readme))
                {
                    string search = $"Latest {(beta ? "beta " : "")}version=";
                    int pos = readme.IndexOf(search);
                    if (pos > -1)
                    {
                        int lastPos = readme.IndexOf('\\', pos + search.Length);
                        StringBuilder sb = new(readme.Substring(pos + search.Length, lastPos - pos - search.Length));
                        while (sb.ToString().Count(c => c == '.') < 3)
                        {
                            sb.Append(".0");
                        }
                        result = sb.ToString();
                    }
                }
            }
            catch (Exception) { /* Ignore errors */ }
            finally
            {
                client?.Dispose();
            }
            return result;
        }

        internal static void InstallNewVersion(string version)
        {
            WebClient client = null;
            try
            {
                string destFile = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "LogRipper.zip");
                if (File.Exists(destFile))
                    File.Delete(destFile);
                client = new WebClient();
                client.DownloadFile(new Uri($"https://github.com/FilRip/LogRipper/releases/download/v{version}/LogRipper.zip"), destFile);
                if (!File.Exists(destFile) || new FileInfo(destFile).Length == 0)
                    throw new LogRipperException(Locale.LBL_ERROR_DOWNLOAD_NEW_VERSION);
                if (!ZipManager.Extract(AutoUpdateDirectory, destFile))
                    throw new LogRipperException(Locale.LBL_ERROR_EXTRACT_NEW_VERSION);
                File.Delete(destFile);
                if (!UpdateFile(AutoUpdateDirectory))
                    throw new LogRipperException(Locale.LBL_ERROR_INSTALL_NEW_VERSION);
                RemoveAllAutoUpdateDir(AutoUpdateDirectory);
                if (WpfMessageBox.ShowModal(Locale.LBL_RESTART_APP_NEW_VERSION, "LogRipper", MessageBoxButton.YesNo))
                    Restart();
            }
            catch { /* Ignore errors */ }
            finally
            {
                client?.Dispose();
            }
        }

        internal static void SearchNewVersion(bool beta)
        {
            Task.Run(() =>
            {
                CleanUpUpdate();
                try
                {
                    string version = LatestVersion(beta);
                    if (!string.IsNullOrWhiteSpace(version))
                    {
                        Version latestVersion = new(version);
                        if (Assembly.GetEntryAssembly().GetName().Version.CompareTo(latestVersion) < 0 &&
                            WpfMessageBox.ShowModal(string.Format(Locale.LBL_NEW_VERSION, latestVersion.ToString()), "LogRipper", MessageBoxButton.YesNo))
                        {
                            InstallNewVersion(version);
                        }
                    }
                }
                catch (Exception) { /* Ignore errors */ }
            });
        }

        private static void CleanUpUpdate()
        {
            if (Directory.Exists(AutoUpdateDirectory))
                RemoveAllAutoUpdateDir(AutoUpdateDirectory);
            RemoveAllOlderFile(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));
        }

        private static void RemoveAllAutoUpdateDir(string dir)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                File.Delete(file);
            }

            foreach (string subDir in Directory.GetDirectories(dir))
            {
                RemoveAllAutoUpdateDir(subDir);
                Directory.Delete(subDir);
            }
        }

        private static void RemoveAllOlderFile(string dir)
        {
            foreach (string file in Directory.GetFiles(dir, $"*{UpdateExtension}"))
            {
                File.Delete(file);
            }

            foreach (string subDir in Directory.GetDirectories(dir))
            {
                RemoveAllOlderFile(subDir);
            }
        }

        private static bool UpdateFile(string directory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    string destFile = file.Replace($"{UpdateFolder}\\", "");
                    if (File.Exists(destFile + UpdateExtension))
                        File.Delete(destFile + UpdateExtension);
                    if (File.Exists(destFile))
                        File.Move(destFile, destFile + UpdateExtension);
                    File.Move(file, destFile);
                }
                foreach (string dir in Directory.GetDirectories(directory))
                {
                    string destDir = dir.Replace($"{UpdateFolder}\\", "");
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);
                    if (!UpdateFile(dir))
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static void Restart()
        {
            Task.Run(() =>
            {
                StringBuilder args = new();
                if (Environment.GetCommandLineArgs().Length > 1)
                    for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
                    {
                        if (args.Length > 0)
                            args.Append(" ");
                        args.Append(Environment.GetCommandLineArgs()[i]);
                    }
                Process.Start(Environment.GetCommandLineArgs()[0], args.ToString());
                Environment.Exit(0);
            });
        }
    }
}

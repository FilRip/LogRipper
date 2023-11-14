using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using LogRipper.Constants;
using LogRipper.Exceptions;
using LogRipper.Windows;

namespace LogRipper.Helpers;

internal class ExtendWebClient : WebClient
{
    private Uri _lastUri;

    public Uri LastUri
    {
        get { return _lastUri; }
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        WebResponse response = base.GetWebResponse(request);
        if (response != null)
            _lastUri = response.ResponseUri;
        return response;
    }

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
        WebResponse response = base.GetWebResponse(request, result);
        if (response != null)
            _lastUri = response.ResponseUri;
        return response;
    }
}

internal static class AutoUpdater
{
    private const string UpdateFolder = "AutoUpdate";
    private const string UpdateExtension = ".update";
    private static readonly string AutoUpdateDirectory = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), UpdateFolder);

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
    internal static StringBuilder LatestVersion(bool beta)
    {
        ExtendWebClient client = null;
        StringBuilder result = null;
        try
        {
            client = new ExtendWebClient();
            string readme = client.DownloadString(new Uri($"https://github.com/FilRip/LogRipper/releases/latest"));
            if (!string.IsNullOrWhiteSpace(readme))
                result = new StringBuilder(client.LastUri.Segments[client.LastUri.Segments.Length - 1].Replace("v", ""));
        }
        catch (Exception) { /* Ignore errors */ }
        finally
        {
            client?.Dispose();
        }
        return result;
    }
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé

    internal static void InstallNewVersion(string version)
    {
        ExtendWebClient client = null;
        try
        {
            string destFile = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "LogRipper.zip");
            if (File.Exists(destFile))
                File.Delete(destFile);
            client = new ExtendWebClient();
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
            MainDispatcher.Run(() => Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ActiveProgressRing = true);
            CleanUpUpdate();
            try
            {
                StringBuilder version = LatestVersion(beta);
                if (version != null)
                {
                    Version latestVersion = new(version.ToString());
                    if (Assembly.GetEntryAssembly().GetName().Version.CompareTo(latestVersion) < 0 &&
                        WpfMessageBox.ShowModal(string.Format(Locale.LBL_NEW_VERSION, latestVersion.ToString()), "LogRipper", MessageBoxButton.YesNo))
                    {
                        InstallNewVersion(version.ToString());
                    }
                }
            }
            catch (Exception) { /* Ignore errors */ }
            finally
            {
                MainDispatcher.Run(() => Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ActiveProgressRing = false);
            }
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

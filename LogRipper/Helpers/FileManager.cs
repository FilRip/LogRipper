using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

using LogRipper.Constants;
using LogRipper.Exceptions;
using LogRipper.Models;
using LogRipper.Windows;

namespace LogRipper.Helpers;

internal static class FileManager
{
    private static readonly Dictionary<string, OneFile> _listFiles = [];

    internal static OneFile GetFile(string filename)
    {
        if (filename != null && _listFiles.ContainsKey(filename))
            return _listFiles[filename];
        return null;
    }

    internal static List<OneFile> GetAllFiles()
    {
        return [.. _listFiles.Values.OfType<OneFile>().Where(f => !f.IsDisposed)];
    }

    internal static void RemoveAllFiles()
    {
        _listFiles.Values.OfType<OneFile>().ToList().ForEach(f => f.Dispose());
        _listFiles.Clear();
    }

    internal static void AddFile(OneFile file)
    {
        if (_listFiles.TryGetValue(file.FileName, out _))
            _listFiles.Remove(file.FileName);
        _listFiles.Add(file.FileName, file);
    }

    internal static void SetNewListFiles(List<OneFile> listFiles)
    {
        RemoveAllFiles();
        foreach (OneFile file in listFiles)
        {
            _listFiles.Add(file.FullPath, file);
            file.CommonInit();
        }
        Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshListFiles();
    }

    internal static List<OneLine> LoadFile(string filename, out OneFile file, Encoding defaultFileEncoding = null, SolidColorBrush defaultBackgfround = null, SolidColorBrush defaultForeground = null, bool activeAutoReload = false, bool createFile = true)
    {
        file = null;
        if (createFile && _listFiles.Values.OfType<OneFile>().Any(f => f.FullPath == filename))
            throw new LogRipperException(Locale.ERROR_FILE_ALREADY_LOADED);
        List<OneLine> list = [];
        int num = 0;
        defaultFileEncoding ??= Encoding.Default;
        defaultBackgfround ??= new SolidColorBrush(Constants.Colors.DefaultBackgroundColor);
        defaultForeground ??= new SolidColorBrush(Constants.Colors.DefaultForegroundColor);
        int length = (int)new FileInfo(filename).Length;
        if (length <= 0)
        {
            WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, filename), Locale.TITLE_ERROR);
            return list;
        }
        FileStream fs = null;
        string[] lines = null;
        try
        {
            byte[] donnees = new byte[length];
            fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            length = fs.Read(donnees, 0, length);
            if (donnees.Length > 0)
                lines = defaultFileEncoding.GetString(donnees).Split(Environment.NewLine.ToCharArray());
        }
        catch (Exception ex)
        {
            WpfMessageBox.ShowModal(Locale.ERROR_READING_FILE + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
            return list;
        }
        finally
        {
            fs?.Close();
            fs?.Dispose();
        }
        if (lines?.Length > 0)
        {
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    num++;
                    OneLine oneLine = new(filename)
                    {
                        NumLine = num,
                        Line = line,
                    };
                    list.Add(oneLine);
                }
            }
            if (createFile)
            {
                file = new OneFile(filename, length, defaultFileEncoding, activeAutoReload, false)
                {
                    DefaultBackground = defaultBackgfround,
                    DefaultForeground = defaultForeground,
                };
                _listFiles.Add(filename, file);
            }
        }
        else
            WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, filename), Locale.TITLE_ERROR);
        return list;
    }

    internal static void ComputeDate(IEnumerable<OneLine> listLines, string dateFormat)
    {
        DateTime lastDt = DateTime.MinValue;
        foreach (OneLine oneLine in listLines)
        {
            if (!string.IsNullOrWhiteSpace(oneLine.Line) && oneLine.Line.Length >= dateFormat.Length &&
                DateTime.TryParseExact(oneLine.Line.Substring(0, dateFormat.Length), dateFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime currentDate))
            {
                lastDt = currentDate;
            }
            oneLine.Date = lastDt;
        }
    }

    internal static bool CheckDateFormat(string formatDate, string line)
    {
        if (string.IsNullOrWhiteSpace(formatDate) || string.IsNullOrWhiteSpace(line))
            return true;
        bool ret = TryParseDate(formatDate, line);
        if (!ret)
            return WpfMessageBox.ShowModal(Locale.INVALID_DATE_FORMAT, Locale.BTN_EDIT_RULE.Replace("_", ""), MessageBoxButton.YesNo);
        return true;
    }

    internal static bool TryParseDate(string formatDate, string line)
    {
        if (string.IsNullOrWhiteSpace(formatDate))
            return true;
        return DateTime.TryParseExact(line.Substring(0, formatDate.Length), formatDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out _);
    }

    internal static void ExportToHtml(ObservableCollection<OneLine> listLines, ListCurrentRules listRules, string filepath)
    {
        StringBuilder sb = new();

        sb.AppendLine("<html>");
        sb.AppendLine("<body>");
        sb.AppendLine("<font face=\"Consolas\">");
        sb.AppendLine("<table>");

        foreach (OneLine line in listLines.Where(line => !string.IsNullOrWhiteSpace(line.Line) && !listRules.ExecuteRulesHideLine(line.Line, line.Date)))
        {
            sb.AppendLine("<tr>");

            sb.Append($"<td bgcolor=\"#{_listFiles[line.FilePath].DefaultBackground.Color.R:X2}{_listFiles[line.FilePath].DefaultBackground.Color.G:X2}{_listFiles[line.FilePath].DefaultBackground.Color.B:X2}\">");
            sb.Append($"<font color=\"#{_listFiles[line.FilePath].DefaultForeground.Color.R:X2}{_listFiles[line.FilePath].DefaultForeground.Color.G:X2}{_listFiles[line.FilePath].DefaultForeground.Color.B:X2}\">{line.NumLine}</font>");
            sb.Append("</td>");

            sb.Append($"<td bgcolor=\"#{_listFiles[line.FilePath].DefaultBackground.Color.R:X2}{_listFiles[line.FilePath].DefaultBackground.Color.G:X}{_listFiles[line.FilePath].DefaultBackground.Color.B:X2}\">");
            sb.Append($"<font color=\"#{_listFiles[line.FilePath].DefaultForeground.Color.R:X2}{_listFiles[line.FilePath].DefaultForeground.Color.G:X}{_listFiles[line.FilePath].DefaultForeground.Color.B:X2}\">{line.FileName}</font>");
            sb.Append("</td>");

            SolidColorBrush back, fore;
            back = listRules.ExecuteRulesBackground(line.Line, line.Date) ?? _listFiles[line.FilePath].DefaultBackground;
            fore = listRules.ExecuteRulesForeground(line.Line, line.Date) ?? _listFiles[line.FilePath].DefaultForeground;
            sb.Append($"<td bgcolor=\"#{back.Color.R:X2}{back.Color.G:X2}{back.Color.B:X2}\">");
            sb.Append($"<font color=\"#{fore.Color.R:X2}{fore.Color.G:X2}{fore.Color.B:X2}\">{line.Line}</font>");
            sb.Append("</td>");
        }
        sb.AppendLine("</table>");
        sb.AppendLine("</font>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        File.WriteAllText(filepath, sb.ToString());
    }
}

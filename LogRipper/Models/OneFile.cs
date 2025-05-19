using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.ViewModels;
using LogRipper.Windows;

namespace LogRipper.Models;

[XmlRoot()]
public partial class OneFile : ObservableObject, IDisposable
{
    private FileSystemWatcher _watcher;
    [ObservableProperty()]
    [property: XmlElement()]
    private bool _active;
    private bool disposedValue;
    private readonly object _lockFileAccess = new();
    private Encoding _currentEncoding;
    private readonly Timer _forceRefresh;
    private readonly bool _isConsole;

    // For XmlSerialization
    public OneFile() { }

    internal OneFile(string filepath, int offset, Encoding currentEncoding, bool activeAutoReload, bool console) : this()
    {
        _isConsole = console;
        _active = true;
        AutoReload = activeAutoReload;
        FullPath = filepath;
        if (_isConsole)
            FileName = filepath;
        else
            FileName = Path.GetFileNameWithoutExtension(filepath);
        LastOffset = offset;
        _currentEncoding = currentEncoding;
        SetEncodingName();
        CommonInit();
        _forceRefresh = new Timer(10)
        {
            AutoReset = true,
            Enabled = activeAutoReload,
        };
        _forceRefresh.Elapsed += ForceRefresh_Elapsed;
    }

    private void ForceRefresh_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (AutoReload && !disposedValue)
        {
            lock (_lockFileAccess)
            {
                if (_isConsole)
                {
                    short newY = ConsoleHelper.GetCursorPosition().Y;
                    if (newY > LastOffset)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            int lastNumLine = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.Max(l => l.NumLine);
                            for (short y = (short)(LastOffset + 1); y < newY; y++)
                            {
                                OneLine line = new(FullPath)
                                {
                                    NumLine = lastNumLine++,
                                    Line = ConsoleHelper.GetText(0, y),
                                };
                                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.AddLine(line);
                            }
                            LastOffset = newY;
                            Application.Current.GetCurrentWindow<MainWindow>().ScrollToEnd();
                            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
                        }, System.Windows.Threading.DispatcherPriority.DataBind);
                    }
                }
                else if (new FileInfo(FullPath).Length > LastOffset)
                    Watcher_Changed(null, null);
            }
        }
    }

    private void SetEncodingName()
    {
        EncoderName = _currentEncoding switch
        {
            ASCIIEncoding => "ASCII",
            UTF7Encoding => "UTF7",
            UTF8Encoding => "UTF8",
            UTF32Encoding => "UTF32",
            UnicodeEncoding => "Unicode",
            _ => "Default",
        };
    }

    internal void CommonInit()
    {
        _currentEncoding ??= EncoderName switch
        {
            "ASCII" => Encoding.ASCII,
            "UTF7" => Encoding.UTF7,
            "UTF8" => Encoding.UTF8,
            "UTF32" => Encoding.UTF32,
            "Unicode" => Encoding.Unicode,
            _ => Encoding.Default,
        };
        if (!_isConsole)
            CreateFileWatcher();
    }

    internal void CreateFileWatcher()
    {
        _watcher = new FileSystemWatcher(Path.GetDirectoryName(FullPath), Path.GetFileName(FullPath))
        {
            EnableRaisingEvents = true,
        };
        _watcher.Changed += Watcher_Changed;
        _watcher.Error += Watcher_Error;
        _watcher.Deleted += Watcher_Deleted;
        _watcher.Renamed += Watcher_Renamed;
    }

    internal void ActiveAutoReload()
    {
        AutoReload = !AutoReload;
        _forceRefresh.Enabled = AutoReload;
        if (!_isConsole && AutoReload && new FileInfo(FullPath).Length != LastOffset)
            Watcher_Changed(null, null);
    }

    #region Events File Watcher

    private void Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (OneLine line in Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.Where(line => line.FilePath == FullPath))
            {
                line.FilePath = e.FullPath;
                line.FileName = e.Name;
            }
        }, System.Windows.Threading.DispatcherPriority.DataBind);
        FileName = e.Name;
        FullPath = e.FullPath;
    }

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        if (WpfMessageBox.ShowModalReturnButton(string.Format(Locale.ASK_DELETE_LINES, FileName), Locale.TITLE_DELETE, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            Application.Current.Dispatcher.Invoke(RemoveLines);
        }
        else
            _watcher.Deleted -= Watcher_Deleted;
    }

    private void Watcher_Error(object sender, ErrorEventArgs e)
    {
        AutoReload = false;
        _watcher.Deleted -= Watcher_Deleted;
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (AutoReload && !disposedValue)
        {
            lock (_lockFileAccess)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    int newLength = (int)new FileInfo(FullPath).Length;
                    FileStream fs = null;
                    try
                    {
                        fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                        fs.Position = LastOffset;
                        // TODO : If file smaller than before
                        if (newLength <= LastOffset)
                            return;
                        byte[] donnees = new byte[newLength - LastOffset];
                        int nbRead = fs.Read(donnees, 0, newLength - LastOffset);
                        LastOffset += nbRead;
                        string[] newString = _currentEncoding.GetString(donnees).Split((char)13, (char)10);
                        int lastNumLine = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.Max(l => l.NumLine);
                        foreach (string text in newString)
                        {
                            if (!string.IsNullOrEmpty(text))
                            {
                                lastNumLine++;
                                OneLine line = new(FullPath)
                                {
                                    NumLine = lastNumLine,
                                    Line = text,
                                };
                                if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(DateFormat) && text.Length >= DateFormat.Length &&
                                    DateTime.TryParseExact(text.Substring(0, DateFormat.Length), DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime currentDate))
                                {
                                    line.Date = currentDate;
                                }
                                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.AddLine(line);
                            }
                        }

                        Application.Current.GetCurrentWindow<MainWindow>().ScrollToEnd();
                        Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
                    }
                    catch (Exception) { /* Do nothing for this time */ }
                    finally
                    {
                        fs?.Close();
                        fs?.Dispose();
                    }
                }, System.Windows.Threading.DispatcherPriority.DataBind);
            }
        }
    }

    #endregion

    #region Properties

    [XmlElement()]
    public int LastOffset { get; set; }

    [XmlElement()]
    public string FileName { get; set; }

    [XmlElement()]
    public string FullPath { get; set; }

    [XmlIgnore()]
    public SolidColorBrush DefaultBackground { get; set; }

    [XmlIgnore()]
    public SolidColorBrush DefaultForeground { get; set; }

    [XmlElement()]
    public bool AutoReload { get; set; }

    [XmlElement()]
    public string DateFormat { get; set; }

    async partial void OnActiveChanged(bool value)
    {
        await Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshListLines();
        Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
    }

    [XmlElement()]
    public Color DefaultBackgroundColor
    {
        get { return DefaultBackground.Color; }
        set { DefaultBackground = new SolidColorBrush(value); }
    }

    [XmlElement()]
    public Color DefaultForegroundColor
    {
        get { return DefaultForeground.Color; }
        set { DefaultForeground = new SolidColorBrush(value); }
    }

    #endregion

    #region Methods

    [RelayCommand()]
    internal async Task ReloadFile()
    {
        MainWindowViewModel mainWindowViewModel = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
        mainWindowViewModel.ActiveProgressRing = true;
        await Task.Delay(10);
        if (_isConsole)
        {
            await mainWindowViewModel.ReadFromConsole(FullPath);
        }
        else
        {
            List<OneLine> listLines = [.. mainWindowViewModel.ListLines];
            listLines.RemoveAll(l => l.FilePath == FullPath);
            List<OneLine> listNewLines;
            lock (_lockFileAccess)
            {
                listNewLines = FileManager.LoadFile(FullPath, out _, _currentEncoding, DefaultBackground, DefaultForeground, createFile: false);
                LastOffset = (int)new FileInfo(FullPath).Length;
            }
            if (!string.IsNullOrWhiteSpace(DateFormat))
                FileManager.ComputeDate(listNewLines, DateFormat);
            mainWindowViewModel.ListLines = [.. listLines.Concat(listNewLines).OrderBy(l => l.Date)];
            await mainWindowViewModel.RefreshListLines();
        }
        mainWindowViewModel.ActiveProgressRing = false;
    }

    [RelayCommand()]
    private async Task RemoveFile()
    {
        if (!WpfMessageBox.ShowModal(Locale.CONFIRM_REMOVE_FILE + Environment.NewLine + FileName, Locale.TITLE_CONFIRM_DEL, MessageBoxButton.YesNo))
            return;
        lock (_lockFileAccess)
        {
            Dispose();
            Active = false;
        }
        await RemoveLines();
    }

    private async Task RemoveLines()
    {
        MainWindowViewModel mainWindowViewModel = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
        List<OneLine> listLines = [.. mainWindowViewModel.ListLines];
        listLines.RemoveAll(l => l.FilePath == FullPath);
        mainWindowViewModel.ListLines = [.. listLines];
        mainWindowViewModel.RefreshListFiles();
        await mainWindowViewModel.RefreshListLines();
    }

    [XmlElement()]
    public string EncoderName { get; set; }

    [RelayCommand()]
    internal async Task<bool> EditFile()
    {
        FileWindow win = new()
        {
            Title = Locale.BTN_EDIT_RULE.Replace("_", "") + " " + FileName,
        };
        win.MyDataContext.FormatDate = DateFormat;
        if (string.IsNullOrWhiteSpace(DateFormat))
            win.MyDataContext.FormatDate = Properties.Settings.Default.DefaultDateFormat;
        win.MyDataContext.ForeColor = DefaultForegroundColor;
        win.MyDataContext.BackColor = DefaultBackgroundColor;
        if (_currentEncoding == Encoding.Default)
            win.MyDataContext.CurrentEncoder = Locale.MENU_ENC_DEFAULT.Replace("_", "");
        else if (_currentEncoding == Encoding.ASCII)
            win.MyDataContext.CurrentEncoder = Locale.MENU_ENC_ASCII.Replace("_", "");
        else if (_currentEncoding == Encoding.UTF7)
            win.MyDataContext.CurrentEncoder = Locale.MENU_ENC_UTF7.Replace("_", "");
        else if (_currentEncoding == Encoding.UTF8)
            win.MyDataContext.CurrentEncoder = Locale.MENU_ENC_UTF8.Replace("_", "");
        else if (_currentEncoding == Encoding.UTF32)
            win.MyDataContext.CurrentEncoder = Locale.MENU_ENC_UTF32.Replace("_", "");
        else if (_currentEncoding == Encoding.Unicode)
            win.MyDataContext.CurrentEncoder = Locale.MENU_ENC_UNICODE.Replace("_", "");
        SetEncodingName();
        string oldDateFormat = DateFormat;
        Encoding previousEncoding = _currentEncoding;
        win.MyDataContext.FirstLine = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.FirstOrDefault(l => l.FilePath == FullPath && !string.IsNullOrWhiteSpace(l.Line))?.Line;
        if (win.ShowDialog() == true)
        {
            DateFormat = win.MyDataContext.FormatDate;
            DefaultForegroundColor = win.MyDataContext.ForeColor;
            DefaultBackgroundColor = win.MyDataContext.BackColor;
            if (win.MyDataContext.CurrentEncoder == Locale.MENU_ENC_DEFAULT.Replace("_", ""))
                _currentEncoding = Encoding.Default;
            else if (win.MyDataContext.CurrentEncoder == Locale.MENU_ENC_ASCII.Replace("_", ""))
                _currentEncoding = Encoding.ASCII;
            else if (win.MyDataContext.CurrentEncoder == Locale.MENU_ENC_UTF7.Replace("_", ""))
                _currentEncoding = Encoding.UTF7;
            else if (win.MyDataContext.CurrentEncoder == Locale.MENU_ENC_UTF8.Replace("_", ""))
                _currentEncoding = Encoding.UTF8;
            else if (win.MyDataContext.CurrentEncoder == Locale.MENU_ENC_UTF32.Replace("_", ""))
                _currentEncoding = Encoding.UTF32;
            else if (win.MyDataContext.CurrentEncoder == Locale.MENU_ENC_UNICODE.Replace("_", ""))
                _currentEncoding = Encoding.Unicode;
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ActiveProgressRing = true;
            if (previousEncoding != _currentEncoding)
                await ReloadFile();
            if (!string.IsNullOrWhiteSpace(DateFormat) && oldDateFormat != DateFormat)
            {
                ObservableCollection<OneLine> lines = [.. Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.Where(line => line.FilePath == FullPath)];
                FileManager.ComputeDate(lines, DateFormat);
            }
            MainWindow mainWindow = Application.Current.GetCurrentWindow<MainWindow>();
            mainWindow.RefreshMargin();
            mainWindow.MyDataContext.RefreshListFiles();
            await mainWindow.MyDataContext.RefreshListLines();
            mainWindow.MyDataContext.RefreshVisibleLines();
            mainWindow.MyDataContext.ActiveProgressRing = false;
            return true;
        }
        return false;
    }

    #endregion

    #region IDisposable

    internal bool IsDisposed
    {
        get { return disposedValue; }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_watcher != null)
                {
                    _watcher.Changed -= Watcher_Changed;
                    _watcher.Error -= Watcher_Error;
                    _watcher.Deleted -= Watcher_Deleted;
                    _watcher.Renamed -= Watcher_Renamed;
                }
                _watcher?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}

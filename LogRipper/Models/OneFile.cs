using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.ViewModels;
using LogRipper.Windows;

namespace LogRipper.Models
{
    [XmlRoot()]
    public class OneFile : ViewModelBase, IDisposable
    {
        private FileSystemWatcher _watcher;
        private bool disposedValue, _active;
        private int _lastOffset;
        private readonly object _lockFileAccess;
        private Encoding _currentEncoding;
        private readonly ICommand _cmdReloadFile, _cmdRemoveFile, _cmdEditFile;

        public OneFile() { }

        internal OneFile(string filepath, int offset, Encoding currentEncoding, bool activeAutoReload) : this()
        {
            _active = true;
            AutoReload = activeAutoReload;
            FullPath = filepath;
            FileName = Path.GetFileNameWithoutExtension(filepath);
            _currentEncoding = currentEncoding;
            _lockFileAccess = new object();
            _lastOffset = offset;
            _cmdReloadFile = new RelayCommand((param) => ReloadFile());
            _cmdRemoveFile = new RelayCommand((param) => RemoveFile());
            _cmdEditFile = new RelayCommand((param) => EditFile());
            CreateFileWatcher();
        }

        internal void CreateFileWatcher()
        {
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(FullPath), Path.GetFileName(FullPath))
            {
                EnableRaisingEvents = true,
            };
            _watcher.Changed += Watcher_Changed;
            _watcher.Created += Watcher_Created;
            _watcher.Error += Watcher_Error;
            _watcher.Deleted += Watcher_Deleted;
            _watcher.Renamed += Watcher_Renamed;
        }

        internal void ActiveAutoReload()
        {
            AutoReload = !AutoReload;
            if (AutoReload && new FileInfo(FullPath).Length != _lastOffset)
                Watcher_Changed(null, null);
        }

        #region Events File Watcher

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            // TODO : Change filename
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            // TODO : Remove line from this file
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            // TODO : Ask to remove line of this file
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // TODO : Restart OneLine to zero
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (AutoReload && !disposedValue)
            {
                lock (_lockFileAccess)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        int newLength = (int)new FileInfo(FullPath).Length;
                        FileStream fs = null;
                        try
                        {
                            fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                            fs.Position = _lastOffset;
                            // TODO : If file smaller than before
                            if (newLength < _lastOffset)
                                return;
                            byte[] donnees = new byte[newLength - _lastOffset];
                            int nbRead = fs.Read(donnees, 0, newLength - _lastOffset);
                            _lastOffset += nbRead;
                            string[] newString = _currentEncoding.GetString(donnees).Split((char)13, (char)10);
                            int lastNumLine = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.Max(l => l.NumLine);
                            foreach (string text in newString)
                            {
                                if (!string.IsNullOrEmpty(text))
                                {
                                    lastNumLine++;
                                    OneLine line = new()
                                    {
                                        NumLine = lastNumLine,
                                        Line = text,
                                        FileName = FileName,
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
                    }), System.Windows.Threading.DispatcherPriority.DataBind);
                }
            }
        }

        #endregion

        #region Properties

        [XmlIgnore()]
        public ICommand CmdReloadFile
        {
            get { return _cmdReloadFile; }
        }

        [XmlIgnore()]
        public ICommand CmdRemoveFile
        {
            get { return _cmdRemoveFile; }
        }

        [XmlIgnore()]
        public ICommand CmdEditFile
        {
            get { return _cmdEditFile; }
        }

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

        [XmlElement()]
        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnPropertyChanged();
                    Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshListLines();
                    Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                }
            }
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

        internal void ReloadFile()
        {
            lock (_lockFileAccess)
            {
                MainWindowViewModel mainWindowViewModel = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
                List<OneLine> listLines = mainWindowViewModel.ListLines.ToList();
                listLines.RemoveAll(l => l.FileName == FileName);
                ObservableCollection<OneLine> listNewLines = FileManager.LoadFile(FullPath, _currentEncoding, DefaultBackground, DefaultForeground, createFile: false);
                _lastOffset = (int)new FileInfo(FullPath).Length;
                if (!string.IsNullOrWhiteSpace(DateFormat))
                    FileManager.ComputeDate(listNewLines, DateFormat);
                mainWindowViewModel.ListLines = new ObservableCollection<OneLine>(listLines.Concat(listNewLines).OrderBy(l => l.Date));
                mainWindowViewModel.RefreshListLines();
            }
        }

        private void RemoveFile()
        {
            if (!WpfMessageBox.ShowModal(Locale.CONFIRM_REMOVE_FILE + Environment.NewLine + FileName, Locale.TITLE_CONFIRM_DEL, MessageBoxButton.YesNo))
                return;
            lock (_lockFileAccess)
            {
                Dispose();
                Active = false;
                MainWindowViewModel mainWindowViewModel = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
                List<OneLine> listLines = mainWindowViewModel.ListLines.ToList();
                listLines.RemoveAll(l => l.FileName == FileName);
                mainWindowViewModel.ListLines = new ObservableCollection<OneLine>(listLines);
                mainWindowViewModel.RefreshListFiles();
                mainWindowViewModel.RefreshListLines();
            }
        }

        private void EditFile()
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
            string oldDateFormat = DateFormat;
            if (win.ShowDialog() == true)
            {
                DateFormat = win.MyDataContext.FormatDate;
                DefaultForegroundColor = win.MyDataContext.ForeColor;
                DefaultBackgroundColor = win.MyDataContext.BackColor;
                Encoding previousEncoding = _currentEncoding;
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
                    ReloadFile();
                ObservableCollection<OneLine> lines = new(Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListLines.Where(line => line.FileName == FileName));
                if (!string.IsNullOrWhiteSpace(DateFormat) && oldDateFormat != DateFormat)
                    FileManager.ComputeDate(lines, DateFormat);
                Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshListFiles();
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshListLines();
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ActiveProgressRing = false;
            }
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
}

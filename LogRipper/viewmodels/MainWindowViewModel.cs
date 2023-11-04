using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Exceptions;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.Windows;

using Microsoft.Win32;

namespace LogRipper.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<OneLine> _listLines;
        private IEnumerable<OneLine> _selectedItems;
        private OneLine _currentLine;
        private ListCurrentRules _listRules;
        private readonly ICommand _addRule, _listCurrentRules, _saveRules, _loadRules, _cmdShowDateFilter, _cmdOpenFile, _cmdReloadAllFiles, _cmdMergeRules;
        private readonly ICommand _cmdEncodageDefault, _cmdEncodageAscii, _cmdEncodageUtf7, _cmdEncodageUtf8, _cmdEncodageUtf32, _cmdEncodageUnicode;
        private readonly ICommand _showNumLine, _showFileName, _cmdShowSearchResult, _cmdExportToHtml, _cmdExit, _cmdCopySelectedItems, _cmdCheckUpdate;
        private readonly ICommand _cmdSearch, _cmdGotoLine, _cmdMergeFile, _cmdAutoReload, _cmdAutoScrollToEnd, _cmdAutoFollowInMargin;
        private readonly ICommand _cmdOptions, _cmdShowMargin, _cmdShowToolbar, _cmdSaveState, _cmdLoadState, _cmdShowAbout, _cmdSaveSearchResult;
        private readonly ICommand _cmdHideLineBefore, _cmdHideLineAfter;
        private bool _encodageDefault, _encodageAscii, _encodageUtf7, _encodageUtf8, _encodageUtf32, _encodageUnicode, _showFilterByDate, _autoFollowInMargin;
        private bool _currentShowNumLine, _currentShowFileName, _showSearchResult, _enableAutoReload, _autoScrollToEnd, _showMargin, _showToolbar, _dateFilter;
        private bool _hideAllOthersLines, _activeProgressRing;
        private string _search;
        private StringComparison _searchCaseSensitive;
        private int _lastLineNumber;
        private readonly List<OneRule> _listSearchRules;
        private ECurrentSearchMode _currentSearchMode;
        private int _numVisibleStart;
        private int _numVisibleEnd;
        private DateTime? _startDate, _endDate, _minDate, _maxDate;
        private Thickness _margin;
        private string _selectedText;

        private ICollectionView _filterListLines;
        private ObservableCollection<OneFile> _listFiles;

        #endregion

        #region Constructors

        public MainWindowViewModel() : base()
        {
            _listRules = new ListCurrentRules();
            _listSearchRules = new List<OneRule>();
            ListCategory = new List<OneCategory>();
            _listRules.AddRuleEvent += UpdateRules;
            _listRules.RemoveRuleEvent += UpdateRules;
            _listRules.EditRuleEvent += UpdateRules;
            _addRule = new RelayCommand((param) => AddRule());
            _listCurrentRules = new RelayCommand((param) => OpenListRules());
            _saveRules = new RelayCommand((param) => SaveRules());
            _loadRules = new RelayCommand((param) => LoadRules());

            _cmdEncodageDefault = new RelayCommand((param) => ChangeEncodageToDefault());
            _cmdEncodageAscii = new RelayCommand((param) => ChangeEncodageToAscii());
            _cmdEncodageUtf7 = new RelayCommand((param) => ChangeEncodageToUtf7());
            _cmdEncodageUtf8 = new RelayCommand((param) => ChangeEncodageToUtf8());
            _cmdEncodageUtf32 = new RelayCommand((param) => ChangeEncodageToUtf32());
            _cmdEncodageUnicode = new RelayCommand((param) => ChangeEncodageToUnicode());
            _cmdAutoReload = new RelayCommand((param) => AutoReload());
            _showNumLine = new RelayCommand((param) => ToggleShowNumLine());
            _showFileName = new RelayCommand((param) => ToggleShowFileName());
            _cmdShowSearchResult = new RelayCommand((param) => ToggleShowSearchResult());
            _cmdSearch = new RelayCommand((param) => Search());
            _cmdGotoLine = new RelayCommand((param) => GotoLine());
            _cmdAutoScrollToEnd = new RelayCommand((param) => ToggleAutoScrollToEnd());
            _cmdOptions = new RelayCommand((param) => OpenSettings());
            _cmdMergeFile = new RelayCommand((param) => MergeFile());
            _cmdShowMargin = new RelayCommand((param) => ShowMargin());
            _cmdShowToolbar = new RelayCommand((param) => ShowToolbar());
            _cmdSaveState = new RelayCommand((param) => SaveState());
            _cmdLoadState = new RelayCommand((param) => LoadState());
            _cmdShowAbout = new RelayCommand((param) => ShowAbout());
            _cmdShowDateFilter = new RelayCommand((param) => ShowDateFilter());
            _cmdOpenFile = new RelayCommand((param) => OpenFile());
            _cmdExportToHtml = new RelayCommand((param) => ExportToHtml());
            _cmdExit = new RelayCommand((param) => Exit());
            _cmdCopySelectedItems = new RelayCommand((param) => CopySelectedItems());
            _cmdAutoFollowInMargin = new RelayCommand((param) => AutoFollowMargin());
            _cmdReloadAllFiles = new RelayCommand((param) => ReloadAllFiles());
            _cmdSaveSearchResult = new RelayCommand((param) => SaveSearchResult());
            _cmdMergeRules = new RelayCommand((param) => MergeRules());
            _cmdCheckUpdate = new RelayCommand((param) => CheckForUpdate());
            _cmdHideLineBefore = new RelayCommand((param) => HideLineBefore());
            _cmdHideLineAfter = new RelayCommand((param) => HideLineAfter());

            ResetMinMaxDate();
            StartDateTime = DateTime.MinValue;
            EndDateTime = DateTime.MaxValue;

            ToggleShowNumLine();
            ToggleShowFileName();

            ChangeEncodageToDefault();
            SetMargin();

            EnableShowMargin = Properties.Settings.Default.ShowMargin;
            EnableShowToolbar = Properties.Settings.Default.ShowToolBar;
            EnableShowDateFilter = Properties.Settings.Default.ShowDateFilter;
            AutoFollowInMargin = Properties.Settings.Default.AutoFollowMargin;
        }

        #endregion

        #region Properties

        public ICommand CmdHideLineBefore
        {
            get { return _cmdHideLineBefore; }
        }

        public ICommand CmdHideLineAfter
        {
            get { return _cmdHideLineAfter; }
        }

        public ICommand CmdCheckUpdate
        {
            get { return _cmdCheckUpdate; }
        }

        public ICommand CmdMergeRules
        {
            get { return _cmdMergeRules; }
        }

        public ICommand CmdSaveSearchResult
        {
            get { return _cmdSaveSearchResult; }
        }

        public ICommand CmdReloadAllFiles
        {
            get { return _cmdReloadAllFiles; }
        }

        public bool AutoFollowInMargin
        {
            get { return _autoFollowInMargin; }
            set
            {
                if (_autoFollowInMargin != value)
                {
                    _autoFollowInMargin = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ActiveProgressRing
        {
            get { return _activeProgressRing; }
            set
            {
                if (_activeProgressRing != value)
                {
                    _activeProgressRing = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HideAllOthersLines
        {
            get { return _hideAllOthersLines; }
            set
            {
                if (_hideAllOthersLines != value)
                {
                    _hideAllOthersLines = value;
                    RefreshListLines();
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdAutoFollowInMargin
        {
            get { return _cmdAutoFollowInMargin; }
        }

        public IEnumerable<OneLine> SelectedLines
        {
            get { return _selectedItems; }
            set
            {
                if (_selectedItems != value)
                {
                    _selectedItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdShowDateFilter
        {
            get { return _cmdShowDateFilter; }
        }

        public bool EnableShowDateFilter
        {
            get { return _showFilterByDate; }
            set
            {
                if (_showFilterByDate != value)
                {
                    _showFilterByDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? StartDateTime
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                    RefreshListLines();
                }
            }
        }

        public DateTime? EndDateTime
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                    RefreshListLines();
                }
            }
        }

        public DateTime? MinDate
        {
            get { return _minDate; }
        }

        public DateTime? MaxDate
        {
            get { return _maxDate; }
        }

        private void ResetMinMaxDate()
        {
            _minDate = DateTime.MinValue;
            _maxDate = DateTime.MaxValue;
            OnPropertyChanged(nameof(MinDate));
            OnPropertyChanged(nameof(MaxDate));
        }

        public bool FilterByDate
        {
            get { return _dateFilter; }
            set
            {
                if (_dateFilter != value)
                {
                    if (_listFiles?.Count > 0 && string.IsNullOrWhiteSpace(_listFiles[0].DateFormat))
                    {
                        WpfMessageBox.ShowModal(Locale.NO_DATEFORMAT_IN_FILE, Locale.TITLE_ERROR);
                        if (!_listFiles[0].EditFile())
                            return;
                    }
                    _dateFilter = value;
                    OnPropertyChanged();
                    try
                    {
                        _startDate = _listLines.Where(line => line.Date != DateTime.MinValue).Min(line => line.Date);
                        _endDate = _listLines.Where(line => line.Date != DateTime.MinValue).Max(line => line.Date);
                    }
                    catch (Exception)
                    {
                        _startDate = DateTime.MinValue;
                        _endDate = DateTime.MaxValue;
                    }
                    ResetMinMaxDate();
                    OnPropertyChanged(nameof(StartDateTime));
                    OnPropertyChanged(nameof(EndDateTime));
                    _minDate = _startDate;
                    _maxDate = _endDate;
                    OnPropertyChanged(nameof(MinDate));
                    OnPropertyChanged(nameof(MaxDate));
                    RefreshListLines();
                }
            }
        }

        public List<OneCategory> ListCategory { get; set; }

        public void UpdateCategory(string category = null)
        {
            if (!string.IsNullOrWhiteSpace(category) && !ListCategory.Exists(c => c.Category.IndexOf(category, 0, StringComparison.CurrentCultureIgnoreCase) >= 0))
                ListCategory.Add(new OneCategory(category));
            if (ListRules.ListRules?.Count > 0)
            {
                for (int i = ListCategory.Count - 1; i >= 0; i--)
                    if (!ListRules.ListRules.Any(r => !string.IsNullOrWhiteSpace(r.Category) && r.Category.IndexOf(ListCategory[i].Category, 0, StringComparison.CurrentCultureIgnoreCase) >= 0))
                        ListCategory.RemoveAt(i);

                foreach (string cat in ListRules.ListRules.Where(r => !string.IsNullOrWhiteSpace(r.Category)).Select(r => r.Category).Distinct())
                    if (!ListCategory.Exists(r => r.Category.IndexOf(cat, 0, StringComparison.CurrentCultureIgnoreCase) >= 0))
                        ListCategory.Add(new OneCategory(cat));
            }
            OnPropertyChanged(nameof(ListCategory));
        }

        public bool EnableShowToolbar
        {
            get { return _showToolbar; }
            set
            {
                if (_showToolbar != value)
                {
                    _showToolbar = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdShowToolbar
        {
            get { return _cmdShowToolbar; }
        }

        public ICommand CmdShowAbout
        {
            get { return _cmdShowAbout; }
        }

        public bool EnableShowMargin
        {
            get { return _showMargin; }
            set
            {
                if (_showMargin != value)
                {
                    _showMargin = value;
                    OnPropertyChanged();
                    Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                }
            }
        }

        internal void SetMargin()
        {
            _margin = new Thickness(Values.FontSize, 0, 0, 0);
        }

        public Thickness Margin
        {
            get { return _margin; }
        }

        public ICommand CmdShowMargin
        {
            get { return _cmdShowMargin; }
        }

        public bool EnableAutoScrollToEnd
        {
            get { return _autoScrollToEnd; }
            set
            {
                if (_autoScrollToEnd != value)
                {
                    _autoScrollToEnd = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EnableAutoReload
        {
            get { return _enableAutoReload; }
            set
            {
                if (_enableAutoReload != value)
                {
                    _enableAutoReload = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowSearchResult
        {
            get { return _showSearchResult; }
            set
            {
                if (_showSearchResult != value)
                {
                    _showSearchResult = value;
                    OnPropertyChanged();
                    Application.Current.GetCurrentWindow<MainWindow>().Window_SizeChanged(null, null);
                }
            }
        }

        public int RowIndexSelected { get; set; }

        public OneLine SelectedLine
        {
            get { return _currentLine; }
            set
            {
                if (_currentLine != value)
                {
                    _currentLine = value;
                    OnPropertyChanged();
                    ((MainWindow)Application.Current.MainWindow).ScrollToSelected();
                }
            }
        }

        public ICollectionView FilteredListLines
        {
            get { return _filterListLines; }
        }

        public ObservableCollection<OneLine> ListLines
        {
            get
            {
                return _listLines;
            }
            set
            {
                if (_listLines != value)
                {
                    _listLines = value;
                    OnPropertyChanged();
                    SetFilteredListLinesView();
                }
            }
        }

        public ListCurrentRules ListRules
        {
            get { return _listRules; }
            set
            {
                if (_listRules != value)
                {
                    _listRules = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdSaveState
        {
            get { return _cmdSaveState; }
        }

        public ICommand CmdLoadState
        {
            get { return _cmdLoadState; }
        }

        public ICommand CmdAddRule
        {
            get { return _addRule; }
        }

        public ICommand CmdListRules
        {
            get { return _listCurrentRules; }
        }

        public ICommand CmdSaveRules
        {
            get { return _saveRules; }
        }

        public ICommand CmdLoadRules
        {
            get { return _loadRules; }
        }

        public ICommand SetEncodageDefault
        {
            get { return _cmdEncodageDefault; }
        }

        public ICommand SetEncodageAscii
        {
            get { return _cmdEncodageAscii; }
        }

        public ICommand SetEncodageUtf7
        {
            get { return _cmdEncodageUtf7; }
        }

        public ICommand SetEncodageUtf8
        {
            get { return _cmdEncodageUtf8; }
        }

        public ICommand SetEncodageUtf32
        {
            get { return _cmdEncodageUtf32; }
        }

        public ICommand SetEncodageUnicode
        {
            get { return _cmdEncodageUnicode; }
        }

        public ICommand CmdShowSearchResult
        {
            get { return _cmdShowSearchResult; }
        }

        public ICommand CmdOptions
        {
            get { return _cmdOptions; }
        }

        public ICommand CmdMergeFile
        {
            get { return _cmdMergeFile; }
        }

        public ICommand CmdEnableAutoReload
        {
            get { return _cmdAutoReload; }
        }

        public ICommand CmdEnableAutoScrollToEnd
        {
            get { return _cmdAutoScrollToEnd; }
        }

        public bool EncodageDefault
        {
            get { return _encodageDefault; }
            set
            {
                if (_encodageDefault != value)
                {
                    _encodageDefault = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EncodageAscii
        {
            get { return _encodageAscii; }
            set
            {
                if (_encodageAscii != value)
                {
                    _encodageAscii = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EncodageUtf7
        {
            get { return _encodageUtf7; }
            set
            {
                if (_encodageUtf7 != value)
                {
                    _encodageUtf7 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EncodageUtf8
        {
            get { return _encodageUtf8; }
            set
            {
                if (_encodageUtf8 != value)
                {
                    _encodageUtf8 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EncodageUtf32
        {
            get { return _encodageUtf32; }
            set
            {
                if (_encodageUtf32 != value)
                {
                    _encodageUtf32 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EncodageUnicode
        {
            get { return _encodageUnicode; }
            set
            {
                if (_encodageUnicode != value)
                {
                    _encodageUnicode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CurrentShowNumLine
        {
            get { return _currentShowNumLine; }
            set
            {
                if (_currentShowNumLine != value)
                {
                    _currentShowNumLine = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CurrentShowFileName
        {
            get { return _currentShowFileName; }
            set
            {
                if (_currentShowFileName != value)
                {
                    _currentShowFileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdCopySelectedItems
        {
            get { return _cmdCopySelectedItems; }
        }

        public ICommand CmdExit
        {
            get { return _cmdExit; }
        }

        public ICommand CmdExportToHtml
        {
            get { return _cmdExportToHtml; }
        }

        public ICommand CmdOpenFile
        {
            get { return _cmdOpenFile; }
        }

        public ICommand CmdShowNumLine
        {
            get { return _showNumLine; }
        }

        public ICommand CmdShowFileName
        {
            get { return _showFileName; }
        }

        public ICommand CmdSearch
        {
            get { return _cmdSearch; }
        }

        public ICommand CmdGotoLine
        {
            get { return _cmdGotoLine; }
        }

        public ECurrentSearchMode CurrentSearchMode
        {
            get { return _currentSearchMode; }
        }

        public string WhatToSearch
        {
            get
            {
                string ret = _search;
                if (_currentSearchMode == ECurrentSearchMode.BY_RULES)
                    ret = _listSearchRules[0].ToString();
                if (!string.IsNullOrWhiteSpace(Locale.LBL_RESULT))
                    return $"{ret} " + string.Format(Locale.LBL_RESULT, NbMatchSearch);
                else return "";
            }
        }

        public int NbMatchSearch
        {
            get { return ListLinesSearch?.Count() ?? 0; }
        }

        public ObservableCollection<OneFile> ListFiles
        {
            get { return _listFiles; }
            set
            {
                if (_listFiles != value)
                {
                    _listFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void CheckForUpdate()
        {
            string version = AutoUpdater.LatestVersion(Properties.Settings.Default.beta);
            if (!string.IsNullOrWhiteSpace(version))
            {
                Version latestVersion = new(version);
                string result = string.Format(Locale.LBL_VERSION_COMPARE, Assembly.GetEntryAssembly().GetName().Version.ToString(), $"{latestVersion.Major}.{latestVersion.Minor}.{latestVersion.Build}.{latestVersion.Revision}");
                WpfMessageBox.ShowModal(result, "LogRipper");
                if (Assembly.GetEntryAssembly().GetName().Version.CompareTo(latestVersion) < 0 &&
                    WpfMessageBox.ShowModal(string.Format(Locale.LBL_NEW_VERSION, latestVersion.ToString()), "LogRipper", MessageBoxButton.YesNo))
                {
                    AutoUpdater.InstallNewVersion(version);
                }
            }
            else
                WpfMessageBox.ShowModal(Locale.ERROR_CHECK_NEW_VERSION, Locale.TITLE_ERROR);
        }

        private void ShowDateFilter()
        {
            EnableShowDateFilter = !EnableShowDateFilter;
        }

        private void Search()
        {
            InputBoxWindow input = new();
            input.ShowModal(Locale.TITLE_SEARCH, Locale.LBL_SEARCH_TEXT, _search);
            if (input.DialogResult == true && ListLines?.Count > 0 && !string.IsNullOrEmpty(input.TxtUserEdit.Text))
            {
                _search = input.TxtUserEdit.Text;
                _currentSearchMode = ECurrentSearchMode.BY_STRING;
                _searchCaseSensitive = StringComparison.CurrentCultureIgnoreCase;
                if (input.ChkCaseSensitive.IsChecked == true)
                    _searchCaseSensitive = StringComparison.CurrentCulture;
                OneLine find = ListLines.FirstOrDefault(line => line.Line.IndexOf(_search, 0, _searchCaseSensitive) >= 0);
                if (find != null)
                    SelectedLine = find;
                ShowSearchResult = true;
                OnPropertyChanged(nameof(ListLinesSearch));
                OnPropertyChanged(nameof(WhatToSearch));
            }
        }

        private void ToggleShowSearchResult()
        {
            ShowSearchResult = !_showSearchResult;
        }

        private void GotoLine()
        {
            InputBoxWindow input = new();
            input.ChkCaseSensitive.Visibility = Visibility.Collapsed;
            input.ShowModal(Locale.TITLE_GOTO, Locale.LBL_GOTO_LINE, _lastLineNumber.ToString());
            if (input.DialogResult == true && ListLines?.Count > 0 && int.TryParse(input.TxtUserEdit.Text, out int numLine))
            {
                _lastLineNumber = numLine;
                OneLine find = ListLines.FirstOrDefault(line => line.NumLine == numLine);
                if (find != null)
                    SelectedLine = find;
            }
        }

        public void UpdateRules(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ListLines));
            RefreshListRules();
        }

        internal void SetTextSelected(string text)
        {
            _selectedText = text;
        }

        private void AddRule()
        {
            RuleWindow win = new();
            win.MyDataContext.Text = _selectedText;
            if (win.ShowDialog() == true)
            {
                OneRule rule = new()
                {
                    Text = win.MyDataContext.Text,
                    BackgroundColor = win.MyDataContext.BackColor,
                    ForegroundColor = win.MyDataContext.ForeColor,
                    CaseSensitive = win.MyDataContext.CaseSensitive,
                    Not = win.MyDataContext.Not,
                    Priority = win.MyDataContext.Priority,
                    Conditions = ConditionsManager.ConditionStringToEnum(win.MyDataContext.Condition),
                    HideLine = win.MyDataContext.HideLine,
                    Title = win.MyDataContext.Title,
                    Category = win.MyDataContext.Category,
                    Bold = win.MyDataContext.Bold,
                    Italic = win.MyDataContext.Italic,
                    SubRules = win.MyDataContext.SubRules.ToList(),
                };
                _listRules.AddRule(rule);
                RefreshVisibleLines();
                UpdateCategory(rule.Category);
            }
        }

        internal void SetVisibleLine(int nbStart, int nbVisible)
        {
            _numVisibleStart = nbStart;
            _numVisibleEnd = nbStart + nbVisible;
            if (_listLines?.Count > 0 && _numVisibleEnd >= _listLines.Count)
                _numVisibleEnd = _listLines.Count - 1;
        }

        internal int NumStartVisibleLine
        {
            get { return _numVisibleStart; }
        }

        internal void RefreshVisibleLines()
        {
            if (_listLines == null)
                return;
            for (int i = _numVisibleStart - 1; i <= _numVisibleEnd; i++)
                _listLines[i].RefreshLine();
        }

        private void OpenListRules()
        {
            ListRulesWindow win = new();
            win.MyDataContext.ListRules = ListRules.ListRules;
            win.ShowDialog();
            RefreshVisibleLines();
        }

        private void SaveRules()
        {
            SaveFileDialog dialog = new()
            {
                Filter = "Xml file|*.xml|All files|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    SavedRules.SaveFile(dialog.FileName, Path.GetFileNameWithoutExtension(dialog.FileName), ListRules.ListRules);
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal($"{Locale.ERROR_SAVE_RULES}{Environment.NewLine}{ex.Message}", Locale.TITLE_ERROR);
                }
            }
        }

        internal void LoadRules(string filename = null)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Xml file|*.xml|All files|*.*",
            };
            if (!string.IsNullOrWhiteSpace(filename) || dialog.ShowDialog() == true)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(filename))
                        filename = dialog.FileName;
                    SavedRules result = SavedRules.LoadFile(filename);
                    if (result == null || result.ListRules?.Count == 0)
                        throw new LogRipperException(Locale.ERROR_NO_RULE_FOUND);
                    ListRules.SetRules(new ObservableCollection<OneRule>(result.ListRules));
                    RefreshListRules();
                    UpdateCategory();
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal($"{Locale.ERROR_LOAD_RULES}{Environment.NewLine}{ex.Message}", Locale.TITLE_ERROR);
                }
            }
        }

        private void MergeRules()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Xml file|*.xml|All files|*.*",
            };
            if (!string.IsNullOrWhiteSpace(dialog.FileName) || dialog.ShowDialog() == true)
            {
                try
                {
                    SavedRules result = SavedRules.LoadFile(dialog.FileName);
                    if (result == null || result.ListRules?.Count == 0)
                        throw new LogRipperException(Locale.ERROR_NO_RULE_FOUND);
                    foreach (OneRule rule in result.ListRules)
                        if (!_listRules.RuleExist(rule))
                            ListRules.ListRules.Add(rule);
                    RefreshListRules();
                    UpdateCategory();
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal($"{Locale.ERROR_LOAD_RULES}{Environment.NewLine}{ex.Message}", Locale.TITLE_ERROR);
                }
            }
        }

        private void ResetEncoding()
        {
            EncodageDefault = false;
            EncodageAscii = false;
            EncodageUtf7 = false;
            EncodageUtf8 = false;
            EncodageUtf32 = false;
            EncodageUnicode = false;
        }

        private void ChangeEncodageToDefault()
        {
            ResetEncoding();
            EncodageDefault = true;
        }

        private void ChangeEncodageToAscii()
        {
            ResetEncoding();
            EncodageAscii = true;
        }

        private void ChangeEncodageToUtf7()
        {
            ResetEncoding();
            EncodageUtf7 = true;
        }

        private void ChangeEncodageToUtf8()
        {
            ResetEncoding();
            EncodageUtf8 = true;
        }

        private void ChangeEncodageToUtf32()
        {
            ResetEncoding();
            EncodageUtf32 = true;
        }

        private void ChangeEncodageToUnicode()
        {
            ResetEncoding();
            EncodageUnicode = true;
        }

        private void ToggleShowNumLine()
        {
            CurrentShowNumLine = !CurrentShowNumLine;
        }

        private void ToggleShowFileName()
        {
            CurrentShowFileName = !CurrentShowFileName;
        }

        internal Encoding ReturnCurrentFileEncoding
        {
            get
            {
                Encoding encoding = Encoding.Default;
                if (_encodageAscii)
                    encoding = Encoding.ASCII;
                else if (_encodageUtf7)
                    encoding = Encoding.UTF7;
                else if (_encodageUtf8)
                    encoding = Encoding.UTF8;
                else if (_encodageUtf32)
                    encoding = Encoding.UTF32;
                else if (_encodageUnicode)
                    encoding = Encoding.Unicode;
                return encoding;
            }
        }

        public IEnumerable<OneLine> ListLinesSearch
        {
            get
            {
                if (_currentSearchMode == ECurrentSearchMode.BY_RULES)
                    return ListLines?.Where(line => !string.IsNullOrWhiteSpace(line.Line) && _listSearchRules.Exists(rule => rule.Execute(line.Line, line.Date)));
                return ListLines?.Where(line => !string.IsNullOrWhiteSpace(line.Line) && line.Line.IndexOf(_search, 0, _searchCaseSensitive) >= 0);
            }
        }

        internal void SearchRule(IEnumerable<OneRule> rules)
        {
            _listSearchRules.Clear();
            _listSearchRules.AddRange(rules);
            _currentSearchMode = ECurrentSearchMode.BY_RULES;
            ShowSearchResult = true;
            OnPropertyChanged(nameof(ListLinesSearch));
            OnPropertyChanged(nameof(WhatToSearch));
        }

        private void OpenSettings()
        {
            OptionsWindow win = new();
            win.ShowDialog();
        }

        private void MergeFile()
        {
            string format = Properties.Settings.Default.DefaultDateFormat;
            FusionWindow win = new();
            win.MyDataContext.FormatDate = format;
            if (win.ShowDialog() == true)
            {
                ActiveProgressRing = true;
                string firstLine = ListLines.Select(o => o.Line).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
                if (firstLine == null)
                {
                    WpfMessageBox.ShowModal(Locale.ERROR_EMPTY_FILE, Locale.TITLE_ERROR);
                    return;
                }
                foreach (OneFile file in FileManager.GetAllFiles().Where(f => string.IsNullOrWhiteSpace(f.DateFormat)))
                {
                    if (!DateTime.TryParseExact(firstLine.Substring(0, win.MyDataContext.FormatDate.Length), win.MyDataContext.FormatDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime _))
                    {
                        WpfMessageBox.ShowModal(Locale.ERROR_DATEFORMAT, Locale.TITLE_ERROR);
                        return;
                    }
                    FileManager.ComputeDate(ListLines.Where(l => l.FileName == file.FileName), win.MyDataContext.FormatDate);
                    file.DateFormat = win.MyDataContext.FormatDate;
                }
                if (ListLines.Count == 0)
                {
                    WpfMessageBox.ShowModal(Locale.ERROR_EMPTY_FILE, Locale.TITLE_ERROR);
                    return;
                }
                ObservableCollection<OneLine> secondFile = null;
                try
                {
                    secondFile = FileManager.LoadFile(win.MyDataContext.FileName, ReturnCurrentFileEncoding, new SolidColorBrush(win.MyDataContext.BackColor), new SolidColorBrush(win.MyDataContext.ForeColor), EnableAutoReload);
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal(string.Format(Locale.ERROR_READING_FILE, win.MyDataContext.FileName) + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
                }
                if (secondFile?.Count == 0)
                {
                    WpfMessageBox.ShowModal(Locale.ERROR_EMPTY_FILE, Locale.TITLE_ERROR);
                    return;
                }
                FileManager.ComputeDate(secondFile, win.MyDataContext.FormatDate);
                FileManager.GetFile(Path.GetFileNameWithoutExtension(win.MyDataContext.FileName)).DateFormat = win.MyDataContext.FormatDate;
                if (secondFile == null || secondFile.Count == 0)
                {
                    WpfMessageBox.ShowModal(Locale.ERROR_EMPTY_FILE, Locale.TITLE_ERROR);
                    return;
                }
                _listLines = new ObservableCollection<OneLine>(_listLines.Concat(secondFile).OrderBy(l => l.Date));
                OnPropertyChanged(nameof(ListLines));
                SetFilteredListLinesView();
                Properties.Settings.Default.DefaultDateFormat = win.MyDataContext.FormatDate;
                Properties.Settings.Default.Save();
                Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                ActiveProgressRing = false;
            }
        }

        private void AutoReload()
        {
            EnableAutoReload = !EnableAutoReload;
            FileManager.GetAllFiles().ForEach(file => file.ActiveAutoReload());
        }

        private void ToggleAutoScrollToEnd()
        {
            EnableAutoScrollToEnd = !EnableAutoScrollToEnd;
        }

        private void ShowMargin()
        {
            EnableShowMargin = !EnableShowMargin;
            Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
        }

        private void ShowToolbar()
        {
            EnableShowToolbar = !EnableShowToolbar;
            if (EnableShowToolbar)
                Application.Current.GetCurrentWindow<MainWindow>().Window_SizeChanged(null, null);
        }

        internal void RefreshListFiles()
        {
            ListFiles = new ObservableCollection<OneFile>(FileManager.GetAllFiles());
        }

        internal void RefreshListRules()
        {
            OnPropertyChanged(nameof(ListRules));
            RefreshVisibleLines();
        }

        internal void RefreshListLines()
        {
            OnPropertyChanged(nameof(ListLines));
            if (FilteredListLines == null)
                SetFilteredListLinesView();
            FilteredListLines?.Refresh();
        }

        private void SetFilteredListLinesView()
        {
            _filterListLines = CollectionViewSource.GetDefaultView(ListLines);
            if (_filterListLines != null)
                ((ListCollectionView)_filterListLines).Filter = FilterLine;
            OnPropertyChanged(nameof(FilteredListLines));
        }

        private bool FilterLine(object obj)
        {
            if (obj is OneLine line && FileManager.GetFile(line.FileName).Active)
            {
                if (HideAllOthersLines && !ListRules.ExecuteRules(line.Line, line.Date))
                    return false;
                if (_dateFilter)
                {
                    if (line.Date.CompareTo(_startDate) >= 0 && line.Date.CompareTo(_endDate) <= 0)
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        internal void AddLine(OneLine line)
        {
            ListLines.Add(line);
            if (EnableAutoReload && EnableAutoScrollToEnd)
            {
                _numVisibleStart++;
                _numVisibleEnd++;
            }
        }

        private void SaveState()
        {
            OneState.SaveCurrentState();
        }

        private void LoadState()
        {
            OneState.LoadNewState();
        }

        private void ShowAbout()
        {
            new AboutWindow().ShowDialog();
        }

        private void OpenFile()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Log file|*.log|All files|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ActiveProgressRing = true;
                    FileManager.RemoveAllFiles();
                    ListLines = FileManager.LoadFile(dialog.FileName, ReturnCurrentFileEncoding, activeAutoReload: EnableAutoReload);
                    Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                    FilterByDate = false;
                    ActiveProgressRing = false;
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal(string.Format(Locale.ERROR_READING_FILE, dialog.FileName) + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
                }
            }
        }

        private void ExportToHtml()
        {
            SaveFileDialog dialog = new()
            {
                Filter = "HTML file|*.html|All files|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                ActiveProgressRing = true;
                FileManager.ExportToHtml(ListLines, ListRules, dialog.FileName);
                ActiveProgressRing = false;
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void CopySelectedItems()
        {
            if (SelectedLines?.Count() > 0)
            {
                StringBuilder listCopy = new();
                foreach (OneLine selected in SelectedLines)
                {
                    if (listCopy.Length > 0)
                        listCopy.Append(Environment.NewLine);
                    listCopy.Append(selected.Line);
                }
                Clipboard.SetText(listCopy.ToString());
            }
        }

        private void SaveSearchResult()
        {
            SaveFileDialog dialog = new()
            {
                Filter = "Log files|*.log",
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    if (File.Exists(dialog.FileName))
                        File.Delete(dialog.FileName);
                    File.WriteAllLines(dialog.FileName, ListLinesSearch.Select(l => l.Line), Encoding.Default);
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal(Locale.ERROR_SAVE_FILE + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
                }
            }
        }

        private void ReloadAllFiles()
        {
            ActiveProgressRing = true;
            foreach (OneFile file in FileManager.GetAllFiles())
                file.ReloadFile();
            ActiveProgressRing = false;
        }

        private void AutoFollowMargin()
        {
            AutoFollowInMargin = !AutoFollowInMargin;
        }

        private void HideLineBefore()
        {
            if (RowIndexSelected <= 0)
                return;
            for (int i = 0; i < RowIndexSelected; i++)
                ListLines[i].GroupLines = "HideIt";
            RefreshListLines();
        }

        private void HideLineAfter()
        {
            if (RowIndexSelected + 1 >= ListLines.Count)
                return;
            for (int i = RowIndexSelected + 1; i < ListLines.Count; i++)
                ListLines[i].GroupLines = "HideIt";
            RefreshListLines();
        }

        #endregion
    }
}

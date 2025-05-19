using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Exceptions;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.Windows;

using Microsoft.Win32;

namespace LogRipper.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    #region Fields

    [ObservableProperty()]
    private ObservableCollection<OneLine> _listLines;
    [ObservableProperty()]
    private ObservableCollection<OneFile> _listFiles;
    [ObservableProperty()]
    private DateTime? _startDateTime, _endDateTime;
    [ObservableProperty()]
    private bool _encodingDefault, _encodingAscii, _encodingUtf7, _encodingUtf8, _encodingUtf32, _encodingUnicode, _hideAllOthersLines, _currentShowNumLine, _currentShowFileName;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(FilterByDateChecked))]
    private bool _filterByDate;
    [ObservableProperty()]
    private bool _enableShowDateFilter, _autoFollowInMargin, _activeProgressRing, _enableShowToolbar, _enableShowMargin, _enableAutoScrollToEnd, _enableAutoReload, _showSearchResult;
    [ObservableProperty()]
    private IEnumerable<OneLine> _selectedLines;
    [ObservableProperty()]
    private OneLine _selectedLine;
    [ObservableProperty()]
    private ListCurrentRules _listRules;
    private string _search;
    private StringComparison _searchCaseSensitive;
    private int _lastLineNumber;
    private readonly List<OneRule> _listSearchRules;
    private ECurrentSearchMode _currentSearchMode;
    private int _numVisibleStart;
    private int _numVisibleEnd;
    private DateTime? _minDate, _maxDate;
    private Thickness _margin;
    private string _selectedText;
    private int _numGroupLine;
    private ICollectionView _filterListLines;
    [ObservableProperty()]
    private ObservableCollection<TabItemSearch> _listSearchTab;
    [ObservableProperty()]
    private TabItemSearch _currentSearchTab;
    [ObservableProperty()]
    private int _rowIndexSelected;
    private bool _readConsoleMode;
    private NativeMethods.ConsoleScreenBufferInfo _lastCoords;

    #endregion

    #region Constructors

    public MainWindowViewModel() : base()
    {
        _listRules = new ListCurrentRules();
        _listSearchRules = [];
        ListCategory = [];
        _listRules.AddRuleEvent += UpdateRules;
        _listRules.RemoveRuleEvent += UpdateRules;
        _listRules.EditRuleEvent += UpdateRules;
        _listFiles = [];
        _listLines = [];
        _listSearchTab = [];
        _searchCaseSensitive = StringComparison.CurrentCultureIgnoreCase;

        ResetMinMaxDate();
        _startDateTime = DateTime.MinValue;
        _endDateTime = DateTime.MaxValue;

        ToggleShowNumLine();
        ToggleShowFileName();

        ChangeEncodingToDefault();
        SetMargin();

        EnableShowMargin = Properties.Settings.Default.ShowMargin;
        EnableShowToolbar = Properties.Settings.Default.ShowToolBar;
        EnableShowDateFilter = Properties.Settings.Default.ShowDateFilter;
        AutoFollowInMargin = Properties.Settings.Default.AutoFollowMargin;
    }

    #endregion

    #region Properties

    async partial void OnHideAllOthersLinesChanged(bool value)
    {
        await RefreshListLines();
    }

    async partial void OnEndDateTimeChanged(DateTime? value)
    {
        await RefreshListLines();
    }

    async partial void OnStartDateTimeChanged(DateTime? value)
    {
        await RefreshListLines();
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

    async partial void OnFilterByDateChanged(bool value)
    {
        ResetMinMaxDate();
        try
        {
            StartDateTime = ListLines.Where(line => line.Date != DateTime.MinValue).Min(line => line.Date);
            EndDateTime = ListLines.Where(line => line.Date != DateTime.MinValue).Max(line => line.Date);
        }
        catch (Exception)
        {
            StartDateTime = DateTime.MinValue;
            EndDateTime = DateTime.MaxValue;
        }
        _minDate = StartDateTime;
        _maxDate = EndDateTime;
        OnPropertyChanged(nameof(MinDate));
        OnPropertyChanged(nameof(MaxDate));
        await RefreshListLines();
    }

    public ObservableCollection<OneCategory> ListCategory { get; set; }

    public void UpdateCategory(string category = null)
    {
        if (!string.IsNullOrWhiteSpace(category) && !ListCategory.Any(c => c.Category.IndexOf(category, 0, StringComparison.CurrentCultureIgnoreCase) >= 0))
            ListCategory.Add(new OneCategory(category));
        if (ListRules.ListRules?.Count > 0)
        {
            for (int i = ListCategory.Count - 1; i >= 0; i--)
                if (!ListRules.ListRules.Any(r => !string.IsNullOrWhiteSpace(r.Category) && r.Category.IndexOf(ListCategory[i].Category, 0, StringComparison.CurrentCultureIgnoreCase) >= 0))
                    ListCategory.RemoveAt(i);

            foreach (string cat in ListRules.ListRules.Where(r => !string.IsNullOrWhiteSpace(r.Category)).Select(r => r.Category).Distinct())
                if (!ListCategory.Any(r => r.Category.IndexOf(cat, 0, StringComparison.CurrentCultureIgnoreCase) >= 0))
                    ListCategory.Add(new OneCategory(cat));
        }
        OnPropertyChanged(nameof(ListCategory));
    }

    partial void OnEnableShowMarginChanged(bool value)
    {
        Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
    }

    internal void SetMargin()
    {
        _margin = new Thickness(Values.FontSize, 0, 0, 0);
    }

    public Thickness Margin
    {
        get { return _margin; }
    }

    partial void OnSelectedLineChanged(OneLine value)
    {
        ((MainWindow)Application.Current.MainWindow).ScrollToSelected();
    }

    public ICollectionView FilteredListLines
    {
        get { return _filterListLines; }
    }

    async partial void OnListLinesChanged(ObservableCollection<OneLine> value)
    {
        await SetFilteredListLinesView();
    }

    public bool FilterByDateChecked
    {
        get { return FilterByDate; }
    }

    #endregion

    #region Methods

    [RelayCommand()]
    private async Task CheckForUpdate()
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        StringBuilder version = AutoUpdater.LatestVersion(Properties.Settings.Default.beta);
        if (version != null)
        {
            string ver = version.ToString();
            while (version.ToString().Count(c => c == '.') < 3)
            {
                version.Append(".0");
            }
            Version latestVersion = new(version.ToString());
            string result = string.Format(Locale.LBL_VERSION_COMPARE, Assembly.GetEntryAssembly().GetName().Version.ToString(), $"{latestVersion.Major}.{latestVersion.Minor}.{latestVersion.Build}.{latestVersion.Revision}");
            WpfMessageBox.ShowModal(result, "LogRipper");
            if (Assembly.GetEntryAssembly().GetName().Version.CompareTo(latestVersion) < 0 &&
                WpfMessageBox.ShowModal(string.Format(Locale.LBL_NEW_VERSION, latestVersion.ToString()), "LogRipper", MessageBoxButton.YesNo))
            {
                AutoUpdater.InstallNewVersion(ver, Properties.Settings.Default.beta);
            }
        }
        else
            WpfMessageBox.ShowModal(Locale.ERROR_CHECK_NEW_VERSION, Locale.TITLE_ERROR);
        ActiveProgressRing = false;
    }

    [RelayCommand()]
    private void ShowDateFilter()
    {
        EnableShowDateFilter = !EnableShowDateFilter;
    }

    [RelayCommand()]
    private void Search()
    {
        if (ListFiles == null || ListFiles.Count == 0)
        {
            WpfMessageBox.ShowModal(Locale.ERROR_EMPTY_FILE, Locale.TITLE_ERROR);
            return;
        }
        InputBoxWindow input = new();
        input.ChkCaseSensitive.IsChecked = _searchCaseSensitive == StringComparison.CurrentCulture;
        input.TxtUserEdit.Text = _selectedText;
        if (FilterByDate)
        {
            input.MyDataContext.FilterByDate = FilterByDate;
            input.MyDataContext.SetMinMaxDate(MinDate.Value, MaxDate.Value);
        }
        input.MyDataContext.ExecuteIfOk = CallbackSearch;
        input.Show(Locale.TITLE_SEARCH, Locale.LBL_SEARCH_TEXT, (string.IsNullOrWhiteSpace(_selectedText) ? _search : _selectedText));
    }

    private async Task CallbackSearch(InputBoxWindow input)
    {
        if (ListLines?.Count > 0 && !string.IsNullOrEmpty(input.TxtUserEdit.Text))
        {
            _listSearchRules.Clear();
            _search = input.TxtUserEdit.Text;
            _searchCaseSensitive = StringComparison.CurrentCultureIgnoreCase;
            if (input.ChkCaseSensitive.IsChecked == true)
                _searchCaseSensitive = StringComparison.CurrentCulture;
            _currentSearchMode = ECurrentSearchMode.BY_STRING;
            _searchByDate = input.MyDataContext.FilterByDate;
            _searchStartDate = input.MyDataContext.StartDateTime;
            _searchEndDate = input.MyDataContext.EndDateTime;
            await CreateSearchTab();
        }
    }

    private bool _searchByDate;
    private DateTime? _searchStartDate, _searchEndDate;

    private async Task CreateSearchTab()
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        TabItemSearch newTab = new();
        List<OneLine> result;
        if (_currentSearchMode == ECurrentSearchMode.BY_RULES)
            result = ListLines?.Where(line => !string.IsNullOrWhiteSpace(line.Line) && _listSearchRules.Exists(rule => rule.Execute(line.Line, line.Date))).ToList();
        else
        {
            result = ListLines?.Where(line => !string.IsNullOrWhiteSpace(line.Line) && line.Line.IndexOf(_search, 0, _searchCaseSensitive) >= 0).ToList();
            if (_searchByDate)
                result = [.. result.Where(line => (_searchStartDate.HasValue && line.Date >= _searchStartDate) && (_searchEndDate.HasValue && line.Date <= _searchEndDate))];
        }
        newTab.MyDataContext.SetNewSearch(result, _currentSearchMode, _search, _listSearchRules);
        newTab.MyDataContext.CurrentShowNumLine = CurrentShowNumLine;
        newTab.MyDataContext.CurrentShowFileName = CurrentShowFileName;
        ListSearchTab.Add(newTab);
        OnPropertyChanged(nameof(ListSearchTab));
        CurrentSearchTab = newTab;
        newTab.SetTitle(_currentSearchMode == ECurrentSearchMode.BY_RULES ? _listSearchRules[0].ToString() : _search);
        if (result?.Count > 0)
            SelectedLine = result[0];
        ShowSearchResult = true;
        ActiveProgressRing = false;
    }

    [RelayCommand()]
    private void ToggleShowSearchResult()
    {
        ShowSearchResult = !ShowSearchResult;
    }

    [RelayCommand()]
    private void GotoLine()
    {
        InputBoxWindow input = new();
        input.ChkCaseSensitive.Visibility = Visibility.Collapsed;
        input.StackDateFilter.Visibility = Visibility.Collapsed;
        input.MyDataContext.ExecuteIfOk = CallbackGotoLine;
        input.Show(Locale.TITLE_GOTO, Locale.LBL_GOTO_LINE, _lastLineNumber.ToString());
    }

    private async Task CallbackGotoLine(InputBoxWindow input)
    {
        if (ListLines?.Count > 0 && int.TryParse(input.TxtUserEdit.Text, out int numLine))
        {
            await Task.Delay(10);
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

    [RelayCommand()]
    private void AddRule()
    {
        RuleWindow win = new();
        win.MyDataContext.Text = _selectedText;
        win.MyDataContext.ExecuteWhenOk = () =>
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
                SubRules = [.. win.MyDataContext.SubRules],
            };
            ListRules.AddRule(rule);
            RefreshVisibleLines();
            UpdateCategory(rule.Category);
        };
        win.Show();
    }

    internal void SetVisibleLine(int nbStart, int nbVisible)
    {
        _numVisibleStart = nbStart;
        _numVisibleEnd = nbStart + nbVisible;
        if (ListLines?.Count > 0 && _numVisibleEnd >= ListLines.Count)
            _numVisibleEnd = ListLines.Count - 1;
    }

    internal int NumStartVisibleLine
    {
        get { return _numVisibleStart; }
    }

    internal void RefreshVisibleLines()
    {
        if (ListLines == null || ListLines.Count == 0)
            return;
        for (int i = _numVisibleStart - 1; i <= _numVisibleEnd; i++)
            ListLines[i].RefreshLine();
    }

    [RelayCommand()]
    private void OpenListRules()
    {
        ListRulesWindow win = new();
        win.MyDataContext.ListRules = ListRules.ListRules;
        win.Show();
    }

    [RelayCommand()]
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

    internal async Task LoadNewRules(string filename)
    {
        try
        {
            ActiveProgressRing = true;
            await Task.Delay(10);
            SavedRules result = SavedRules.LoadFile(filename);
            if (result == null || result.ListRules?.Count == 0)
                throw new LogRipperException(Locale.ERROR_NO_RULE_FOUND);
            ListRules.SetRules([.. result.ListRules]);
            RefreshListRules();
            UpdateCategory();
        }
        catch (Exception ex)
        {
            WpfMessageBox.ShowModal($"{Locale.ERROR_LOAD_RULES}{Environment.NewLine}{ex.Message}", Locale.TITLE_ERROR);
        }
        finally
        {
            ActiveProgressRing = false;
        }
    }

    [RelayCommand()]
    internal async Task LoadRules(string filename = null)
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Xml file|*.xml|All files|*.*",
        };
        if (!string.IsNullOrWhiteSpace(filename) || dialog.ShowDialog() == true)
        {
            if (string.IsNullOrWhiteSpace(filename))
                filename = dialog.FileName;
            await LoadNewRules(filename);
        }
    }

    internal async Task MergingRule(string[] listFiles)
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        foreach (string file in listFiles)
        {
            try
            {
                SavedRules result = SavedRules.LoadFile(file);
                if (result == null || result.ListRules?.Count == 0)
                    throw new LogRipperException(Locale.ERROR_NO_RULE_FOUND);
                foreach (OneRule rule in result.ListRules)
                    if (!ListRules.RuleExist(rule))
                        ListRules.ListRules.Add(rule);
                RefreshListRules();
                UpdateCategory();
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowModal($"{Locale.ERROR_LOAD_RULES}{Environment.NewLine}{ex.Message}", Locale.TITLE_ERROR);
            }
        }
        ActiveProgressRing = true;
    }

    [RelayCommand()]
    private async Task MergeRules()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Xml file|*.xml|All files|*.*",
        };
        if (!string.IsNullOrWhiteSpace(dialog.FileName) || dialog.ShowDialog() == true)
        {
            await MergingRule([dialog.FileName]);
        }
    }

    private void ResetEncoding()
    {
        EncodingDefault = false;
        EncodingAscii = false;
        EncodingUtf7 = false;
        EncodingUtf8 = false;
        EncodingUtf32 = false;
        EncodingUnicode = false;
    }

    [RelayCommand()]
    private void ChangeEncodingToDefault()
    {
        ResetEncoding();
        EncodingDefault = true;
    }

    [RelayCommand()]
    private void ChangeEncodingToAscii()
    {
        ResetEncoding();
        EncodingAscii = true;
    }

    [RelayCommand()]
    private void ChangeEncodingToUtf7()
    {
        ResetEncoding();
        EncodingUtf7 = true;
    }

    [RelayCommand()]
    private void ChangeEncodingToUtf8()
    {
        ResetEncoding();
        EncodingUtf8 = true;
    }

    [RelayCommand()]
    private void ChangeEncodingToUtf32()
    {
        ResetEncoding();
        EncodingUtf32 = true;
    }

    [RelayCommand()]
    private void ChangeEncodingToUnicode()
    {
        ResetEncoding();
        EncodingUnicode = true;
    }

    [RelayCommand()]
    private void ToggleShowNumLine()
    {
        CurrentShowNumLine = !CurrentShowNumLine;
    }

    [RelayCommand()]
    private void ToggleShowFileName()
    {
        CurrentShowFileName = !CurrentShowFileName;
    }

    internal Encoding ReturnCurrentFileEncoding
    {
        get
        {
            Encoding encoding = Encoding.Default;
            if (EncodingAscii)
                encoding = Encoding.ASCII;
            else if (EncodingUtf7)
                encoding = Encoding.UTF7;
            else if (EncodingUtf8)
                encoding = Encoding.UTF8;
            else if (EncodingUtf32)
                encoding = Encoding.UTF32;
            else if (EncodingUnicode)
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
    internal async Task SearchRule(OneRule rules)
    {
        await SearchRule([rules]);
    }

    internal async Task SearchRule(IEnumerable<OneRule> rules)
    {
        _listSearchRules.Clear();
        _listSearchRules.AddRange(rules);
        _currentSearchMode = ECurrentSearchMode.BY_RULES;
        await CreateSearchTab();
    }

    [RelayCommand()]
    private void OpenSettings()
    {
        OptionsWindow win = new();
        win.ShowDialog();
    }

    internal async Task MergingFile(string[] filesToMerge)
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        foreach (string file in filesToMerge)
        {
            FusionWindow win = new();
            OneFile nextFile = null;
            List<OneLine> listLines = null;
            win.MyDataContext.SetFileReadOnly(file);
            if (win.ShowDialog() == true)
            {
                try
                {
                    listLines = FileManager.LoadFile(file, out nextFile, ReturnCurrentFileEncoding, win.MyDataContext.BackColorBrush, win.MyDataContext.ForeColorBrush, EnableAutoReload);
                }
                catch (Exception ex)
                {
                    WpfMessageBox.ShowModal(string.Format(Locale.ERROR_READING_FILE, file) + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
                }
                if (listLines?.Count == 0)
                {
                    WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, file), Locale.TITLE_ERROR);
                    continue;
                }
                ListFiles.Add(nextFile);
                FileManager.ComputeDate(listLines, win.MyDataContext.FormatDate);
                nextFile.DateFormat = win.MyDataContext.FormatDate;
                if (!string.IsNullOrWhiteSpace(win.MyDataContext.FormatDate))
                    Properties.Settings.Default.DefaultDateFormat = win.MyDataContext.FormatDate;
                Properties.Settings.Default.Save();
                ListLines = [.. ListLines.Concat(listLines).OrderBy(l => l.Date)];
                Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
            }
        }
        ActiveProgressRing = false;
    }

    [RelayCommand()]
    private async Task MergeFile()
    {
        if (_readConsoleMode)
        {
            WpfMessageBox.ShowModal(Locale.NOT_DURING_READ_CONSOLE_MODE, Locale.TITLE_ERROR);
            return;
        }
        string format = Properties.Settings.Default.DefaultDateFormat;
        FusionWindow win = new();
        win.MyDataContext.FormatDate = format;
        if (win.ShowDialog() == true)
        {
            ActiveProgressRing = true;
            await Task.Delay(10);
            string firstLine = ListLines.Select(o => o.Line).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
            if (firstLine == null)
            {
                WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, FileManager.GetAllFiles()?[0]?.FullPath), Locale.TITLE_ERROR);
                return;
            }
            foreach (OneFile presentfile in FileManager.GetAllFiles().Where(f => string.IsNullOrWhiteSpace(f.DateFormat)))
            {
                bool result = await presentfile.EditFile();
                if (!result)
                    return;
            }
            if (ListLines.Count == 0)
            {
                WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, FileManager.GetAllFiles()?[0]?.FullPath), Locale.TITLE_ERROR);
                return;
            }
            List<OneLine> newListLines = null;
            OneFile newFile = null;
            try
            {
                newListLines = FileManager.LoadFile(win.MyDataContext.FileName, out newFile, ReturnCurrentFileEncoding, win.MyDataContext.BackColorBrush, win.MyDataContext.ForeColorBrush, EnableAutoReload);
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowModal(string.Format(Locale.ERROR_READING_FILE, win.MyDataContext.FileName) + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
            }
            if (newListLines?.Count == 0)
            {
                WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, win.MyDataContext.FileName), Locale.TITLE_ERROR);
                return;
            }
            if (newFile == null)
                throw new LogRipperException("OneFile object instance is null");
            ListFiles.Add(newFile);
            FileManager.ComputeDate(newListLines, win.MyDataContext.FormatDate);
            newFile.DateFormat = win.MyDataContext.FormatDate;
            if (newListLines == null || newListLines.Count == 0)
            {
                WpfMessageBox.ShowModal(string.Format(Locale.ERROR_EMPTY_FILE, win.MyDataContext.FileName), Locale.TITLE_ERROR);
                return;
            }
            if (!string.IsNullOrWhiteSpace(win.MyDataContext.FormatDate))
                Properties.Settings.Default.DefaultDateFormat = win.MyDataContext.FormatDate;
            Properties.Settings.Default.Save();
            ListLines = [.. ListLines.Concat(newListLines).OrderBy(l => l.Date)];
            Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
            string[] listFilesToMerge = win.MyDataContext.ListFiles;
            if (listFilesToMerge?.Length > 0)
                await MergingFile(listFilesToMerge);
            ActiveProgressRing = false;
        }
    }

    [RelayCommand()]
    private void AutoReload()
    {
        EnableAutoReload = !EnableAutoReload;
        FileManager.GetAllFiles().ForEach(file => file.ActiveAutoReload());
    }

    [RelayCommand()]
    private void ToggleAutoScrollToEnd()
    {
        EnableAutoScrollToEnd = !EnableAutoScrollToEnd;
    }

    [RelayCommand()]
    private void ShowMargin()
    {
        EnableShowMargin = !EnableShowMargin;
        Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
    }

    [RelayCommand()]
    private void ShowToolbar()
    {
        EnableShowToolbar = !EnableShowToolbar;
        if (EnableShowToolbar)
            Application.Current.GetCurrentWindow<MainWindow>().Window_SizeChanged(null, null);
    }

    internal void RefreshListFiles()
    {
        ListFiles = [.. FileManager.GetAllFiles()];
    }

    internal void RefreshListRules()
    {
        OnPropertyChanged(nameof(ListRules));
        RefreshVisibleLines();
    }

    internal async Task RefreshListLines()
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        OnPropertyChanged(nameof(ListLines));
        if (FilteredListLines == null)
            await SetFilteredListLinesView();
        FilteredListLines?.Refresh();
        ActiveProgressRing = false;
    }

    private async Task SetFilteredListLinesView()
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        _filterListLines = CollectionViewSource.GetDefaultView(ListLines);
        if (_filterListLines != null)
            ((ListCollectionView)_filterListLines).Filter = FilterLine;
        OnPropertyChanged(nameof(FilteredListLines));
        ActiveProgressRing = false;
    }

    private bool FilterLine(object obj)
    {
        if (obj is OneLine line && (FileManager.GetFile(line.FilePath)?.Active == true || !string.IsNullOrWhiteSpace(line.GroupLines)))
        {
            if (!string.IsNullOrWhiteSpace(line.GroupLines) && line.GroupLines.IndexOf(' ') < 0)
                return false;
            if (HideAllOthersLines && !ListRules.ExecuteRules(line.Line, line.Date))
                return false;
            if (FilterByDate)
            {
                if (line.Date.CompareTo(StartDateTime) >= 0 && line.Date.CompareTo(EndDateTime) <= 0)
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

    [RelayCommand()]
    private async Task SaveState()
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        OneState.SaveCurrentState();
        ActiveProgressRing = false;
    }

    [RelayCommand()]
    private async Task LoadState()
    {
        ActiveProgressRing = true;
        await Task.Delay(10);
        OneState.LoadNewState();
        ActiveProgressRing = false;
    }

    [RelayCommand()]
    private void ShowAbout()
    {
        new AboutWindow().ShowDialog();
    }

    internal async Task OpenNewFile(string filename)
    {
        ActiveProgressRing = true;
        List<OneLine> newListLines;
        if (ListLines.Count > 0)
            Application.Current.GetCurrentWindow<MainWindow>().ScrollToBegin();
        await Task.Delay(10);
        FileManager.RemoveAllFiles();
        try
        {
            newListLines = FileManager.LoadFile(filename, out OneFile file, ReturnCurrentFileEncoding, activeAutoReload: EnableAutoReload);
            if (file != null)
            {
                if (ListLines.Count > 0)
                {
                    _numVisibleStart = 1;
                    _numVisibleEnd = 2;
                    _filterListLines = null;
                    SelectedLine = null;
                    SelectedLines = null;
                    RowIndexSelected = 0;
                    ListFiles.Clear();
                    ListLines.Clear();
                }
                ListFiles.Add(file);
                ListLines = [.. newListLines];
                Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                FilterByDate = false;
                if (WpfMessageBox.ShowModal(Locale.ASK_EDIT_FILE_NOW, Locale.BTN_EDIT_RULE.Replace("_", ""), MessageBoxButton.YesNo))
                {
                    await file.EditFile();
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
        finally
        {
            ActiveProgressRing = false;
        }
    }

    [RelayCommand()]
    private async Task OpenFile()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Log file|*.log|All files|*.*",
            Multiselect = true,
        };
        if (dialog.ShowDialog() == true)
        {
            _readConsoleMode = false;
            try
            {
                await OpenNewFile(dialog.FileNames[0]);
                if (dialog.FileNames.Length > 1)
                {
                    List<string> otherFiles = [.. dialog.FileNames];
                    otherFiles.RemoveAt(0);
                    await MergingFile([.. otherFiles]);
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowModal(string.Format(Locale.ERROR_READING_FILE, dialog.FileName) + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
            }
        }
    }

    [RelayCommand()]
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

    [RelayCommand()]
    private void Exit()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand()]
    private void CopySelectedItems()
    {
        if (SelectedLines?.Any() == true)
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

    [RelayCommand()]
    private async Task ReloadAllFiles()
    {
        ActiveProgressRing = true;
        foreach (OneFile file in FileManager.GetAllFiles())
            await file.ReloadFile();
        ActiveProgressRing = false;
    }

    [RelayCommand()]
    private void AutoFollowMargin()
    {
        AutoFollowInMargin = !AutoFollowInMargin;
    }

    [RelayCommand()]
    private async Task HideLineBefore()
    {
        if (ListLines?.Count == 0)
            return;
        ActiveProgressRing = true;
        await Task.Run(() =>
        {
            if (RowIndexSelected <= 0)
                return;
            for (int i = 0; i < RowIndexSelected; i++)
                ListLines[i].GroupLines = "HideIt";
        });
        ListLines.Insert(RowIndexSelected - 1, new()
        {
            Line = "",
            GroupLines = string.Format(Locale.LBL_GROUP_LINE, ++_numGroupLine),
        });
        await RefreshListLines();
    }

    [RelayCommand()]
    private async Task HideLineAfter()
    {
        if (ListLines?.Count == 0)
            return;
        ActiveProgressRing = true;
        await Task.Run(() =>
        {
            if (RowIndexSelected + 1 >= ListLines.Count)
                return;
            for (int i = RowIndexSelected + 1; i < ListLines.Count; i++)
                ListLines[i].GroupLines = "HideIt";
        });
        ListLines.Insert(RowIndexSelected + 1, new()
        {
            Line = "",
            GroupLines = string.Format(Locale.LBL_GROUP_LINE, ++_numGroupLine),
        });
        await RefreshListLines();
    }

    [RelayCommand()]
    private async Task ShowHiddenLineBA()
    {
        if (ListLines?.Count == 0)
            return;
        ActiveProgressRing = true;
        List<OneLine> lineToRemove = [];
        await Task.Run(() =>
        {
            foreach (OneLine line in ListLines)
            {
                if (!string.IsNullOrWhiteSpace(line.GroupLines) && line.GroupLines.IndexOf(' ') >= 0)
                    lineToRemove.Add(line);
                line.GroupLines = null;
            }
        });
        if (lineToRemove.Count > 0)
            foreach (OneLine line in lineToRemove)
                ListLines.Remove(line);
        await RefreshListLines();
    }

    [RelayCommand()]
    private async Task ChangeFilterByDate()
    {
        if (!FilterByDate && ListFiles?.Count > 0 && string.IsNullOrWhiteSpace(ListFiles[0].DateFormat))
        {
            WpfMessageBox.ShowModal(Locale.NO_DATEFORMAT_IN_FILE, Locale.TITLE_ERROR);
            if (!await ListFiles[0].EditFile())
            {
                OnPropertyChanged(nameof(FilterByDateChecked));
                return;
            }
        }
        FilterByDate = !FilterByDate;
    }

    [RelayCommand()]
    public async Task HideSelectedLines()
    {
        if (SelectedLines?.Any() == true)
        {
            ActiveProgressRing = true;
            foreach (OneLine line in SelectedLines)
                line.GroupLines = "HideIt";
            ListLines.Insert(RowIndexSelected, new()
            {
                Line = "",
                GroupLines = string.Format(Locale.LBL_GROUP_LINE, ++_numGroupLine),
            });
            await RefreshListLines();
        }
    }

    [RelayCommand()]
    public void ReadConsole()
    {
        ChoiceProcessWindow window = new();
        window.ShowDialog();
    }

    public async Task ReadFromConsole(string process)
    {
        ActiveProgressRing = true;
        try
        {
            _readConsoleMode = true;
            List<OneLine> newListLines = [];
            if (ListLines.Count > 0)
                Application.Current.GetCurrentWindow<MainWindow>().ScrollToBegin();
            await Task.Delay(10);
            FileManager.RemoveAllFiles();

            _lastCoords = ConsoleHelper.GetConsoleInfo();
            for (short i = 0; i < _lastCoords.dwCursorPosition.Y; i++)
                newListLines.Add(new OneLine(process)
                {
                    NumLine = i,
                    Line = ConsoleHelper.GetText(0, i),
                });

            if (ListLines.Count > 0)
            {
                _numVisibleStart = 1;
                _numVisibleEnd = 2;
                _filterListLines = null;
                SelectedLine = null;
                SelectedLines = null;
                RowIndexSelected = 0;
                ListFiles.Clear();
                ListLines.Clear();
            }

            OneFile file = new(process, ConsoleHelper.GetCursorPosition().Y - 1, Encoding.UTF8, EnableAutoReload, true)
            {
                DefaultBackground = new SolidColorBrush(Constants.Colors.DefaultBackgroundColor),
                DefaultForeground = new SolidColorBrush(Constants.Colors.DefaultForegroundColor),
                FullPath = process,
            };
            ListFiles.Add(file);
            FileManager.AddFile(file);
            OnPropertyChanged(nameof(ListFiles));
            ListLines = [.. newListLines];
            Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
            RefreshVisibleLines();
            FilterByDate = false;
        }
        catch (Exception ex)
        {
            WpfMessageBox.ShowModal(ex.Message, Locale.TITLE_ERROR);
        }
        finally
        {
            ActiveProgressRing = false;
        }
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Helpers;
using LogRipper.Windows;

using Microsoft.Win32;

namespace LogRipper.ViewModels;

internal partial class OptionsWindowViewModel : ObservableObject
{
    private readonly List<OneLanguage> _listLanguages;
    private readonly List<string> _listThemes;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(BackColorBrush))]
    private Color? _defaultBackgroundColor;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ForeColorBrush))]
    private Color? _defaultForegroundColor;
    [ObservableProperty()]
    private OneLanguage _selectedLanguage;
    [ObservableProperty()]
    private string _rulesFilename, _selectedTheme, _currentDateFormat;
    [ObservableProperty()]
    private double? _defaultFontSize;
    [ObservableProperty()]
    private int? _defaultSpaceSize;
    [ObservableProperty()]
    private bool _wrapLines, _autoShowMargin, _autoShowDateFilter, _autoShowToolbar, _showIconSystray, _minimizeInSystray;

    public OptionsWindowViewModel() : base()
    {
        OneLanguage _default = null;
        OneLanguage _current = null;
        _listLanguages = new List<OneLanguage>();
        foreach (string lang in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Languages"), "*.ini"))
        {
            CultureInfo ci = CultureInfo.GetCultureInfo(Path.GetFileNameWithoutExtension(lang));
            if (ci != null)
            {
                _listLanguages.Add(new OneLanguage()
                {
                    Language = ci.DisplayName,
                    LanguageCode = ci.Name,
                });
                if (ci.IetfLanguageTag == "en-us")
                    _default = _listLanguages[_listLanguages.Count - 1];
                if (ci.IetfLanguageTag == Properties.Settings.Default.Language)
                    _current = _listLanguages[_listLanguages.Count - 1];
            }
        }
        OnPropertyChanged(nameof(ListLanguages));
        if (_current != null)
            SelectedLanguage = _current;
        else if (_default != null)
            SelectedLanguage = _default;
        _listThemes = new List<string>()
        {
            "Windows",
            Locale.THEME_LIGHT,
            Locale.THEME_DARK,
        };
        SelectedTheme = Properties.Settings.Default.Theme;
        DefaultBackgroundColor = Color.FromRgb(Properties.Settings.Default.DefaultBackgroundColor.R, Properties.Settings.Default.DefaultBackgroundColor.G, Properties.Settings.Default.DefaultBackgroundColor.B);
        DefaultForegroundColor = Color.FromRgb(Properties.Settings.Default.DefaultForegroundColor.R, Properties.Settings.Default.DefaultForegroundColor.G, Properties.Settings.Default.DefaultForegroundColor.B);
        _currentDateFormat = Properties.Settings.Default.DefaultDateFormat;
        _rulesFilename = Properties.Settings.Default.DefaultListRules;
        DefaultFontSize = Values.FontSize;
        DefaultSpaceSize = Values.SpaceSize;
        AutoShowDateFilter = Properties.Settings.Default.ShowDateFilter;
        AutoShowMargin = Properties.Settings.Default.ShowMargin;
        AutoShowToolbar = Properties.Settings.Default.ShowToolBar;
        WrapLines = Properties.Settings.Default.WrapLines;
        ShowIconSystray = Properties.Settings.Default.ShowInSystray;
        MinimizeInSystray = Properties.Settings.Default.MinimizeInSystray;
    }

    public List<OneLanguage> ListLanguages
    {
        get { return _listLanguages; }
    }

    public List<string> ListThemes
    {
        get { return _listThemes; }
    }

    public bool PresentInRegistry
    {
        get { return RegistryManager.AlreadyPresent(); }
    }

    public bool NotPresentInRegistry
    {
        get { return !RegistryManager.AlreadyPresent(); }
    }

    public Brush BackColorBrush
    {
        get { return new SolidColorBrush(DefaultBackgroundColor.Value); }
    }

    public Brush ForeColorBrush
    {
        get { return new SolidColorBrush(DefaultForegroundColor.Value); }
    }

    [RelayCommand()]
    public void AddRegistry()
    {
        RegistryManager.SetRegistry(false);
        OnPropertyChanged(nameof(PresentInRegistry));
        OnPropertyChanged(nameof(NotPresentInRegistry));
    }

    [RelayCommand()]
    public void RemoveRegistry()
    {
        RegistryManager.SetRegistry(true);
        OnPropertyChanged(nameof(PresentInRegistry));
        OnPropertyChanged(nameof(NotPresentInRegistry));
    }

    [RelayCommand()]
    internal void OpenRulesFilename()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Xml file|*.xml|All files|*.*",
        };
        if (dialog.ShowDialog() == true)
            RulesFilename = dialog.FileName;
    }

    [RelayCommand()]
    public void ResetWindowPosition()
    {
        Properties.Settings.Default.PosX = 0;
        Properties.Settings.Default.PosY = 0;
        Properties.Settings.Default.WindowState = "";
        Properties.Settings.Default.RulePosX = 0;
        Properties.Settings.Default.RulePosY = 0;
        Properties.Settings.Default.RuleWS = "";
        Properties.Settings.Default.ListRulesPosX = 0;
        Properties.Settings.Default.ListRulesPosY = 0;
        Properties.Settings.Default.ListRulesWS = "";
        Properties.Settings.Default.MergePosX = 0;
        Properties.Settings.Default.MergePosY = 0;
        Properties.Settings.Default.MergeWS = "";
        Properties.Settings.Default.InputBoxPosX = 0;
        Properties.Settings.Default.InputBoxPosY = 0;
        Properties.Settings.Default.SizeX = 0;
        Properties.Settings.Default.SizeY = 0;
        Properties.Settings.Default.RuleSizeX = 0;
        Properties.Settings.Default.RuleSizeY = 0;
        Properties.Settings.Default.ListRulesSizeX = 0;
        Properties.Settings.Default.ListRulesSizeY = 0;
        Properties.Settings.Default.MergeSizeX = 0;
        Properties.Settings.Default.MergeSizeY = 0;
        Properties.Settings.Default.InputBoxSizeX = 0;
        Properties.Settings.Default.InputBoxSizeY = 0;
        Properties.Settings.Default.FilePosX = 0;
        Properties.Settings.Default.FilePosY = 0;
        Properties.Settings.Default.FileSizeX = 0;
        Properties.Settings.Default.FileSizeY = 0;
        Properties.Settings.Default.SubRulePosX = 0;
        Properties.Settings.Default.SubRulePosY = 0;
        Properties.Settings.Default.SubRuleSizeX = 0;
        Properties.Settings.Default.SubRuleSizeY = 0;
        Properties.Settings.Default.SubRuleWS = "";
        Properties.Settings.Default.Save();
    }

    [RelayCommand()]
    public void SaveAndClose()
    {
        Properties.Settings.Default.Language = SelectedLanguage.LanguageCode;
        Properties.Settings.Default.DefaultDateFormat = CurrentDateFormat;
        Properties.Settings.Default.DefaultBackgroundColor = System.Drawing.Color.FromArgb(DefaultBackgroundColor.Value.R, DefaultBackgroundColor.Value.G, DefaultBackgroundColor.Value.B);
        Properties.Settings.Default.DefaultForegroundColor = System.Drawing.Color.FromArgb(DefaultForegroundColor.Value.R, DefaultForegroundColor.Value.G, DefaultForegroundColor.Value.B);
        Properties.Settings.Default.Theme = SelectedTheme;
        Properties.Settings.Default.DefaultListRules = RulesFilename;
        Properties.Settings.Default.FontSize = DefaultFontSize.Value;
        Properties.Settings.Default.SpaceSize = DefaultSpaceSize.Value;
        Properties.Settings.Default.ShowMargin = AutoShowMargin;
        Properties.Settings.Default.ShowToolBar = AutoShowToolbar;
        Properties.Settings.Default.ShowDateFilter = AutoShowDateFilter;
        Properties.Settings.Default.WrapLines = WrapLines;
        Properties.Settings.Default.ShowInSystray = ShowIconSystray;
        Properties.Settings.Default.MinimizeInSystray = MinimizeInSystray;
        Properties.Settings.Default.Save();
        Application.Current.GetCurrentWindow<OptionsWindow>().Close();
    }

    public sealed class OneLanguage
    {
        public string Language { get; set; }
        public string LanguageCode { get; set; }

        public override string ToString()
        {
            return Language;
        }
    }
}

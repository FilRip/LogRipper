using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;

using Microsoft.Win32;

namespace LogRipper.ViewModels
{
    internal class OptionsWindowViewModel : ViewModelBase
    {
        private readonly List<string> _listLanguages, _listThemes;
        private string _selectedLanguage;
        private string _currentDateFormat;
        private string _selectedTheme;
        private readonly ICommand _cmdResetPositionWindow, _cmdSave, _cmdAddRegistry, _cmdRemoveRegistry, _cmdRulesFilename;
        private Color? _backColor, _foreColor;
        private string _rulesFilename;
        private double? _fontSize;
        private int? _spaceSize;
        private bool _showMargin, _showToolbar, _showDateFilter;
        private bool _wrapLines;

        public OptionsWindowViewModel() : base()
        {
            _listLanguages = new List<string>()
            {
                "English",
            };
            foreach (string lang in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "Languages"), "*.ini"))
                _listLanguages.Add(Path.GetFileNameWithoutExtension(lang));
            if (_listLanguages.Contains(Properties.Settings.Default.Language))
                SelectedLanguage = Properties.Settings.Default.Language;
            else
                SelectedLanguage = "English";
            _listThemes = new List<string>()
            {
                "Windows",
                Locale.THEME_LIGHT,
                Locale.THEME_DARK,
            };
            SelectedTheme = Properties.Settings.Default.Theme;
            DefaultBackgroundColor = Color.FromRgb(Properties.Settings.Default.DefaultBackgroundColor.R, Properties.Settings.Default.DefaultBackgroundColor.G, Properties.Settings.Default.DefaultBackgroundColor.B);
            DefaultForegroundColor = Color.FromRgb(Properties.Settings.Default.DefaultForegroundColor.R, Properties.Settings.Default.DefaultForegroundColor.G, Properties.Settings.Default.DefaultForegroundColor.B);
            _cmdResetPositionWindow = new RelayCommand((param) => ResetWindowPosition());
            _cmdSave = new RelayCommand((param) => SaveAndClose());
            _currentDateFormat = Properties.Settings.Default.DefaultDateFormat;
            _cmdAddRegistry = new RelayCommand((param) => AddRegistry());
            _cmdRemoveRegistry = new RelayCommand((param) => RemoveRegistry());
            _rulesFilename = Properties.Settings.Default.DefaultListRules;
            _cmdRulesFilename = new RelayCommand((param) => OpenRulesFilename());
            _fontSize = Values.FontSize;
            _spaceSize = Values.SpaceSize;
            _showDateFilter = Properties.Settings.Default.ShowDateFilter;
            AutoShowMargin = Properties.Settings.Default.ShowMargin;
            AutoShowToolbar = Properties.Settings.Default.ShowToolBar;
            WrapLines = Properties.Settings.Default.WrapLines;
        }

        public ICommand CmdRulesFilename
        {
            get { return _cmdRulesFilename; }
        }

        public bool WrapLines
        {
            get { return _wrapLines; }
            set
            {
                if (_wrapLines != value)
                {
                    _wrapLines = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoShowMargin
        {
            get { return _showMargin; }
            set
            {
                if (_showMargin != value)
                {
                    _showMargin = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoShowDateFilter
        {
            get { return _showDateFilter; }
            set
            {
                if (_showDateFilter != value)
                {
                    _showDateFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoShowToolbar
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

        public string RulesFilename
        {
            get { return _rulesFilename; }
            set
            {
                if (_rulesFilename != value)
                {
                    _rulesFilename = value;
                    OnPropertyChanged();
                }
            }
        }

        public double? DefaultFontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? DefaultSpaceSize
        {
            get { return _spaceSize; }
            set
            {
                if (_spaceSize != value)
                {
                    _spaceSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> ListLanguages
        {
            get { return _listLanguages; }
        }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> ListThemes
        {
            get { return _listThemes; }
        }

        public string SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentDateFormat
        {
            get { return _currentDateFormat; }
            set
            {
                if (_currentDateFormat != value)
                {
                    _currentDateFormat = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdResetPositionWindow
        {
            get { return _cmdResetPositionWindow; }
        }

        public ICommand CmdSaveAndClose
        {
            get { return _cmdSave; }
        }

        public ICommand CmdAddRegistry
        {
            get { return _cmdAddRegistry; }
        }

        public ICommand CmdRemoveRegistry
        {
            get { return _cmdRemoveRegistry; }
        }

        public bool PresentInRegistry
        {
            get { return RegistryManager.AlreadyPresent(); }
        }

        public bool NotPresentInRegistry
        {
            get { return !RegistryManager.AlreadyPresent(); }
        }

        public Color? DefaultBackgroundColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackColorBrush));
                }
            }
        }

        public Color? DefaultForegroundColor
        {
            get { return _foreColor; }
            set
            {
                if (_foreColor != value)
                {
                    _foreColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ForeColorBrush));
                }
            }
        }

        public Brush BackColorBrush
        {
            get { return new SolidColorBrush(_backColor.Value); }
        }

        public Brush ForeColorBrush
        {
            get { return new SolidColorBrush(_foreColor.Value); }
        }

        public void AddRegistry()
        {
            RegistryManager.SetRegistry(false);
            OnPropertyChanged(nameof(PresentInRegistry));
            OnPropertyChanged(nameof(NotPresentInRegistry));
        }

        public void RemoveRegistry()
        {
            RegistryManager.SetRegistry(true);
            OnPropertyChanged(nameof(PresentInRegistry));
            OnPropertyChanged(nameof(NotPresentInRegistry));
        }

        internal void OpenRulesFilename()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Xml file|*.xml|All files|*.*",
            };
            if (dialog.ShowDialog() == true)
                RulesFilename = dialog.FileName;
        }

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

        public void SaveAndClose()
        {
            Properties.Settings.Default.Language = SelectedLanguage;
            Properties.Settings.Default.DefaultDateFormat = CurrentDateFormat;
            Properties.Settings.Default.DefaultBackgroundColor = System.Drawing.Color.FromArgb(_backColor.Value.R, _backColor.Value.G, _backColor.Value.B);
            Properties.Settings.Default.DefaultForegroundColor = System.Drawing.Color.FromArgb(_foreColor.Value.R, _foreColor.Value.G, _foreColor.Value.B);
            Properties.Settings.Default.Theme = SelectedTheme;
            Properties.Settings.Default.DefaultListRules = _rulesFilename;
            Properties.Settings.Default.FontSize = _fontSize.Value;
            Properties.Settings.Default.SpaceSize = _spaceSize.Value;
            Properties.Settings.Default.ShowMargin = _showMargin;
            Properties.Settings.Default.ShowToolBar = _showToolbar;
            Properties.Settings.Default.ShowDateFilter = _showDateFilter;
            Properties.Settings.Default.WrapLines = _wrapLines;
            Properties.Settings.Default.Save();
            Application.Current.GetCurrentWindow<OptionsWindow>().Close();
        }
    }
}

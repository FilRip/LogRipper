using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.Windows;

namespace LogRipper.ViewModels
{
    public class RuleWindowViewModel : ViewModelBase
    {
        #region Fields

        private string _condition;
        private string _text;
        private Color _backColor;
        private Color _foreColor;
        private bool _caseSensitive;
        private int _priority;
        private bool _not;
        private readonly ICommand _add, _close, _addSubRule, _removeSubRule, _cmdEditSubRule;
        private string _title;
        private bool _hideLine;
        private string _category;
        private bool _bold, _italic;
        private bool _dialogResult;
        private Action _closeAction;
        private ObservableCollection<OneSubRule> _subRules;
        private OneSubRule _selectedSubRule;

        #endregion

        #region Constructors

        public RuleWindowViewModel() : base()
        {
            _dialogResult = false;
            BackColor = Constants.Colors.DefaultBackgroundColor;
            ForeColor = Constants.Colors.DefaultForegroundColor;
            _add = new RelayCommand((param) => AddRule());
            _close = new RelayCommand((param) => CloseWindow());
            _addSubRule = new RelayCommand((param) => AddSubRule());
            _removeSubRule = new RelayCommand((param) => RemoveSubRule());
            _cmdEditSubRule = new RelayCommand((param) => EditSubRule());
            Condition = ListConditions[0];
            SubRules = new ObservableCollection<OneSubRule>();
        }

        #endregion

        #region Properties

        public ICommand CmdEditSubRule
        {
            get { return _cmdEditSubRule; }
        }

        public ICommand CmdAddSubRule
        {
            get { return _addSubRule; }
        }

        public ICommand CmdRemoveSubRule
        {
            get { return _removeSubRule; }
        }

        internal void SetCloseAction(Action close)
        {
            _closeAction = close;
        }

        internal bool DialogResult
        {
            get { return _dialogResult; }
        }

        public bool Bold
        {
            get { return _bold; }
            set
            {
                if (_bold != value)
                {
                    _bold = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        public FontWeight Weight
        {
            get { return (_bold ? FontWeights.Bold : FontWeights.Normal); }
        }

        public ObservableCollection<OneSubRule> SubRules
        {
            get { return _subRules; }
            set
            {
                if (_subRules != value)
                {
                    if (value == null)
                        _subRules = new ObservableCollection<OneSubRule>();
                    else
                        _subRules = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ListSubRules));
                }
            }
        }

        public OneSubRule SelectedSubRule
        {
            get { return _selectedSubRule; }
            set
            {
                if (_selectedSubRule != value)
                {
                    _selectedSubRule = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ListSubRules
        {
            get
            {
                if (_subRules?.Count > 0)
                {
                    StringBuilder sb = new();
                    foreach (OneSubRule subRule in _subRules)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.Append(subRule.ToString());
                    }
                    return sb.ToString();
                }
                return "";
            }
        }

        public bool Italic
        {
            get { return _italic; }
            set
            {
                if (_italic != value)
                {
                    _italic = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Style));
                }
            }
        }

        public FontStyle Style
        {
            get { return (_italic ? FontStyles.Italic : FontStyles.Normal); }
        }

        public List<string> ListConditions
        {
            get { return Locale.ListConditions; }
        }

        public string Condition
        {
            get { return _condition; }
            set
            {
                if (_condition != value)
                {
                    _condition = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TextVisible));
                    OnPropertyChanged(nameof(ScriptVisible));
                    OnPropertyChanged(nameof(RegExVisible));
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color BackColor
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

        public Color ForeColor
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
            get { return new SolidColorBrush(_backColor); }
        }

        public Brush ForeColorBrush
        {
            get { return new SolidColorBrush(_foreColor); }
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                if (_caseSensitive != value)
                {
                    _caseSensitive = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Not
        {
            get { return _not; }
            set
            {
                if (_not != value)
                {
                    _not = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HideLine
        {
            get { return _hideLine; }
            set
            {
                if (_hideLine != value)
                {
                    _hideLine = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CmdAddRule
        {
            get { return _add; }
        }

        public ICommand CmdCloseWindow
        {
            get { return _close; }
        }

        public bool TextVisible
        {
            get
            {
                if (Condition == Locale.LBL_CONTAINS ||
                    Condition == Locale.LBL_START_WITH ||
                    Condition == Locale.LBL_END_WITH)

                    return true;
                return false;
            }
        }

        public bool RegExVisible
        {
            get
            {
                return (Condition == Locale.LBL_REG_EX);
            }
        }

        public bool ScriptVisible
        {
            get
            {
                return (Condition == Locale.LBL_SCRIPT_CSHARP);
            }
        }

        #endregion

        #region Methods

        internal void LoadRule(OneRule rule)
        {
            Condition = ConditionsManager.ConditionEnumToString(rule.Conditions);
            Text = rule.Text;
            ForeColor = rule.ForegroundColor;
            BackColor = rule.BackgroundColor;
            Not = rule.Not;
            CaseSensitive = rule.CaseSensitive;
            Priority = rule.Priority;
            Title = rule.Title;
            HideLine = rule.HideLine;
            Category = rule.Category;
            Bold = rule.Bold;
            Italic = rule.Italic;
            SubRules = new ObservableCollection<OneSubRule>(rule.SubRules);
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.UpdateCategory(Category);
        }

        private void AddRule()
        {
            if (TestIfRequestFieldPresent())
            {
                _dialogResult = true;
                _closeAction();
            }
        }

        private bool TestIfRequestFieldPresent()
        {
            if (string.IsNullOrWhiteSpace(Condition))
            {
                WpfMessageBox.ShowModal(Locale.ERROR_NO_CONDITION, Locale.TITLE_ERROR);
                return false;
            }
            Conditions cond = ConditionsManager.ConditionStringToEnum(Condition);
            switch (cond)
            {
                case Conditions.REG_EX:
                    try
                    {
                        _ = new Regex(_text);
                    }
                    catch (Exception ex)
                    {
                        WpfMessageBox.ShowModal(Locale.ERROR_REGEX + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
                        return false;
                    }
                    break;
                case Conditions.SCRIPT:
                    CompilerResults result = Compiler.Compile(_text);
                    if (result == null || result.Errors.Count > 0)
                    {
                        StringBuilder sb = new();
                        if (result != null && result.Errors.Count > 0)
                        {
                            foreach (CompilerError error in result.Errors)
                                sb.AppendLine(error.ToString());
                        }
                        WpfMessageBox.ShowModal(Locale.ERROR_SCRIPT + Environment.NewLine + sb.ToString(), Locale.TITLE_ERROR);
                        return false;
                    }
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(_text))
                    {
                        WpfMessageBox.ShowModal(Locale.ERROR_TEXT, Locale.TITLE_ERROR);
                        return false;
                    }
                    break;
            }
            return true;
        }

        public IEnumerable<string> ListCategory
        {
            get { return Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListCategory.Select(c => c.Category); }
        }

        private void CloseWindow()
        {
            _closeAction();
        }

        private void AddSubRule()
        {
            if (!TestIfRequestFieldPresent())
                return;

            SubRuleWindow win = new(this);
            if (win.ShowDialog() == true)
            {
                OnPropertyChanged(nameof(ListSubRules));
                OnPropertyChanged(nameof(SubRules));
            }
        }

        private void RemoveSubRule()
        {
            if (_selectedSubRule == null)
                return;
            _subRules.Remove(_selectedSubRule);
            _selectedSubRule = null;
            OnPropertyChanged(nameof(ListSubRules));
            OnPropertyChanged(nameof(SubRules));
        }

        private void EditSubRule()
        {
            if (!TestIfRequestFieldPresent())
                return;

            if (_selectedSubRule == null)
                return;

            SubRuleWindow win = new(this);
            win.LoadSubRule(_selectedSubRule);
            if (win.ShowDialog() == true)
            {
                _selectedSubRule.CaseSensitive = win.MyDataContext.CaseSensitive;
                _selectedSubRule.Concatenation = win.MyDataContext.Concatenation;
                _selectedSubRule.Conditions = ConditionsManager.ConditionStringToEnum(win.MyDataContext.Condition);
                _selectedSubRule.Text = win.MyDataContext.Text;
                _selectedSubRule.Refresh();
                SubRules = new ObservableCollection<OneSubRule>(_subRules.ToList());
                OnPropertyChanged(nameof(SelectedSubRule));
            }
        }

        #endregion
    }
}

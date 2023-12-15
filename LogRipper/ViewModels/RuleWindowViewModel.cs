using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.Windows;

namespace LogRipper.ViewModels;

public partial class RuleWindowViewModel : ObservableObject
{
    #region Fields

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TextVisible), nameof(RegExVisible), nameof(ScriptVisible))]
    private string _condition;

    [ObservableProperty()]
    private string _text, _title, _category;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(BackColorBrush))]
    private Color _backColor;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ForeColorBrush))]
    private Color _foreColor;

    [ObservableProperty()]
    private bool _caseSensitive, _not, _hideLine;

    [ObservableProperty()]
    private int _priority;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(Weight))]
    private bool _bold;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(Style))]
    private bool _italic;

    private bool _dialogResult;
    private Action _closeAction;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ListSubRules))]
    private ObservableCollection<OneSubRule> _subRules;

    [ObservableProperty()]
    private OneSubRule _selectedSubRule;

    #endregion

    #region Constructors

    public RuleWindowViewModel() : base()
    {
        _dialogResult = false;
        BackColor = Constants.Colors.DefaultBackgroundColor;
        ForeColor = Constants.Colors.DefaultForegroundColor;
        Condition = ListConditions[0];
        SubRules = [];
    }

    #endregion

    #region Properties

    internal void SetCloseAction(Action close)
    {
        _closeAction = close;
    }

    internal bool DialogResult
    {
        get { return _dialogResult; }
    }

    public FontWeight Weight
    {
        get { return (Bold ? FontWeights.Bold : FontWeights.Normal); }
    }

    public string ListSubRules
    {
        get
        {
            if (SubRules?.Count > 0)
            {
                StringBuilder sb = new();
                foreach (OneSubRule subRule in SubRules)
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

    public FontStyle Style
    {
        get { return (Italic ? FontStyles.Italic : FontStyles.Normal); }
    }

    public List<string> ListConditions
    {
        get { return Locale.ListConditions; }
    }

    public Brush BackColorBrush
    {
        get { return new SolidColorBrush(BackColor); }
    }

    public Brush ForeColorBrush
    {
        get { return new SolidColorBrush(ForeColor); }
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

    [RelayCommand()]
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
        return Compiler.TestCondition(cond, Text);
    }

    public IEnumerable<string> ListCategory
    {
        get { return Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListCategory.Select(c => c.Category); }
    }

    [RelayCommand()]
    private void CloseWindow()
    {
        _closeAction();
    }

    [RelayCommand()]
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

    [RelayCommand()]
    private void RemoveSubRule()
    {
        if (SelectedSubRule == null)
            return;
        SubRules.Remove(SelectedSubRule);
        SelectedSubRule = null;
        OnPropertyChanged(nameof(ListSubRules));
        OnPropertyChanged(nameof(SubRules));
    }

    [RelayCommand()]
    private void EditSubRule()
    {
        if (!TestIfRequestFieldPresent())
            return;

        if (SelectedSubRule == null)
            return;

        SubRuleWindow win = new(this);
        win.LoadSubRule(SelectedSubRule);
        if (win.ShowDialog() == true)
        {
            SelectedSubRule.CaseSensitive = win.MyDataContext.CaseSensitive;
            SelectedSubRule.Concatenation = win.MyDataContext.Concatenation;
            SelectedSubRule.Conditions = ConditionsManager.ConditionStringToEnum(win.MyDataContext.Condition);
            SelectedSubRule.Text = win.MyDataContext.Text;
            SelectedSubRule.Refresh();
            SubRules = new ObservableCollection<OneSubRule>(SubRules.ToList());
            OnPropertyChanged(nameof(SelectedSubRule));
        }
    }

    #endregion
}

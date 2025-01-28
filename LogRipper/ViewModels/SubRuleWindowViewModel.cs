using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Helpers;

namespace LogRipper.ViewModels;

public partial class SubRuleWindowViewModel : ObservableObject
{
    private bool _dialogResult;
    private Action _closeAction;
    private readonly List<string> _listConcatenation;
    private RuleWindowViewModel _parentRule;
    private bool _modify;

    [ObservableProperty(), NotifyPropertyChangedFor(nameof(TextVisible), nameof(RegExVisible), nameof(ScriptVisible))]
    private string _condition;

    [ObservableProperty()]
    private bool _caseSensitive;

    [ObservableProperty()]
    private string _text, _selectedConcatenation;

    public SubRuleWindowViewModel() : base()
    {
        _modify = false;
        _dialogResult = false;
        _listConcatenation =
        [
            Locale.CONCATENATION_AND,
            Locale.CONCATENATION_OR,
        ];
        Condition = ListConditions[0];
        SelectedConcatenation = _listConcatenation[0];
    }

    internal void SetModify()
    {
        _modify = true;
    }

    public List<string> ListConcatenation
    {
        get { return _listConcatenation; }
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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public List<string> ListConditions
    {
        get { return Locale.ListConditions; }
    }
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

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

    internal Concatenation Concatenation
    {
        get
        {
            if (SelectedConcatenation == Locale.CONCATENATION_AND)
                return Concatenation.AND;
            else
                return Concatenation.OR;
        }
    }

    public bool DialogResult
    {
        get { return _dialogResult; }
    }

    internal void SetCloseAction(Action closeAction)
    {
        _closeAction = closeAction;
    }

    internal void SetParentRule(RuleWindowViewModel parentRule)
    {
        _parentRule = parentRule;
    }

    [RelayCommand()]
    private void Add()
    {
        Conditions cond = ConditionsManager.ConditionStringToEnum(Condition);
        if (!Compiler.TestCondition(cond, Text))
            return;
        if (!_modify)
        {
            _parentRule.SubRules.Add(new()
            {
                Concatenation = Concatenation,
                Text = Text,
                Conditions = cond,
                CaseSensitive = CaseSensitive,
            });
        }
        _dialogResult = true;
        _closeAction();
    }

    [RelayCommand()]
    private void Cancel()
    {
        _closeAction();
    }
}

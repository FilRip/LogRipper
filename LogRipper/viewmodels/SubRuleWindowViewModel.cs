using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

using LogRipper.Constants;
using LogRipper.Helpers;

namespace LogRipper.ViewModels
{
    public class SubRuleWindowViewModel : ViewModelBase
    {
        private bool _dialogResult;
        private Action _closeAction;
        private readonly ICommand _cmdAdd, _cmdCancel;
        private readonly List<string> _listConcatenation;
        private string _selectedConcatenation, _condition;
        private RuleWindowViewModel _parentRule;
        private bool _caseSensitive;
        private string _text;
        private bool _modify;

        public SubRuleWindowViewModel() : base()
        {
            _modify = false;
            _dialogResult = false;
            _cmdAdd = new RelayCommand((param) => Add());
            _cmdCancel = new RelayCommand((param) => Cancel());
            _listConcatenation = new List<string>()
            {
                Locale.CONCATENATION_AND,
                Locale.CONCATENATION_OR,
            };
            Condition = ListConditions[0];
            SelectedConcatenation = _listConcatenation[0];
        }

        internal void SetModify()
        {
            _modify = true;
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

        public List<string> ListConcatenation
        {
            get { return _listConcatenation; }
        }

        public string SelectedConcatenation
        {
            get { return _selectedConcatenation; }
            set
            {
                if (_selectedConcatenation != value)
                {
                    _selectedConcatenation = value;
                    OnPropertyChanged();
                }
            }
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
                if (_selectedConcatenation == Locale.CONCATENATION_AND)
                    return Concatenation.AND;
                else
                    return Concatenation.OR;
            }
        }

        public bool DialogResult
        {
            get { return _dialogResult; }
        }

        public ICommand CmdAdd
        {
            get { return _cmdAdd; }
        }

        public ICommand CmdCancel
        {
            get { return _cmdCancel; }
        }

        internal void SetCloseAction(Action closeAction)
        {
            _closeAction = closeAction;
        }

        internal void SetParentRule(RuleWindowViewModel parentRule)
        {
            _parentRule = parentRule;
        }

        private void Add()
        {
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
                        return;
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
                        return;
                    }
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(_text))
                    {
                        WpfMessageBox.ShowModal(Locale.ERROR_TEXT, Locale.TITLE_ERROR);
                        return;
                    }
                    break;
            }
            if (!_modify)
            {
                _parentRule.SubRules.Add(new()
                {
                    Concatenation = Concatenation,
                    Text = _text,
                    Conditions = cond,
                    CaseSensitive = _caseSensitive,
                });
            }
            _dialogResult = true;
            _closeAction();
        }

        private void Cancel()
        {
            _closeAction();
        }
    }
}

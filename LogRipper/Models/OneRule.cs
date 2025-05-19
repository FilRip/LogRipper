using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.Models;

[XmlRoot()]
public partial class OneRule : RuleViewModelBase
{
    private bool _active;
    [ObservableProperty()]
    private List<OneSubRule> _subRules;

    public OneRule() : base()
    {
        _subRules = [];
        _active = true;
    }

    private Color _foreground, _background;
    private SolidColorBrush _foregroundBrush, _backgroundBrush;

    [XmlElement()]
    public string Title { get; set; }

    [XmlElement()]
    public Color BackgroundColor
    {
        get { return _background; }
        set
        {
            if (_background != value)
            {
                _background = value;
                _backgroundBrush = new SolidColorBrush(_background);
            }
        }
    }

    [XmlIgnore()]
    public SolidColorBrush BackgroundBrush
    {
        get { return _backgroundBrush; }
    }

    [XmlElement()]
    public Color ForegroundColor
    {
        get { return _foreground; }
        set
        {
            if (_foreground != value)
            {
                _foreground = value;
                _foregroundBrush = new SolidColorBrush(_foreground);
            }
        }
    }

    [XmlIgnore()]
    public SolidColorBrush ForegroundBrush
    {
        get { return _foregroundBrush; }
    }

    [XmlElement()]
    public int Priority { get; set; }

    [XmlElement()]
    public bool Not { get; set; }

    [XmlElement()]
    public bool HideLine { get; set; }

    [XmlElement()]
    public bool Active
    {
        get { return _active; }
        set
        {
            if (_active != value)
            {
                _active = value;
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
                Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
            }
        }
    }

    [XmlElement()]
    public string Category { get; set; }

    [XmlElement()]
    public bool Bold { get; set; }

    [XmlElement()]
    public bool Italic { get; set; }

    internal override bool Execute(string line, DateTime dateline)
    {
        if (!Active)
            return false;

        bool result = base.Execute(line, dateline);

        if (SubRules.Count > 0)
            foreach (OneSubRule rule in SubRules)
            {
                bool r2 = rule.Execute(line, dateline);
                result = rule.Concatenation switch
                {
                    Concatenation.OR => result || r2,
                    _ => result && r2,
                };
            }

        return (Not ? !result : result);
    }

    internal override bool AreSame(RuleViewModelBase baseRule)
    {
        OneRule rule = (OneRule)baseRule;
        if (!base.AreSame(rule))
            return false;
        if (rule.BackgroundColor != BackgroundColor)
            return false;
        if (rule.ForegroundColor != ForegroundColor)
            return false;
        if (rule.Priority != Priority)
            return false;
        if (rule.Not != Not)
            return false;
        if (rule.HideLine != HideLine)
            return false;
        if (rule.Category != Category)
            return false;
        if (rule.Bold != Bold)
            return false;
        if (rule.Italic != Italic)
            return false;
        if (rule.SubRules.Count != SubRules.Count)
            return false;
        if (SubRules.Count > 0)
            for (int i = 0; i < SubRules.Count; i++)
                if (!SubRules[i].AreSame(rule.SubRules[i]))
                    return false;
        return true;
    }

    [XmlIgnore()]
    public string Label
    {
        get { return ToString(); }
    }

    [RelayCommand()]
    private void DeleteThisRule()
    {
        if (WpfMessageBox.ShowModal($"{Locale.ASK_CONFIRM_DEL}{Environment.NewLine}{Label}", Locale.TITLE_CONFIRM_DEL, MessageBoxButton.YesNo))
        {
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.RemoveRule(this);
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.UpdateCategory();
        }
    }

    [RelayCommand()]
    private async Task SearchResult()
    {
        await Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.SearchRule(this);
    }

    [RelayCommand()]
    internal void Edit()
    {
        RuleWindow win = new()
        {
            Title = Locale.TITLE_EDIT_RULE,
        };
        win.BtnOK.Text = Locale.BTN_EDIT_RULE;
        win.MyDataContext.LoadRule(this);
        win.MyDataContext.ExecuteWhenOk = () =>
        {
            Conditions = ConditionsManager.ConditionStringToEnum(win.MyDataContext.Condition);
            BackgroundColor = win.MyDataContext.BackColor;
            ForegroundColor = win.MyDataContext.ForeColor;
            Text = win.MyDataContext.Text;
            Priority = win.MyDataContext.Priority;
            CaseSensitive = win.MyDataContext.CaseSensitive;
            HideLine = win.MyDataContext.HideLine;
            Not = win.MyDataContext.Not;
            Title = win.MyDataContext.Title;
            Category = win.MyDataContext.Category;
            Bold = win.MyDataContext.Bold;
            Italic = win.MyDataContext.Italic;
            SubRules = [.. win.MyDataContext.SubRules];
            Refresh();
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.UpdateCategory(win.MyDataContext.Category);
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.EditRule(this);
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.UpdateRules(null, EventArgs.Empty);
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
            Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
        };
        win.Show();
    }

    internal override void Refresh()
    {
        base.Refresh();
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(BackgroundBrush));
        OnPropertyChanged(nameof(ForegroundColor));
        OnPropertyChanged(nameof(ForegroundBrush));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(HideLine));
        OnPropertyChanged(nameof(Not));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Category));
        OnPropertyChanged(nameof(Active));
        OnPropertyChanged(nameof(Label));
        OnPropertyChanged(nameof(Bold));
        OnPropertyChanged(nameof(Italic));
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Title))
            return Title;
        StringBuilder sb = new(Text.Split(Environment.NewLine.ToCharArray())[0]);
        if (SubRules.Count > 0)
            foreach (OneSubRule subRule in SubRules)
                sb.Append(subRule.ToString());
        return sb.ToString();
    }
}

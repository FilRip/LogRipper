using System.Windows;

using LogRipper.Constants;
using LogRipper.Models;
using LogRipper.ViewModels;

namespace LogRipper.Controls;

/// <summary>
/// Code behind for RuleWindow.xaml
/// </summary>
public partial class SubRuleWindow : Window
{
    public SubRuleWindow(RuleWindowViewModel rule)
    {
        InitializeComponent();

        if (Properties.Settings.Default.SubRulePosX != 0)
            Left = Properties.Settings.Default.SubRulePosX;
        if (Properties.Settings.Default.SubRulePosY != 0)
            Top = Properties.Settings.Default.SubRulePosY;

        MyDataContext.SetCloseAction(Close);
        MyDataContext.SetParentRule(rule);
    }

    internal void LoadSubRule(OneSubRule subRule)
    {
        MyDataContext.CaseSensitive = subRule.CaseSensitive;
        MyDataContext.SelectedConcatenation = (subRule.Concatenation == Concatenation.AND ? Locale.CONCATENATION_AND : Locale.CONCATENATION_OR);
        MyDataContext.Condition = ConditionsManager.ConditionEnumToString(subRule.Conditions);
        MyDataContext.Text = subRule.Text;
        Title = Locale.TITLE_EDIT_RULE;
        BtnOK.Text = Locale.BTN_EDIT_RULE;
        MyDataContext.SetModify();
    }

    internal SubRuleWindowViewModel MyDataContext
    {
        get { return (SubRuleWindowViewModel)DataContext; }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DialogResult = MyDataContext.DialogResult;
        Properties.Settings.Default.SubRulePosX = Left;
        Properties.Settings.Default.SubRulePosY = Top;
        Properties.Settings.Default.Save();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
            Close();
    }
}

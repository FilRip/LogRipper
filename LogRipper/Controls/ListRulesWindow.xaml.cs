using System;
using System.Windows;

using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.ViewModels;
using LogRipper.Windows;

namespace LogRipper.Controls;

/// <summary>
/// Code behind of ListRulesWindow.xaml
/// </summary>
public partial class ListRulesWindow : Window
{
    public ListRulesWindow()
    {
        InitializeComponent();

        if (Properties.Settings.Default.ListRulesPosX != 0)
            Left = Properties.Settings.Default.ListRulesPosX;
        if (Properties.Settings.Default.ListRulesPosY != 0)
            Top = Properties.Settings.Default.ListRulesPosY;
    }

    internal ListRulesWindowViewModel MyDataContext
    {
        get { return (ListRulesWindowViewModel)DataContext; }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Properties.Settings.Default.ListRulesPosX = Left;
        Properties.Settings.Default.ListRulesPosY = Top;
        Properties.Settings.Default.Save();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
            Close();
    }

    private void ListRulesToManage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        MyDataContext.SelectedItems.Clear();
        foreach (OneRule rule in ListRulesToManage.SelectedItems)
            MyDataContext.SelectedItems.Add(rule);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
    }
}

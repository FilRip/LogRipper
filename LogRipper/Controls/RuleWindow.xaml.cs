using System;
using System.Windows;

using LogRipper.ViewModels;

namespace LogRipper.Controls;

/// <summary>
/// Code behind for RuleWindow.xaml
/// </summary>
public partial class RuleWindow : Window
{
    public RuleWindow()
    {
        InitializeComponent();

        if (Properties.Settings.Default.RulePosX != 0)
            Left = Properties.Settings.Default.RulePosX;
        if (Properties.Settings.Default.RulePosY != 0)
            Top = Properties.Settings.Default.RulePosY;
        MyDataContext.SetCloseAction(Close);
    }

    internal RuleWindowViewModel MyDataContext
    {
        get { return (RuleWindowViewModel)DataContext; }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DialogResult = MyDataContext.DialogResult;
        Properties.Settings.Default.RulePosX = Left;
        Properties.Settings.Default.RulePosY = Top;
        Properties.Settings.Default.Save();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
            Close();
    }
}

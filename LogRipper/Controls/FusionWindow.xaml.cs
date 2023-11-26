using System;
using System.Windows;

using LogRipper.ViewModels;

namespace LogRipper.Controls;

/// <summary>
/// Code behind for FusionWindow.xaml
/// </summary>
public partial class FusionWindow : Window
{
    public FusionWindow()
    {
        InitializeComponent();

        if (Properties.Settings.Default.MergePosX != 0)
            Left = Properties.Settings.Default.MergePosX;
        if (Properties.Settings.Default.MergePosY != 0)
            Top = Properties.Settings.Default.MergePosY;
        if (Properties.Settings.Default.MergeSizeX != 0)
            Width = Properties.Settings.Default.MergeSizeX;
        if (Properties.Settings.Default.MergeSizeY != 0)
            Height = Properties.Settings.Default.MergeSizeY;
        if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.MergeWS) && Enum.TryParse(Properties.Settings.Default.MergeWS, out WindowState newState))
            WindowState = newState;
    }

    internal FusionWindowViewModel MyDataContext
    {
        get { return (FusionWindowViewModel)DataContext; }
    }

    private void TextBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (!MyDataContext.IsFileReadOnly)
            MyDataContext.BrowseFilenameCommand.Execute(null);
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Properties.Settings.Default.MergePosX = Left;
        Properties.Settings.Default.MergePosY = Top;
        Properties.Settings.Default.MergeSizeX = Width;
        Properties.Settings.Default.MergeSizeY = Height;
        Properties.Settings.Default.MergeWS = WindowState.ToString("G");
        Properties.Settings.Default.Save();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (MyDataContext.IsFileReadOnly)
        {
            TxtFileName.SelectionStart = TxtFileName.Text.Length;
        }
    }
}

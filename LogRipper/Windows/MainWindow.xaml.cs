using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using LogRipper.Constants;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.ViewModels;

using ModernWpf;
using ModernWpf.Controls;

namespace LogRipper.Windows;

/// <summary>
/// Code behind for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // TODO : Tabs to open multiples files in differents space
    // TODO : Add possibility to wrap lines
    // TODO : Make a scrollbar colored by matching
    // TODO : Remove some Modal window
    // TODO : Make docking/float window/toolbar
    public MainWindow()
    {
        InitializeComponent();
        Icon = Imaging.CreateBitmapSourceFromHIcon(Properties.Resources.MainIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        TitleBar.SetIsIconVisible(this, true);

        if (Properties.Settings.Default.PosX != 0)
            Left = Properties.Settings.Default.PosX;
        if (Properties.Settings.Default.PosY != 0)
            Top = Properties.Settings.Default.PosY;
        if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.WindowState) && Enum.TryParse(Properties.Settings.Default.WindowState, out WindowState newState))
        {
            WindowState = newState;
            if (newState == WindowState.Normal)
            {
                if (Properties.Settings.Default.SizeX != 0)
                    Width = Properties.Settings.Default.SizeX;
                if (Properties.Settings.Default.SizeY != 0)
                    Height = Properties.Settings.Default.SizeY;
            }
        }

        if (Properties.Settings.Default.Theme == Locale.THEME_DARK)
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
        else if (Properties.Settings.Default.Theme == Locale.THEME_LIGHT)
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
        XamlControlsResources xcr = Application.Current.Resources.MergedDictionaries.OfType<XamlControlsResources>().FirstOrDefault();
        if (xcr != null)
            xcr.UseCompactResources = true;

        try
        {
            if (Environment.GetCommandLineArgs().Length > 1 && !Environment.GetCommandLineArgs()[1].StartsWith("/r=", StringComparison.CurrentCultureIgnoreCase))
            {
                _ = MyDataContext.OpenNewFile(Environment.GetCommandLineArgs()[1]);
            }
        }
        catch (Exception ex)
        {
            WpfMessageBox.ShowModal(string.Format(Locale.ERROR_READING_FILE, Environment.GetCommandLineArgs()[1]) + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
        }
        string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        if (version.EndsWith(".0.0"))
            version = version.Substring(0, version.Length - 4);
        Title += version;
    }

    internal MainWindowViewModel MyDataContext
    {
        get { return (MainWindowViewModel)DataContext; }
    }

    internal void ScrollToBegin()
    {
        ListBoxLines?.GetScrollViewer()?.ScrollToTop();
    }

    internal void ScrollToSelected()
    {
        if (MyDataContext.SelectedLine != null)
        {
            ListBoxLines.ScrollIntoView(MyDataContext.SelectedLine);
            if (MyDataContext.EnableShowMargin && MyDataContext.AutoFollowInMargin)
                ListBoxMargin.ScrollIntoView(MyDataContext.SelectedLine);
        }
    }

    private void ListBoxMargin_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is OneLine curLine)
        {
            MyDataContext.SelectedLine = curLine;
            ScrollToSelected();
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Properties.Settings.Default.PosX = Left;
        Properties.Settings.Default.PosY = Top;
        if (WindowState == WindowState.Normal)
        {
            Properties.Settings.Default.SizeX = Width;
            Properties.Settings.Default.SizeY = Height;
        }
        Properties.Settings.Default.WindowState = WindowState.ToString("G");
        Properties.Settings.Default.Save();
    }

    private void ListBoxLines_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        int nbStart = (int)e.VerticalOffset + 1;
        int nbDisplay = (int)e.ViewportHeight + 1;
        MyDataContext.SetVisibleLine(nbStart, nbDisplay);
    }

    internal void ScrollToEnd()
    {
        if (MyDataContext.EnableAutoScrollToEnd)
            ListBoxLines.GetScrollViewer()?.ScrollToEnd();
    }

    internal void RefreshMargin()
    {
        Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            MyDataContext.ActiveProgressRing = true;
            if (MyDataContext?.EnableShowMargin == false && ListBoxMargin.ItemsSource != null)
            {
                ListBoxMargin.ItemsSource = null;
            }
            else if (MyDataContext?.EnableShowMargin == true)
            {
                ListBoxMargin.ItemsSource ??= MyDataContext.FilteredListLines;
                await MyDataContext.RefreshListLines();
            }
            MyDataContext.ActiveProgressRing = false;
        }, DispatcherPriority.Background);
    }

    internal void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Visibility == Visibility.Visible)
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (ToolbarRow.ActualHeight > ToolbarTitleRow.ActualHeight)
                {
                    // We must set a real size to have the scrollbar
                    double sizeToolBar = Math.Max(ToolbarRow.ActualHeight - ToolbarTitleRow.ActualHeight - 15, 35);
                    if (!MyDataContext.EnableShowToolbar)
                        sizeToolBar = 0;
                    ListFilesRow.MaxHeight = sizeToolBar;
                    ListFilesRow.Height = new GridLength(ListFilesRow.MaxHeight);
                    ListRulesRow.MaxHeight = sizeToolBar;
                    ListRulesRow.Height = new GridLength(ListRulesRow.MaxHeight);
                    ListCategoryRow.MaxHeight = sizeToolBar;
                    ListCategoryRow.Height = new GridLength(ListCategoryRow.MaxHeight);

                    if (Properties.Settings.Default.WrapLines)
                    {
                        ColumnLines.MaxWidth = ActualWidth - 10 - ColumnFileName.ActualWidth - ColumnNumLine.ActualWidth - 10;
                        if (MyDataContext.EnableShowMargin)
                            ColumnLines.MaxWidth = ColumnLines.MaxWidth - 10 - ListBoxMargin.Width;
                    }
                    else if (ColumnLines.MaxWidth != double.PositiveInfinity)
                        ColumnLines.MaxWidth = double.PositiveInfinity;
                }
            }, DispatcherPriority.Background);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        for (int i = 1; i <= Environment.GetCommandLineArgs().Length - 1; i++)
            if (Environment.GetCommandLineArgs()[i].Trim().ToLower().StartsWith("/r="))
            {
                _ = MyDataContext.LoadRules(Environment.GetCommandLineArgs()[i].Trim().ToLower().Replace("/r=", ""));
                return;
            }
        if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultListRules) &&
            File.Exists(Properties.Settings.Default.DefaultListRules))
        {
            _ = MyDataContext.LoadRules(Properties.Settings.Default.DefaultListRules);
        }
        AutoUpdater.SearchNewVersion(Properties.Settings.Default.beta);
    }

    private void TxtFile_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        OneFile file = (OneFile)((TextBlock)sender).DataContext;
        file.Active = !file.Active;
    }

    private void TxtRule_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        OneRule rule = (OneRule)((TextBlock)sender).DataContext;
        rule.Active = !rule.Active;
    }

    private void TxtCategory_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        OneCategory category = (OneCategory)((TextBlock)sender).DataContext;
        category.Active = !category.Active;
    }

    private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Window_SizeChanged(sender, e);
    }

    private void ListBoxLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        MyDataContext.SelectedLines = ListBoxLines.SelectedItems.OfType<OneLine>();
    }

    private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textbox)
        {
            if (!string.IsNullOrWhiteSpace(textbox.SelectedText) &&
                textbox.SelectedText[textbox.SelectedText.Length - 1] == ' ')

                textbox.SelectionLength--;

            MyDataContext.SetTextSelected(textbox.SelectedText);
        }
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (Properties.Settings.Default.MinimizeInSystray && WindowState == WindowState.Minimized)
            Hide();
    }

    private void ListBoxLines_Drop(object sender, DragEventArgs e)
    {
        try
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files?.Length > 0)
            {
                MessageBoxResult ret;
                if (MyDataContext.ListFiles?.Count > 0)
                    ret = WpfMessageBox.ShowModalReturnButton(Locale.ASK_MERGE_OR_REPLACE, Locale.MENU_OPEN.Replace("_", ""), MessageBoxButton.YesNo);
                else
                    ret = MessageBoxResult.No;
                if (ret == MessageBoxResult.Yes)
                {
                    _ = MyDataContext.MergingFile(files);
                }
                else if (ret == MessageBoxResult.No)
                {
                    _ = MyDataContext.OpenNewFile(files[0]);
                    if (files.Length > 1)
                    {
                        List<string> otherFiles = files.ToList();
                        otherFiles.RemoveAt(0);
                        _ = MyDataContext.MergingFile(otherFiles.ToArray());
                    }
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }

    private void ListRules_Drop(object sender, DragEventArgs e)
    {
        try
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files?.Length > 0)
            {
                MessageBoxResult ret = WpfMessageBox.ShowModalReturnButton(Locale.ASK_MERGE_OR_REPLACE, Locale.MENU_OPEN.Replace("_", ""), MessageBoxButton.YesNo);
                if (ret == MessageBoxResult.Yes)
                {
                    _ = MyDataContext.MergingRule(files);
                }
                else if (ret == MessageBoxResult.No)
                {
                    _ = MyDataContext.LoadRules(files[0]);
                }
            }
        }
        catch (Exception) { /* Ignore errors */ }
    }
}

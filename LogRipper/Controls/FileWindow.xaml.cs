using System.Windows;

using LogRipper.Constants;
using LogRipper.Helpers;
using LogRipper.ViewModels;

namespace LogRipper.Controls;

/// <summary>
/// Logique d'interaction pour FileWindow.xaml
/// </summary>
public partial class FileWindow : Window
{
    public FileWindow()
    {
        InitializeComponent();

        if (Properties.Settings.Default.FilePosX != 0)
            Left = Properties.Settings.Default.FilePosX;
        if (Properties.Settings.Default.FilePosY != 0)
            Top = Properties.Settings.Default.FilePosY;
        if (Properties.Settings.Default.FileSizeX != 0)
            Width = Properties.Settings.Default.FileSizeX;
        if (Properties.Settings.Default.FileSizeY != 0)
            Height = Properties.Settings.Default.FileSizeY;

        TxtEncoding.Text = Locale.MENU_ENCODING.Replace("_", "");
    }

    internal FileWindowViewModel MyDataContext
    {
        get { return (FileWindowViewModel)DataContext; }
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        if (!FileManager.CheckDateFormat(MyDataContext.FormatDate, MyDataContext.FirstLine))
            return;
        DialogResult = true;
        Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Properties.Settings.Default.FilePosX = Left;
        Properties.Settings.Default.FilePosY = Top;
        Properties.Settings.Default.FileSizeX = Width;
        Properties.Settings.Default.FileSizeY = Height;
        Properties.Settings.Default.Save();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
        {
            DialogResult = false;
            Close();
        }
    }
}

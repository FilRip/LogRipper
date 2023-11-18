using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Hardcodet.Wpf.TaskbarNotification;

using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.ViewModels;

public partial class NotifyIconViewModel : ObservableObject
{
    [ObservableProperty()]
    private string _systrayIcon;
    internal static NotifyIconViewModel Instance { get; private set; }
    private TaskbarIcon _notifyIcon;

    public NotifyIconViewModel() : base()
    {
        Instance = this;
    }

    internal TaskbarIcon SystrayControl
    {
        get { return _notifyIcon; }
    }

    internal void SetIcon(string iconResource = null, bool defaultIcon = true)
    {
        if (Properties.Settings.Default.ShowInSystray)
            SystrayIcon = (defaultIcon ? "/Resources/icon.ico" : iconResource);
    }

    [RelayCommand()]
    private void ExitApplication()
    {
        Application.Current.GetCurrentWindow<MainWindow>().Close();
    }

    internal void RemoveIcon()
    {
        _notifyIcon.Dispose();
    }

    internal void SetControl(TaskbarIcon myControl)
    {
        _notifyIcon = myControl;
        SetIcon();
    }

    [RelayCommand()]
    private void ShowWindow()
    {
        MainWindow win = Application.Current.GetCurrentWindow<MainWindow>();
        win.Show();
        if (win.WindowState == WindowState.Minimized)
            win.WindowState = WindowState.Normal;
        win.Activate();
        win.Focus();
    }
}

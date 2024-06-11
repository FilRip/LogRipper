using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

using LogRipper.Helpers;
using LogRipper.ViewModels;

namespace LogRipper
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;

        public App()
        {
            InitializeComponent();
            if (!LogRipper.Properties.Settings.Default.ConfigReloaded)
            {
                // Reload previous config (if after an update)
                ReloadOlderConfig.SearchAndReload();
                LogRipper.Properties.Settings.Default.ConfigReloaded = true;
                LogRipper.Properties.Settings.Default.Save();
            }
            // If no language, try to set language of current windows culture
            if (string.IsNullOrWhiteSpace(LogRipper.Properties.Settings.Default.Language))
                LogRipper.Properties.Settings.Default.Language = System.Globalization.CultureInfo.CurrentCulture.Name;
            // Init translation
            Constants.Locale.Init();
            // If no theme, select the same as windows
            if (string.IsNullOrWhiteSpace(LogRipper.Properties.Settings.Default.Theme))
                LogRipper.Properties.Settings.Default.Theme = "Windows";
            // Init theme
            Constants.Colors.LoadTheme();
            // Init const
            Constants.Values.Init();
            // Init/load icons
            Constants.Icons.Init();
            // If no color selected in settings, set them in terms of current theme
            if (LogRipper.Properties.Settings.Default.DefaultForegroundColor == System.Drawing.Color.Transparent)
            {
                LogRipper.Properties.Settings.Default.DefaultBackgroundColor = System.Drawing.Color.FromArgb(Constants.Colors.BackgroundColor.R,
                                                                                                             Constants.Colors.BackgroundColor.G,
                                                                                                             Constants.Colors.BackgroundColor.B);
                LogRipper.Properties.Settings.Default.DefaultForegroundColor = System.Drawing.Color.FromArgb(Constants.Colors.ForegroundColor.R,
                                                                                                             Constants.Colors.ForegroundColor.G,
                                                                                                             Constants.Colors.ForegroundColor.B);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            NativeMethods.FreeConsole();
            NotifyIconViewModel.Instance?.RemoveIcon();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (LogRipper.Properties.Settings.Default.ShowInSystray)
            {
                _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
                NotifyIconViewModel.Instance.SetControl(_notifyIcon);
            }
        }
    }
}

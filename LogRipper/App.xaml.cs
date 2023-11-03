using System.Windows;

namespace LogRipper
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Constants.Locale.Init();
            if (string.IsNullOrWhiteSpace(LogRipper.Properties.Settings.Default.Theme))
                LogRipper.Properties.Settings.Default.Theme = "Windows";
            Constants.Colors.LoadTheme();
            Constants.Values.Init();
            Constants.Icons.Init();
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
    }
}

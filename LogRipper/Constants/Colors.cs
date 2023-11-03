using System.Windows.Media;

using LogRipper.Helpers;

using ModernWpf;

namespace LogRipper.Constants
{
    internal static class Colors
    {
        public static Color AccentColor { get; private set; }
        public static Color BackgroundColor { get; private set; }
        public static Color ForegroundColor { get; private set; }
        public static SolidColorBrush AccentColorBrush { get; private set; }
        public static SolidColorBrush BackgroundColorBrush { get; private set; }
        public static SolidColorBrush ForegroundColorBrush { get; private set; }

        public static void LoadTheme()
        {
            AccentColor = ThemeManager.Current.ActualAccentColor;
            AccentColorBrush = new SolidColorBrush(AccentColor);
            if (WindowsSettings.IsWindowsApplicationInDarkMode())
            {
                BackgroundColor = Brushes.Black.Color;
                ForegroundColor = Brushes.White.Color;
            }
            else
            {
                BackgroundColor = Brushes.White.Color;
                ForegroundColor = Brushes.Black.Color;
            }
            BackgroundColorBrush = new SolidColorBrush(BackgroundColor);
            ForegroundColorBrush = new SolidColorBrush(ForegroundColor);
        }

        public static Color DefaultBackgroundColor
        {
            get
            {
                return Color.FromRgb(Properties.Settings.Default.DefaultBackgroundColor.R,
                                     Properties.Settings.Default.DefaultBackgroundColor.G,
                                     Properties.Settings.Default.DefaultBackgroundColor.B);
            }
        }

        public static Color DefaultForegroundColor
        {
            get
            {
                return Color.FromRgb(Properties.Settings.Default.DefaultForegroundColor.R,
                                     Properties.Settings.Default.DefaultForegroundColor.G,
                                     Properties.Settings.Default.DefaultForegroundColor.B);
            }
        }
    }
}

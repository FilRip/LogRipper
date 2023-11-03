using System.Windows;

namespace LogRipper.Constants
{
    internal static class Values
    {
        public static readonly GridLength DefaultRowHeightSpace = new(6);

        public static double FontSize { get; set; } = 14;
        public static int SpaceSize { get; set; } = 2;

        public static double RowHeight
        {
            get { return FontSize + SpaceSize; }
        }

        public static TextWrapping WrapLines
        {
            get { return (Properties.Settings.Default.WrapLines ? TextWrapping.Wrap : TextWrapping.NoWrap); }
        }

        internal static void Init()
        {
            if (Properties.Settings.Default.FontSize > 0)
                FontSize = Properties.Settings.Default.FontSize;
            if (Properties.Settings.Default.SpaceSize > 0)
                SpaceSize = Properties.Settings.Default.SpaceSize;
            if (SpaceSize < 2)
                SpaceSize = 2;
        }
    }
}

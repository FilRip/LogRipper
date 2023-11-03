using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogRipper.Helpers
{
    internal static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        internal static extern bool DwmSetWindowAttribute(IntPtr hwnd, int attribut, ref int attrValeur, int attrSize);

        internal enum PreferredAppMode
        {
            APPMODE_DEFAULT = 0,
            APPMODE_ALLOWDARK = 1,
            APPMODE_FORCEDARK = 2,
            APPMODE_FORCELIGHT = 3,
            APPMODE_MAX = 4
        }

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        internal static ImageSource GetIconFromFile(string filename, int index)
        {
            ExtractIconEx(filename, index, out IntPtr largeIcon, out IntPtr _, 1);
            if (largeIcon != IntPtr.Zero)
                return Imaging.CreateBitmapSourceFromHIcon(largeIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return null;
        }
    }
}

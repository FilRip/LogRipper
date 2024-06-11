using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using static LogRipper.Helpers.ConsoleHelper;

namespace LogRipper.Helpers;

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

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FreeConsole();

    [DllImport("kernel32", SetLastError = true)]
    internal static extern IntPtr GetStdHandle(int num);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    [return: MarshalAs(UnmanagedType.Bool)] //   ̲┌──────────────────^
    internal static extern bool ReadConsoleOutputCharacterA(
      IntPtr hStdout,   // result of 'GetStdHandle(-11)'
      out byte ch,      // A̲N̲S̲I̲ character result
      uint c_in,        // (set to '1')
      uint coord_XY,    // screen location to read, X:loword, Y:hiword
      out uint c_out);  // (unwanted, discard)

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)] //   ̲┌───────────────────^
    internal static extern bool ReadConsoleOutputCharacterW(
        IntPtr hStdout,   // result of 'GetStdHandle(-11)'
        out Char ch,      // U̲n̲i̲c̲o̲d̲e̲ character result
        uint c_in,        // (set to '1')
        uint coord_XY,    // screen location to read, X:loword, Y:hiword
        out uint c_out);  // (unwanted, discard) 

    [StructLayout(LayoutKind.Sequential)]
    internal struct ConsoleScreenBufferInfo
    {
        public Coord dwSize;
        public Coord dwCursorPosition;
        public short wAttributes;
        public SmallRect srWindow;
        public Coord dwMaximumWindowSize;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool GetConsoleScreenBufferInfo(
        IntPtr hConsoleOutput,
        out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

    [DllImport("kernel32.dll")]
    internal static extern uint GetLastError();
}

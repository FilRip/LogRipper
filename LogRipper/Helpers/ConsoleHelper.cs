using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using static LogRipper.Helpers.NativeMethods;

namespace LogRipper.Helpers;

public static class ConsoleHelper
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Coord
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }
    public const int STD_OUTPUT_HANDLE = -11;
    public const long INVALID_HANDLE_VALUE = -1;

    /// <summary>
    /// Get current cursor position from console window.
    /// In .Net 5 > use Console.GetCursorPosition
    /// </summary>
    /// <returns>Cursor position</returns>
    internal static Coord GetCursorPosition()
    {
        // Get a handle for the console
        IntPtr stdout = GetStdHandle(STD_OUTPUT_HANDLE);

        // In .net 5 there's Console.GetCursorPosition for this
        return GetConsoleInfo(stdout).dwCursorPosition;
    }

    /// <summary>
    /// Retrieves information about the current screen buffer window
    /// </summary>
    internal static ConsoleScreenBufferInfo GetConsoleInfo()
    {
        // Get a handle for the console
        IntPtr stdout = GetStdHandle(STD_OUTPUT_HANDLE);
        return GetConsoleInfo(stdout);
    }

    internal static ConsoleScreenBufferInfo GetConsoleInfo(IntPtr ptr)
    {
        if (!GetConsoleScreenBufferInfo(ptr, out ConsoleScreenBufferInfo outInfo))
            throw new Win32Exception((int)GetLastError(), ((Win32Error)GetLastError()).ToString("G"));
        return outInfo;
    }

    /// <summary>
    /// Find text in console window
    /// </summary>
    /// <param name="text">Text to search</param>
    /// <returns>List of found coordinates</returns>
    internal static List<Coord> IndexOfInConsole(string text)
    {
        return IndexOfInConsole([text]);
    }

    /// <summary>
    /// Find texts in console window
    /// </summary>
    /// <param name="text">Text to search</param>
    /// <returns>List of found coordinates</returns>
    internal static List<Coord> IndexOfInConsole(string[] text)
    {
        List<Coord> coords = [];

        // Get a handle for the console
        IntPtr stdout = GetStdHandle(STD_OUTPUT_HANDLE);

        // Get Console Info
        var consoleInfo = GetConsoleInfo(stdout);

        for (short y = 0; y < consoleInfo.dwCursorPosition.Y; y += 1)
        {
            var line = GetText(0, y, stdout);

            // Search through the line and put the results in coords
            foreach (var t in text)
            {
                var xPos = 0;
                while (true)
                {
                    var pos = line.IndexOf(t, xPos);
                    if (pos == -1)
                        break;
                    coords.Add(new Coord() { X = (short)pos, Y = y });
                    xPos = pos + 1;
                }
            }
        }
        return coords;
    }

    /// <summary>
    /// Retrieve character from console window
    /// </summary>
    /// <param name="x">Number of column</param>
    /// <param name="y">Number of line</param>
    /// <returns>One character</returns>
    internal static char GetChar(short x, short y)
    {
        // Get a handle for the console
        IntPtr stdout = GetStdHandle(STD_OUTPUT_HANDLE);

        // Call it with a pointer
        return GetChar(x, y, stdout);
    }

    /// <summary>
    /// Retrieve character from console window
    /// </summary>
    /// <param name="x">Number of column</param>
    /// <param name="y">Number of line</param>
    /// <param name="ptr">Pointer to console handle</param>
    /// <returns>One character</returns>
    internal static char GetChar(short x, short y, IntPtr ptr)
    {
        // Convert to coord and call it
        return GetChar(new Coord() { X = x, Y = y }, ptr);
    }

    /// <summary>
    /// Retrieve character from console window
    /// </summary>
    /// <param name="coordinate">Coordinate of char to retrieve (on X and Y)</param>
    /// <param name="ptr">Pointer to console handle</param>
    /// <returns>One character</returns>
    internal static char GetChar(Coord coordinate, IntPtr ptr)
    {
        // Convert the coordinates to a uint containing both
        uint coord = (uint)coordinate.X;
        coord |= (uint)coordinate.Y << 16;

        if (!ReadConsoleOutputCharacterW(
                    ptr,
                    out char chUnicode, // result: single Unicode char
                    1,                  // # of chars to read
                    coord,              // (X,Y) screen location to read (see above)
                    out _))             // result: actual # of chars (unwanted)
            throw new Win32Exception((int)GetLastError(), ((Win32Error)GetLastError()).ToString("G"));
        return chUnicode;
    }

    /// <summary>
    /// Retrieve text from console window
    /// </summary>
    /// <param name="x">Number of column</param>
    /// <param name="y">Number of line</param>
    /// <returns>All text on the line until end</returns>
    internal static string GetText(short x, short y)
    {
        // Get a handle for the console
        IntPtr stdout = GetStdHandle(STD_OUTPUT_HANDLE);

        // Let's call it with a console pointer
        return GetText(x, y, stdout);
    }

    /// <summary>
    /// Retrieve text from console window
    /// </summary>
    /// <param name="x">Number of column</param>
    /// <param name="y">Number of line</param>
    /// <param name="length">Number of char to retrieve</param>
    /// <returns>The specified text on the line</returns>
    internal static string GetText(short x, short y, short length)
    {
        // Get a handle for the console
        IntPtr stdout = GetStdHandle(STD_OUTPUT_HANDLE);

        // Let's call it with a console pointer
        return GetText(x, y, length, stdout);
    }

    /// <summary>
    /// Retrieve text from console window
    /// </summary>
    /// <param name="x">Number of column</param>
    /// <param name="y">Number of line</param>
    /// <param name="ptr">Pointer to console handle</param>
    /// <returns>All text on the line until end</returns>
    internal static string GetText(short x, short y, IntPtr ptr)
    {
        // Get Console Info
        ConsoleScreenBufferInfo consoleInfo = GetConsoleInfo(ptr);

        // Let's call it with the remaining bit of the x screen buffer
        return GetText(x, y, (short)(consoleInfo.dwSize.X - x), ptr);
    }

    /// <summary>
    /// Retrieve text from console window
    /// </summary>
    /// <param name="x">Number of column</param>
    /// <param name="y">Number of line</param>
    /// <param name="length">Number of char to retrieve</param>
    /// <param name="ptr">Pointer to console handle</param>
    /// <returns>The specified text on the line</returns>
    internal static string GetText(short x, short y, short length, IntPtr ptr)
    {
        // Convert to coord and call it
        return GetText(new Coord() { X = x, Y = y }, length, ptr);
    }

    /// <summary>
    /// Retrieve text from console window
    /// </summary>
    /// <param name="coordinate">coordinate of start of text to retrieve</param>
    /// <param name="length">Liength of text to retrieve</param>
    /// <returns>The specified text on the line</returns>
    internal static string GetText(Coord coordinate, int length, IntPtr ptr)
    {
        StringBuilder text = new();
        for (short x = coordinate.X; x < coordinate.X + length; x += 1)
            text.Append(GetChar(x, coordinate.Y, ptr));
        return text.ToString();
    }
}

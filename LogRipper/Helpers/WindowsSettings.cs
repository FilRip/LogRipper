using System;

using LogRipper.Constants;

using Microsoft.Win32;

namespace LogRipper.Helpers;

public static class WindowsSettings
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

    public static bool IsWindowsApplicationInDarkMode()
    {
        if (Properties.Settings.Default.Theme == Locale.THEME_DARK)
            return true;
        else if (Properties.Settings.Default.Theme == Locale.THEME_LIGHT)
            return false;
        try
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            if (reg != null)
            {
                int lightMode = int.Parse(reg.GetValue("AppsUseLightTheme").ToString());
                return (lightMode == 0);
            }
        }
        catch (Exception) { /* Ignore errors */ }
        return false;
    }

    public static bool IsWindows10OrGreater(int build = 0)
    {
        return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
    }

    public static bool IsWindows11OrGreater(int build = 22000)
    {
        if (build < 22000)
            return false;
        return IsWindows10OrGreater(build);
    }

    public static bool UseImmersiveDarkMode(IntPtr pointeurFenetre, bool activerDarkMode)
    {
        if (IsWindows10OrGreater())
        {
            int attribut = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
            if (IsWindows10OrGreater(18985))
            {
                attribut = DWMWA_USE_IMMERSIVE_DARK_MODE;
            }
            int immersiveActif = (activerDarkMode ? 1 : 0);
            NativeMethods.SetPreferredAppMode(activerDarkMode ? NativeMethods.PreferredAppMode.APPMODE_ALLOWDARK : NativeMethods.PreferredAppMode.APPMODE_DEFAULT);
            return NativeMethods.DwmSetWindowAttribute(pointeurFenetre, attribut, ref immersiveActif, IntPtr.Size);
        }
        return false;
    }
}

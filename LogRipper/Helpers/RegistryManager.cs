using System;

using LogRipper.Constants;

using Microsoft.Win32;

namespace LogRipper.Helpers
{
    internal static class RegistryManager
    {
        private const string REGISTRY_LOCATION = "Software\\Classes\\*\\shell\\LogRipper";

        internal static bool AlreadyPresent()
        {
            return Registry.CurrentUser.OpenSubKey(REGISTRY_LOCATION, false) != null;
        }

        internal static void SetRegistry(bool remove)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(REGISTRY_LOCATION, false);
            if (reg != null)
                Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_LOCATION);
            if (remove)
                return;
            reg = Registry.CurrentUser.CreateSubKey(REGISTRY_LOCATION, true);
            reg.SetValue(null, Locale.EXPLORER_OPEN_WITH);
            reg.SetValue("icon", "\"" + Environment.GetCommandLineArgs()[0] + "\"");
            reg = reg.CreateSubKey("command");
            reg.SetValue(null, "\"" + Environment.GetCommandLineArgs()[0] + "\" \"%1\"");
            reg.Close();
        }
    }
}

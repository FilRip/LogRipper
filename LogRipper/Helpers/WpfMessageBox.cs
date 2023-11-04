using System;
using System.Windows;

namespace LogRipper.Helpers
{
    internal static class WpfMessageBox
    {
        public static bool ShowModal(string text, string title, MessageBoxButton boutons = MessageBoxButton.OK, Window parentWindow = null)
        {
            text = text.Replace(@"\r\n", Environment.NewLine).Replace(@"\r", Environment.NewLine).Replace(@"\n", Environment.NewLine);
            parentWindow ??= Application.Current.GetCurrentWindow();
            MessageBoxResult ret = MessageBoxResult.None;
            if (parentWindow != null)
            {
                parentWindow.Dispatcher.Invoke(() =>
                {
                    ret = MessageBox.Show(parentWindow, text, title, boutons);
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ret = MessageBox.Show(text, title, boutons);
                });
            }
            return (ret == MessageBoxResult.OK || ret == MessageBoxResult.Yes);
        }
    }
}

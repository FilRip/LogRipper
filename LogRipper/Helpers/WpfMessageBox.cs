using System;
using System.Windows;

namespace LogRipper.Helpers;

internal static class WpfMessageBox
{
    public static bool ShowModal(string text, string title, MessageBoxButton buttons = MessageBoxButton.OK, Window parentWindow = null)
    {
        MessageBoxResult ret = ShowModalReturnButton(text, title, buttons, parentWindow);
        return ret == MessageBoxResult.OK || ret == MessageBoxResult.Yes;
    }

    public static MessageBoxResult ShowModalReturnButton(string text, string title, MessageBoxButton buttons = MessageBoxButton.OK, Window parentWindow = null)
    {
        text = text.Replace(@"\r\n", Environment.NewLine).Replace(@"\r", Environment.NewLine).Replace(@"\n", Environment.NewLine);
        parentWindow ??= Application.Current.GetCurrentWindow();
        MessageBoxResult ret = MessageBoxResult.None;
        if (parentWindow != null)
        {
            parentWindow.Dispatcher.Invoke(() =>
            {
                ret = MessageBox.Show(parentWindow, text, title, buttons);
            });
        }
        else
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ret = MessageBox.Show(text, title, buttons);
            });
        }
        return ret;
    }
}

using System;
using System.Windows;

namespace LogRipper.Helpers;

internal static class MainDispatcher
{
    internal static void Run(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }

    internal static void RunAsync(Action action)
    {
        Application.Current.Dispatcher.BeginInvoke(action);
    }
}

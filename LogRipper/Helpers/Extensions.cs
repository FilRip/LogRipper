using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogRipper.Helpers
{
    internal static class Extensions
    {
        internal static Window GetCurrentWindow(this Application application)
        {
            Window ret = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ret = null;
                foreach (Window win in application.Windows)
                    if (win.IsActive)
                    {
                        ret = win;
                        break;
                    }
                ret ??= Application.Current.MainWindow;
            }));
            return ret;
        }

        internal static T GetCurrentWindow<T>(this Application application) where T : Window
        {
            T ret = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ret = null;
                foreach (T win in application.Windows.OfType<T>())
                    if (win.IsActive)
                    {
                        ret = win;
                        break;
                    }
                ret ??= application.Windows.OfType<T>().FirstOrDefault();
            }));
            return ret;
        }

        internal static ScrollViewer GetScrollViewer(this Control control)
        {
            if (VisualTreeHelper.GetChildrenCount(control) == 0)
                return null;
            var x = VisualTreeHelper.GetChild(control, 0);
            if (x == null)
                return null;
            if (VisualTreeHelper.GetChildrenCount(x) == 0)
                return null;
            return (ScrollViewer)VisualTreeHelper.GetChild(x, 0);
        }
    }
}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LogRipper.Converters;

internal class HideGridColumnConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean && boolean)
            return GridLength.Auto;
        else
            return new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

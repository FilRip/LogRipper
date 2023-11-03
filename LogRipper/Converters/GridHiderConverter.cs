using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LogRipper.Converters
{
    internal class GridHiderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean && boolean && parameter is string param)
            {
                double.TryParse(param.Replace("*", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator), out double percent);
                param = param.Trim().ToLower();
                if (param.Contains("*"))
                    return new GridLength(percent, GridUnitType.Star);
                else if (param.Contains("auto"))
                    return GridLength.Auto;
                return new GridLength(percent);
            }
            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

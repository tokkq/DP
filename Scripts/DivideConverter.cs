using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DailyProject_221204
{
    public class DivideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double.TryParse(value.ToString(), out var originValue);
            double.TryParse(parameter.ToString(), out var devideValue);
            var result = new Thickness(-(originValue / devideValue), 0, 0, 0);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double.TryParse(value.ToString(), out var originValue);
            double.TryParse(parameter.ToString(), out var devideValue);
            var result = new Thickness(-(originValue * devideValue), 0, 0, 0);
            return result;
        }
    }
}

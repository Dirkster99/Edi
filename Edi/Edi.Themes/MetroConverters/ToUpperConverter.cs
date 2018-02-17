using System;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class ToUpperConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string val ? val.ToUpper() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class ToLowerConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string val ? val.ToLower() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

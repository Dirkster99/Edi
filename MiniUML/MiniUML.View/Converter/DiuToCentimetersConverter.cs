namespace MiniUML.View.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  /// Converts between double-precision device independent units and centimeters.
  /// </summary>
  [ValueConversion(typeof(double), typeof(String))]
  public class DiuToCentimetersConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((double)value / 96 * 2.54).ToString("F", culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return double.Parse((string)value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint, culture) / 2.54 * 96;
    }
  }
}

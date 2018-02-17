namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// </summary>
  public class StringBoolToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Visibility.Collapsed;
        }

        if (value is string s)
        {
            if (s == "true")
                return Visibility.Visible;
            if (s == "false")
            {
                return Visibility.Collapsed;
            }
        }
        else if (value is bool _)
        {
            var b = (bool) value;

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value ?? string.Empty;
    }
  }
}

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
        return Visibility.Collapsed;

      if (value is string)
      {
        if (value != null)
        {
          string s = (string)value;

          if (s == "true")
            return Visibility.Visible;
          else
          {
            if (s == "false")
            {
              return Visibility.Collapsed;
            }
          }
        }
      }

      if (value is bool)
      {
        bool b = (bool)value;

        if (b == true)
          return Visibility.Visible;
        else
          return Visibility.Collapsed;
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return string.Empty;

      return value;
    }
  }
}

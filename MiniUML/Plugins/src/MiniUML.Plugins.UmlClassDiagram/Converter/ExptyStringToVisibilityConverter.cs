namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Converts an empty string to visibility.collapsed and vice verca.
  /// </summary>
  public class EmptyStringToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;


            if (value is string)
            {
                string s = value as string;

                if (s == string.Empty)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}

namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  /// Some class do not allow null values but empty strings. WPF excepts null if data items are not
  /// set. Therefore, as workaround, this converter translates emty strings into null, and vice versa.
  /// </summary>
  public class EmptyStringToNullConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;


            if (value is string)
            {
                string s = value as string;

                if (s == string.Empty)
                    return null;
            }

            return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return string.Empty;

      return value;
    }
  }
}

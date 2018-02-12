namespace FolderBrowser.Converters
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Converts a boolean value into a configurable
  /// value of type <seealso cref="Visibility"/>.
  /// 
  /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
  /// </summary>
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public sealed class BoolToVisibilityConverter : IValueConverter
  {
    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public BoolToVisibilityConverter()
    {
      // set defaults
      TrueValue = Visibility.Visible;
      FalseValue = Visibility.Collapsed;
    }
    #endregion constructor

    #region properties
    public Visibility TrueValue { get; set; }
    public Visibility FalseValue { get; set; }
    #endregion properties

    #region methods
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return null;
      return (bool)value ? TrueValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (Equals(value, TrueValue))
        return true;

      if (Equals(value, FalseValue))
        return false;

      return null;
    }
    #endregion methods
  }
}

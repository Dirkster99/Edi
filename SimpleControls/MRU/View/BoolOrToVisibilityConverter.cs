namespace SimpleControls.MRU.View
{
  using System;
  using System.Windows.Data;
  using System.Windows.Markup;

  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class BoolOrToVisibilityConverter : MarkupExtension, IMultiValueConverter
  {
    private static BoolOrToVisibilityConverter converter;

    /// <summary>
    /// Converter class
    /// </summary>
    public BoolOrToVisibilityConverter()
    {
    }

    #region IValueConverter Members
    /// <summary>
    /// When implemented in a derived class, returns an object that is provided
    /// as the value of the target property for this markup extension.
    /// 
    /// When a XAML processor processes a type node and member value that is a markup extension,
    /// it invokes the ProvideValue method of that markup extension and writes the result into the
    /// object graph or serialization stream. The XAML object writer passes service context to each
    /// such implementation through the serviceProvider parameter.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new BoolOrToVisibilityConverter();
      }

      return converter;
    }

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null)
        return Binding.DoNothing;

      if (values.Length != 2)
        return Binding.DoNothing;

      bool? bIsChecked = values[0] as bool?;
      bool? bIsMouseOver = values[1] as bool?;

      if (bIsChecked == null || bIsMouseOver == null)
        return Binding.DoNothing;

      if (bIsChecked == true)
        return System.Windows.Visibility.Visible;

      if (bIsMouseOver == true)
        return System.Windows.Visibility.Visible;

      return System.Windows.Visibility.Hidden;
    }

    /// <summary>
    /// Disabled convert back method (throws an exception upon being called)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetTypes"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}

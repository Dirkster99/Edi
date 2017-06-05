namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Windows.Data;
  using System.Windows.Media;

  [ValueConversion(typeof(string), typeof(ImageSource))]
  public class ImageUrlConverter : IValueConverter
  {
    #region constructor
    /// <summary>
    /// Static class constructor
    /// </summary>
    static ImageUrlConverter()
    {
      Instance = new ImageUrlConverter();
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get a static instance of this converter
    /// </summary>
    public static ImageUrlConverter Instance
    {
      get;
      private set;
    }
    #endregion properties

    #region methods
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return Binding.DoNothing;

      Uri imagePath = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
      ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

      return source;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion methods
  }
}

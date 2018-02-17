namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Media;

  public class PointCollectionConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null && (value.GetType() == typeof(ObservableCollection<Point>) && targetType == typeof(PointCollection)))
      {
        var pointCollection = new PointCollection();

        foreach (var point in (ObservableCollection<Point>) value)
          pointCollection.Add(point);

        return pointCollection;
      }

      return null;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null; // not needed
    }

    #endregion
  }
}

namespace MiniUML.View.Converter
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;
  using System.Xml.Linq;
  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.View.Controls;
  using MiniUML.View.Views;

  /// <summary>
  /// Converts a shape id into a shape control.
  /// </summary>
  public class ShapeIdToControlConverter : DependencyObject, IValueConverter
  {
    #region fields
    private static readonly DependencyProperty ReferenceControlProperty =
                            DependencyProperty.Register("ReferenceControl",
                                                        typeof(UIElement),
                                                        typeof(ShapeIdToControlConverter),
                                                        new FrameworkPropertyMetadata(null));
    #endregion fields

    #region properties
    public UIElement ReferenceControl
    {
      get { return (UIElement)GetValue(ReferenceControlProperty); }
      set { this.SetValue(ReferenceControlProperty, value); }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Convert shapeID of type <seealso cref="string"/> of a CanvasView control
    /// into a reference to a CanvasView <seealso cref="FrameworkElement"/>,
    /// The output <seealso cref="FrameworkElement"/> is located
    /// in the vicinity (below) the input control (AnchorPoint) represented by the shapeID.
    /// </summary>
    /// <param name="shapeID"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object shapeID, Type targetType, object parameter, CultureInfo culture)
    {
      object o = this.convert(shapeID, targetType, parameter, culture);

      if (o == null)
        o = AnchorPoint.InvalidSnapTarget;

      return o;
    }

    /// <summary>
    /// Convert given <seealso cref="UIElement"/> reference from <seealso cref="CanvasView"/>
    /// into shapeID of type <seealso cref="string"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
      UIElement control = value as UIElement;

      if (control == null)
        return string.Empty;

      CanvasView cv = CanvasView.GetCanvasView(control);

      return cv.ElementFromControl(control).ID;
    }

    private object convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string shape = value as string;

      // Convert invalid shapeID into invalid snap target
      if (shape == null || shape == string.Empty || this.ReferenceControl == null)
        return null;

      // Find canvas view through reference on this converter
      CanvasView cv = CanvasView.GetCanvasView(this.ReferenceControl);

      // No canvas view found
      if (cv == null)
        return null;

      // Get references element from viewmodel
      ShapeViewModelBase e = cv.CanvasViewModel.DocumentViewModel.dm_DocumentDataModel.GetShapeById(shape);

      // Use CanvasView to find canvas object in vicinity
      return cv.ControlFromElement(e);
    }
    #endregion methods
  }
}

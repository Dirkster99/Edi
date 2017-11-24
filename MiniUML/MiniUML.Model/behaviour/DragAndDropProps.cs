namespace MiniUML.Model.behaviour
{
  using System.Windows;
  using System.Windows.Input;

  using MiniUML.Framework.helpers;

  /// <summary>
  /// Drag and drop behviour to drag & drop elements from
  /// the toolbox on to the canvas
  /// or create elements on canvas through a single click.
  /// </summary>
  public static class DragAndDropProps
  {
    #region fields
    private static readonly DependencyProperty DragStartPointProperty =
        DependencyProperty.RegisterAttached("DragStartPoint", typeof(Point?), typeof(DragAndDropProps));

    private static readonly DependencyProperty EnabledForDragProperty =
        DependencyProperty.RegisterAttached("EnabledForDrag", typeof(bool), typeof(DragAndDropProps),
            new FrameworkPropertyMetadata((bool)false,
                new PropertyChangedCallback(OnEnabledForDragChanged)));
    #endregion fields

    #region DragStartPoint

    /// <summary>
    /// Standard method of DragStartPoint dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Point? GetDragStartPoint(DependencyObject d)
    {
      return (Point?)d.GetValue(DragStartPointProperty);
    }

    /// <summary>
    /// Standard method of DragStartPoint dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="value"></param>
    public static void SetDragStartPoint(DependencyObject d, Point? value)
    {
      d.SetValue(DragStartPointProperty, value);
    }

    #endregion

    #region EnabledForDrag

    /// <summary>
    /// Standard method of EnabledForDrag dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool GetEnabledForDrag(DependencyObject d)
    {
      return (bool)d.GetValue(EnabledForDragProperty);
    }

    /// <summary>
    /// Standard method of EnabledForDrag dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="value"></param>
    public static void SetEnabledForDrag(DependencyObject d, bool value)
    {
      d.SetValue(EnabledForDragProperty, value);
    }

    private static void OnEnabledForDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement fe = (FrameworkElement)d;

      if ((bool)e.NewValue)
      {
        fe.PreviewMouseDown += Fe_PreviewMouseDown;
        fe.MouseMove += Fe_MouseMove;
        fe.MouseUp += fe_MouseUp;
      }
      else
      {
        fe.PreviewMouseDown -= Fe_PreviewMouseDown;
        fe.MouseMove -= Fe_MouseMove;
        fe.MouseUp -= fe_MouseUp;
      }
    }

    /// <summary>
    /// Use the mouse up event to imitate a mouse click (together with mouse down).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void fe_MouseUp(object sender, MouseButtonEventArgs e)
    {
      Point? dragStartPoint = GetDragStartPoint((DependencyObject)sender);

      if (dragStartPoint != null)
      {

                if (((FrameworkElement)sender).DataContext is ToolBoxData)
                {
                    ToolBoxData d = ((FrameworkElement)sender).DataContext as ToolBoxData;

                    if (d.CreateShapeCommand.CreateShape.CanExecute(null))
                        d.CreateShapeCommand.CreateShape.Execute(null);
                }
            }
    }
    #endregion

    private static void Fe_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      Point? dragStartPoint = GetDragStartPoint((DependencyObject)sender);

      if (e.LeftButton != MouseButtonState.Pressed)
      {
        dragStartPoint = null;
      }

      if (dragStartPoint.HasValue)
      {
        DragObject dataObject = new DragObject();


                if (((FrameworkElement)sender).DataContext is ToolBoxData)
                {
                    ToolBoxData d = ((FrameworkElement)sender).DataContext as ToolBoxData;
                    dataObject.ObjectInstance = (object)d.CreateShapeCommand;

                    DragDrop.DoDragDrop((DependencyObject)sender, dataObject, DragDropEffects.Copy);

                    e.Handled = true;
                }
            }
    }

    private static void Fe_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      SetDragStartPoint((DependencyObject)sender, e.GetPosition((IInputElement)sender));
    }
  }
}

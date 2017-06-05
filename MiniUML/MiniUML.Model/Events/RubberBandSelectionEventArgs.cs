namespace MiniUML.View.Views.RubberBand
{
  using System.Windows;

  /// <summary>
  /// This delgation method can be used to handle the <seealso cref="RubberBandSelectionEventArgs"/> event.
  /// 
  /// The <seealso cref="RubberBandSelectionEventArgs"/> event used to message the rubber band selection
  /// event within the view element layers to a viewmodel. The rubber band selection is basicaly a notification
  /// that says: 'Hey, the user has just complited a rubber band gesture (grad mouse on canvas) and here are
  /// the coordinates for it'.
  /// 
  /// The event is composed of rectangle coordinates (e.g. right, bottom corner)
  /// plus selection information to determine whether new elements are to be selected
  /// in addition to the already selected elements or not.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void RubberBandSelectionEventHandler(object sender, RubberBandSelectionEventArgs e);

  /// <summary>
  /// Selection information to determine whether new elements are to
  /// be selected in addition to the already selected elements or not.
  /// </summary>
  public enum MouseSelection
  {
    /// <summary>
    /// Erase current selection and apply a new selection to the items
    /// that are to be selected next (previously selected items are not
    /// necessarily selected after new selection is applied).
    /// </summary>
    ReducedToNewSelection,

    /// <summary>
    /// Keep current selection and add a new selection to the items
    /// that are to be selected next (previously selected items remain selected).
    /// </summary>
    AddToCurrentSelection,

    CancelSelection
  }

  /// <summary>
  /// This class is used to message the rubber band selection
  /// event within the view and viewmodel element layers. The rubber band selection is basicaly a notification
  /// that says: 'Hey, the user has just complited a rubber band gesture (grad mouse on canvas) and here are
  /// the coordinates for it'.
  ///
  /// The event is composed of rectangle coordinates (e.g. right, bottom corner)
  /// plus selection information to determine whether new elements are to be selected
  /// in addition to the already selected elements or not.
  /// </summary>
  public class RubberBandSelectionEventArgs : RoutedEventArgs
  {
    #region constructor
    /// <summary>
    ///    Initializes a new instance of the <see RubberBandSelectionEventArgs class.
    ///
    /// Parameters:
    ///  horizontalChange:
    ///    The horizontal change in the System.Windows.Controls.Primitives.Thumb position
    ///    since the last System.Windows.Controls.Primitives.Thumb.DragDelta event.
    ///
    ///  verticalChange:
    ///    The vertical change in the System.Windows.Controls.Primitives.Thumb position
    ///    since the last System.Windows.Controls.Primitives.Thumb.DragDelta event.
    /// </summary>
    /// <param name="top"></param>
    /// <param name="left"></param>
    /// <param name="bottom"></param>
    /// <param name="right"></param>
    /// <param name="ms">Add or remove current selection before applying new selection</param>
    public RubberBandSelectionEventArgs(double left,
                                        double top,
                                        double right,
                                        double bottom,
                                        MouseSelection ms)
      : base()
    {
      this.Top = top;
      this.Left = left;
      this.Right = right;
      this.Bottom = bottom;

      this.Select = ms;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get x-coordinate of selection rectangle
    /// </summary>
    public double Left { get; private set; }

    /// <summary>
    /// Get y-coordinate of selection rectangle
    /// </summary>
    public double Top { get; private set; }

    /// <summary>
    /// Get x-coordinate of bottom right corner of selection rectangle
    /// </summary>
    public double Right { get; private set; }

    /// <summary>
    /// Get y-coordinate of bottom right corner of selection rectangle
    /// </summary>
    public double Bottom { get; private set; }

    /// <summary>
    /// Get top left corner of selection rectangle
    /// </summary>
    public Point StartPoint
    {
      get
      {
        return new Point(this.Left, this.Top);
      }
    }

    /// <summary>
    /// Get bottom-right corner of selection rectangle
    /// </summary>
    public Point EndPoint
    {
      get
      {
        return new Point(this.Right, this.Bottom);
      }
    }

    /// <summary>
    /// Get whether to add or remove current selection before applying new selection.
    /// </summary>
    public MouseSelection Select { get; private set; }
    #endregion properties
  }
}

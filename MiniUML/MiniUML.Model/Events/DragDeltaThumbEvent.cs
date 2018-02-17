namespace MiniUML.Model.Events
{
    using System.Windows;
  using System.Windows.Controls.Primitives;

  /// <summary>
  /// Event handler delegation method to be used when handling <seealso cref="DragDeltaThumbEvent"/> events.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void DragDeltaThumbEventHandler(object sender, DragDeltaThumbEvent e);

  /// <summary>
  /// This class is used to message the resize thumb event within the view element layers
  /// to an element that has a binding to a viewmodel.
  /// 
  /// The additional HorizontalAlignment and VerticalAlignment parameters are needed to determine
  /// the source of an elements change (e.g. right, bottom corner).
  /// </summary>
  public class DragDeltaThumbEvent : DragDeltaEventArgs
  {
    #region constructor
    /// <summary>
    ///    Initializes a new instance of the System.Windows.Controls.Primitives.DragDeltaEventArgs
    ///    class.
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
    /// <param name="horizontalChange"></param>
    /// <param name="verticalChange"></param>
    /// <param name="ha"></param>
    /// <param name="va"></param>
    public DragDeltaThumbEvent(double horizontalChange, double verticalChange,
                               HorizontalAlignment ha,
                               VerticalAlignment va)
      : base(horizontalChange, verticalChange)
    {
      HorizontalAlignment = ha;
      VerticalAlignment = va;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get property to indicate whether change originates from the bottom or top of an element.
    /// </summary>
    public VerticalAlignment VerticalAlignment { get; private set; }

    /// <summary>
    /// Get property to indicate whether change originates from the left or right side of an element.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment { get; private set; }
    #endregion properties
  }
}

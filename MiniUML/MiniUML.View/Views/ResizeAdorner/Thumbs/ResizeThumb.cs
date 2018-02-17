namespace MiniUML.View.Views.ResizeAdorner.Thumbs
{
  using System.Windows.Controls.Primitives;
  using Model.Events;

  /// <summary>
  /// A resize thumb can be used to resize a shape with user interaction.
  /// 
  /// The resize adorner view design is baed on
  /// http://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
  /// </summary>
  public class ResizeThumb : Thumb
  {
    public ResizeThumb()
    {
      DragDelta += ResizeThumb_DragDelta;
    }

    /// <summary>
    /// This event is processed when the <seealso cref="ResizeThumb"/>  has been moved.
    /// The view element defines its own event because it is also relevant to message
    /// where (top, bottom, left, or right) the item has been moved (in relation to the
    /// overall view item).
    /// </summary>
    public new event DragDeltaThumbEventHandler DragDeltaEvent;

    /// <summary>
    /// Handle the event in which a resize thumb has been moved as resize event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (DragDeltaEvent != null)
        DragDeltaEvent(this, new DragDeltaThumbEvent(e.HorizontalChange, e.VerticalChange,
                                                          HorizontalAlignment, VerticalAlignment));
    }
  }
}

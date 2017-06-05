namespace MiniUML.View.Views.ResizeAdorner.Thumbs
{
  using System.Windows.Controls.Primitives;

  /// <summary>
  /// A move thumb can be used to move a shape on the canvas.
  /// (This thumb is not yet used in this implementation)
  /// 
  /// The resize adorner view design is baed on
  /// http://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
  /// </summary>
  public class MoveThumb : Thumb
  {
    ////private DesignerItem designerItem;
    //// private DesignerCanvas designerCanvas;

    public MoveThumb()
    {
      //// DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
      //// DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
    }

/****
    private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
      this.designerItem = DataContext as DesignerItem;

      if (this.designerItem != null)
      {
        this.designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
      }
    }
****/

/****
    private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (this.designerItem != null && this.designerCanvas != null && this.designerItem.IsSelected)
      {
        double minLeft = double.MaxValue;
        double minTop = double.MaxValue;

        foreach (DesignerItem item in this.designerCanvas.SelectedItems)
        {
          minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
          minTop = Math.Min(Canvas.GetTop(item), minTop);
        }

        double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
        double deltaVertical = Math.Max(-minTop, e.VerticalChange);

        foreach (DesignerItem item in this.designerCanvas.SelectedItems)
        {
          Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
          Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
        }

        this.designerCanvas.InvalidateMeasure();
        e.Handled = true;
      }
    }
 */
  }
}

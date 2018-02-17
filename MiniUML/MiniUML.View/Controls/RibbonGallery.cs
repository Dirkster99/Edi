namespace MiniUML.View.Controls
{
    using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
    using Framework.interfaces;

  /// <summary>
  /// Interaction logic for RibbonGallery.xaml
  /// </summary>
  public class RibbonGallery : ListBox
  {
    #region fields
    private Point mStartPoint;
    #endregion fields

    #region constructor
    static RibbonGallery()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGallery), new FrameworkPropertyMetadata(typeof(RibbonGallery)));
    }
    #endregion constructor

    #region methods
    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);
      mStartPoint = e.GetPosition(null);

      // TODO: This works, but it's a bit too fragile...
      SelectedItem = e.Source;
    }

    /// <summary>
    /// Method is executed when the user drags an item
    /// from the ribbon gallery onto the canvas.
    /// </summary>
    /* <param name="e"></param>
    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
      {
        if (this.SelectedItem == null)
          return;

        CommandButton selectedItem = this.SelectedItem as CommandButton;
        
        if (selectedItem != null)
        {
          IDragableCommandModel cmd = selectedItem.CommandModel as IDragableCommandModel;
          
          if (cmd == null)
            return;

          Point position = e.GetPosition(null);

          if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
              Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
          {
            this.DoDragDrop(e, cmd);
            this.SelectedIndex = -1;
          }
        }
        else
        {
          FrameworkElement frameworkItem = this.SelectedItem as FrameworkElement;
        
          if (frameworkItem == null)
            return;

          IDragableCommandModel cmd = selectedItem.Command as IDragableCommandModel;
          
          if (cmd == null)
            return;

          Point position = e.GetPosition(null);

          if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
              Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
          {
            this.DoDragDrop(e, cmd);
            this.SelectedIndex = -1;
          }
        }
      }
    }*/

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseUp(e);
      SelectedIndex = -1;
    }

    private void DoDragDrop(MouseEventArgs e, IDragableCommandModel cmd)
    {
      ////_isDragging = true;
      DataObject data = new DataObject(typeof(IDragableCommandModel), cmd);
      DragDropEffects de = DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
      ////_isDragging = false;
    }
    #endregion methods
  }
}

namespace MiniUML.Framework.interfaces
{
  using System.Windows;

  /// <summary>
  /// This interface specifies a method that is called when an item is
  /// dragged from the toolbox and being dropped on the canvas.
  /// </summary>
  public interface IDragableCommandModel
  {
    /// <summary>
    /// Method is required by <seealso cref="IDragableCommandModel"/>. It is executed
    /// when the drag & drop operation on the canvas is infished with its last step
    /// (the creation of the viewmodel for the new item).
    /// </summary>
    /// <param name="dropPoint"></param>    
    void OnDragDropExecute(Point dropPoint);
  }
}

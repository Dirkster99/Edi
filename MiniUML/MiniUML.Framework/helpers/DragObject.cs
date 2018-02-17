namespace MiniUML.Framework.helpers
{
    /// <summary>
  /// Wraps info of the dragged object into a class
  /// Source: http://www.codeproject.com/Articles/484616/MVVM-Diagram-Designer?msg=4413242#Drag-And-Drop-To-The-Design-Surface
  /// </summary>
  public class DragObject
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragObject()
    {
      ObjectInstance = null;
    }

    /// <summary>
    /// Get property to find an instance that can create a new canvas item.
    /// </summary>
    public object ObjectInstance { get; set; }
  }
}

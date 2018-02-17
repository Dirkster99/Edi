namespace MiniUML.Model.ViewModels
{
  using Events;
  using Shapes;

  /// <summary>
  /// Shape size adjustment operations
  /// supported by the <seealso cref="IShapeParent"/> interface.
  /// </summary>
  public enum SameSize
  {
    SameWidth,
    SameHeight,
    SameWidthandHeight
  }

  /// <summary>
  /// Shape alignment options for shape alignment operations
  /// supported by the <seealso cref="IShapeParent"/> interface.
  /// </summary>
  public enum AlignShapes
  {
    Left,
    Top,
    Right,
    Bottom,
    CenteredHorizontal,
    CenteredVertical
  }

  public enum Destribute
  {
    Horizontally,
    Vertically,
  }

  /// <summary>
  /// Declare interface that allows shapes to
  /// call parent methodes to execute operations
  /// on them selfs (in the context of their parent element).
  /// 
  /// Parent => Canvas
  ///  Child => Shape 
  ///  --> The interface linkes ShapeViewModels to their CanvasViewModel.
  /// </summary>
  public interface IShapeParent
  {
    /// <summary>
    /// Brings the shape into front of the canvas view
    /// (moves shape on top of virtual Z-axis)
    /// </summary>
    /// <param name="obj"></param>
    void BringToFront(ShapeViewModelBase obj);

    /// <summary>
    /// Brings the shape into the back of the canvas view
    /// (moves shape to the bottom of virtual Z-axis)
    /// </summary>
    /// <param name="obj"></param>
    void SendToBack(ShapeViewModelBase obj);

    /// <summary>
    /// Removes the corresponding shape from the
    /// collection of shapes displayed on the canvas.
    /// </summary>
    /// <param name="obj"></param>
    void Remove(ShapeViewModelBase obj);

    /// <summary>
    /// Resizes the currently selected shapes (if any) by the width or height
    /// requested by the <paramref name="e"/> parameter.
    /// </summary>
    /// <param name="e"></param>
    void ResizeSelectedShapes(DragDeltaThumbEvent e);

    /// <summary>
    /// Align all selected shapes (if any) to a given shape <paramref name="shape"/>.
    /// The actual alignment operation performed is defined by the <paramref name="alignmentOption"/> parameter.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="alignmentOption"></param>
    void AlignShapes(ShapeSizeViewModelBase shape, AlignShapes alignmentOption);

    /// <summary>
    /// Adjusts width, height, or both, of all selected shapes (if any)
    /// such that they are sized equally to the given <paramref name="shape"/>.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="option"></param>
    void AdjustShapesToSameSize(ShapeSizeViewModelBase shape, SameSize option);

    void DistributeShapes(Destribute distibOption);
  }
}

namespace MiniUML.Model.ViewModels
{
    using MiniUML.Model.Events;
    using MiniUML.Model.ViewModels.Shapes;

    /// <summary>
    /// Shape size adjustment operations
    /// supported by the <seealso cref="IShapeParent"/> interface.
    /// </summary>
    public enum SameSize
    {
        /// <summary>
        /// Adjust shapes to have the same width.
        /// </summary>
        SameWidth,

        /// <summary>
        /// Adjust shapes to have the same height.
        /// </summary>
        SameHeight,

        /// <summary>
        /// Adjust shapes to have the same width and height.
        /// </summary>
        SameWidthandHeight
    }

    /// <summary>
    /// Shape alignment options for shape alignment operations
    /// supported by the <seealso cref="IShapeParent"/> interface.
    /// </summary>
    public enum AlignShapes
    {
        /// <summary>
        /// Align shapes at a left X minimum coordinate.
        /// </summary>
        Left,

        /// <summary>
        /// Align shapes at a top Y minimum coordinate.
        /// </summary>
        Top,

        /// <summary>
        /// Align shapes at a right X maximum coordinate.
        /// </summary>
        Right,

        /// <summary>
        /// Align shapes at a bottom Y maximum coordinate.
        /// </summary>
        Bottom,

        /// <summary>
        /// Align shapes centered around an Y coordinate.
        /// </summary>
        CenteredHorizontal,

        /// <summary>
        /// Align shapes centered around an X coordinate.
        /// </summary>
        CenteredVertical
    }

    /// <summary>
    /// Define whether shapes should be destributed horizontally or vertically.
    /// </summary>
    public enum Destribute
    {
        /// <summary>
        /// Destribute shapes horizontally.
        /// </summary>
        Horizontally,

        /// <summary>
        /// Destribute shapes vertically.
        /// </summary>
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

        /// <summary>
        /// Destribute all selected shapes (if any) over X or Y space evenly.
        /// </summary>
        /// <param name="distibOption"></param>
        void DistributeShapes(Destribute distibOption);
    }
}

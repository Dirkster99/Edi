namespace MiniUML.Model.ViewModels.Interfaces
{
    using MiniUML.Model.ViewModels.Shapes;
    using System.Windows;

    /// <summary>
    /// Interface to define interaction for drag and drop
    /// mouse gestures when adding new connection lines,
    /// selecting and resizing shapes, and so forth.
    /// </summary>
    public interface ICanvasViewMouseHandler
    {
        void OnShapeClick(ShapeViewModelBase shape);

        void OnShapeDragBegin(Point position, ShapeViewModelBase shape);

        void OnShapeDragUpdate(Point position, Vector delta);

        void OnShapeDragEnd(Point position, ShapeViewModelBase shape);

        void OnCancelMouseHandler();
    }
}

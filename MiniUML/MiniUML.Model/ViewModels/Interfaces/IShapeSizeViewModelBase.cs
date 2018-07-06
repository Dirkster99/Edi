namespace MiniUML.Model.ViewModels.Interfaces
{
    using System.Windows;
    using System.Windows.Input;
    using MiniUML.Framework.Command;

    /// <summary>
    /// Base interface to manage data items for each shape
    /// that is visible and resizeable on the canvas.
    /// </summary>
    public interface IShapeSizeViewModelBase : IShapeViewModelBase
    {
        #region properties
        /// <summary>
        /// Get/set width of the bound shape.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Get/set height of the bound shape.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Get/set minimum width of shape.
        /// </summary>
        double MinWidth { get; set; }

        /// <summary>
        /// Get/set minimum height of shape.
        /// </summary>
        double MinHeight { get; set; }

        /// <summary>
        /// Get/set bottom righ position of shape
        /// (use this with care as it will adjust
        /// the width and height of a shape).
        /// </summary>
        Point EndPosition { get; set; }

        /// <summary>
        /// View elements (DesignerItem) can bind to this property to get a resize shape
        /// command across into the viewmodel. THis command resizes all currently selected
        /// shapes view the relative movement passed into this command.
        /// </summary>
        ICommand ResizeSelectedShapesCommand { get; }

        /// <summary>
        /// Align all selected shapes (if any) to a given shape (this).
        /// The actual alignment operation performed is defined by the command
        /// parameter of <seealso cref="AlignShapes"/> type.
        /// </summary>
        RelayCommand<object> AlignShapes { get; }

        /// <summary>
        /// Get command to adjust the width and/or height of all selected shapes
        /// to the size of a given (this) shape. Command requires 3 parameters:
        /// 
        /// this            - A shape to adjust the size of all other shapes to (implizit parameter)
        /// SameSize        - enumeration member to determine actual sizing request (explizit)
        /// Selected shapes - shapes that are selected via CNTRL+Click or rubberband selection (implizit parameter)
        /// </summary>
        RelayCommand<object> AdjustShapesToSameSize { get; }

        /// <summary>
        /// Distribute all selected shapes evenly over X or Y space on canvas.
        /// Command Parameter: <seealso cref="Destribute"/> enumeration member
        /// to request actual X or Y spacing option.
        /// </summary>
        RelayCommand<object> DestributeShapes { get; }
        #endregion properties
    }
}

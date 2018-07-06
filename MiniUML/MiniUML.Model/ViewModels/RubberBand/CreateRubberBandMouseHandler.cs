namespace MiniUML.Model.ViewModels.RubberBand
{
    using System.Windows;
    using System.Windows.Input;
    using MiniUML.Model.ViewModels.Document;
    using MiniUML.Model.ViewModels.Interfaces;
    using MiniUML.Model.ViewModels.Shapes;
    using MiniUML.View.Views.RubberBand;

    /// <summary>
    /// The class sets up a mouse handler for drawing and handling a rubber band
    /// selection via mouse gesture (drag operation on drawing canvas).
    /// </summary>
    public class CreateRubberBandMouseHandler : ICanvasViewMouseHandler
    {
        #region fields
        private RubberBandViewModel _RubberBandViewModel = null;
        private bool _IsDone = false;

        // Document viewmodel that contains the shapes including the new association
        private CanvasViewModel _CanvasViewModel;
        #endregion fields

        #region constructor
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="rubberBand"></param>
        public CreateRubberBandMouseHandler(RubberBandViewModel rubberBand, CanvasViewModel canvasViewModel)
        {
            _RubberBandViewModel = rubberBand;
            _CanvasViewModel = canvasViewModel;
        }
        #endregion constructor

        /// <summary>
        /// The rubber band selection event is basicaly a notification that says:
        /// 'Hey, the user has just complited a rubber band gesture (grad mouse on canvas)
        /// and here are the coordinates for it'.
        /// </summary>
        public event RubberBandSelectionEventHandler RubberBandSelection = null;

        #region methods
        #region ICanvasViewMouseHandler methods
        void ICanvasViewMouseHandler.OnShapeClick(ShapeViewModelBase shape)
        {
            if (_IsDone) // return if command is already finished (successfully or cancelled)
                return;
        }

        void ICanvasViewMouseHandler.OnShapeDragBegin(Point position, ShapeViewModelBase shape)
        {
            if (_IsDone) // return if command is already finished (successfully or cancelled)
                return;

            _RubberBandViewModel.IsVisible = true;
        }

        void ICanvasViewMouseHandler.OnShapeDragUpdate(Point position, Vector delta)
        {
            if (_IsDone) // return if command is already finished (successfully or cancelled)
                return;

            _RubberBandViewModel.EndPosition = position;
        }

        void ICanvasViewMouseHandler.OnShapeDragEnd(Point position, ShapeViewModelBase element)
        {
            if (_IsDone) // return if command is already finished (successfully or cancelled)
                return;

            // Clear current selection if no control key is pressed on the keyboard (which would allow adding selected items)
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.RightCtrl) == false))
            {
                _RubberBandViewModel.Select = MouseSelection.ReducedToNewSelection;
            }
            else
                _RubberBandViewModel.Select = MouseSelection.AddToCurrentSelection;

            // create an event that tells all subscribers that we have completed a new selection
            if (this.RubberBandSelection != null)
                this.RubberBandSelection(this, _RubberBandViewModel.GetSelectionEvent());

            CleanUp();
            _CanvasViewModel.FinishCanvasViewMouseHandler();
        }

        void ICanvasViewMouseHandler.OnCancelMouseHandler()
        {
            if (_IsDone) // return if command is already finished (successfully or cancelled)
                return;

            if (_RubberBandViewModel != null)
            {
                // Cancel selection
                _RubberBandViewModel.Select = MouseSelection.CancelSelection;

                // create an event that tells all subscribers that we have completed a new selection
                if (this.RubberBandSelection != null)
                    this.RubberBandSelection(this, _RubberBandViewModel.GetSelectionEvent());
            }

            CleanUp();
        }
        #endregion ICanvasViewMouseHandler methods

        /// <summary>
        /// Reset all variables to indicate that this operation is finished
        /// (successfully or cancelled).
        /// </summary>
        private void CleanUp()
        {
            _RubberBandViewModel.IsVisible = false;
            _IsDone = true;
        }
        #endregion methods
    }
}

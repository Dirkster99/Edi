namespace MiniUML.Model.ViewModels.RubberBand
{
  using System.Windows;
  using System.Windows.Input;
  using Document;
  using Shapes;
  using View.Views.RubberBand;

  /// <summary>
  /// The class sets up a mouse handler for drawing and handling a rubber band
  /// selection via mouse gesture (drag operation on drawing canvas).
  /// </summary>
  public class CreateRubberBandMouseHandler : ICanvasViewMouseHandler
  {
    #region fields
    private RubberBandViewModel mRubberBandViewModel = null;
    private bool mIsDone = false;

    // Document viewmodel that contains the shapes including the new association
    private CanvasViewModel mCanvasViewModel;
    #endregion fields

    #region constructor
    /// <summary>
    /// Parameterized constructor
    /// </summary>
    /// <param name="rubberBand"></param>
    public CreateRubberBandMouseHandler(RubberBandViewModel rubberBand, CanvasViewModel canvasViewModel)
    {
      mRubberBandViewModel = rubberBand;
      mCanvasViewModel = canvasViewModel;
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
      if (mIsDone) // return if command is already finished (successfully or cancelled)
        return;
    }

    void ICanvasViewMouseHandler.OnShapeDragBegin(Point position, ShapeViewModelBase shape)
    {
      if (mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      mRubberBandViewModel.IsVisible = true;
    }
  
    void ICanvasViewMouseHandler.OnShapeDragUpdate(Point position, Vector delta)
    {
      if (mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      mRubberBandViewModel.EndPosition = position;
    }
  
    void ICanvasViewMouseHandler.OnShapeDragEnd(Point position, ShapeViewModelBase element)
    {
      if (mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      // Clear current selection if no control key is pressed on the keyboard (which would allow adding selected items)
      if ((Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.RightCtrl) == false))
      {
        mRubberBandViewModel.Select = MouseSelection.ReducedToNewSelection;
      }
      else
        mRubberBandViewModel.Select = MouseSelection.AddToCurrentSelection;

      // create an event that tells all subscribers that we have completed a new selection
      if (RubberBandSelection != null)
        RubberBandSelection(this, mRubberBandViewModel.GetSelectionEvent());

      cleanUp();
      mCanvasViewModel.FinishCanvasViewMouseHandler();
    }
  
    void ICanvasViewMouseHandler.OnCancelMouseHandler()
    {
      if (mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      if (mRubberBandViewModel != null)
      {
        // Cancel selection
        mRubberBandViewModel.Select = MouseSelection.CancelSelection;

        // create an event that tells all subscribers that we have completed a new selection
        if (RubberBandSelection != null)
          RubberBandSelection(this, mRubberBandViewModel.GetSelectionEvent());
      }

      cleanUp();
    }
    #endregion ICanvasViewMouseHandler methods

    /// <summary>
    /// Reset all variables to indicate that this operation is finished
    /// (successfully or cancelled).
    /// </summary>
    private void cleanUp()
    {
      mRubberBandViewModel.IsVisible = false;
      mIsDone = true;
    }
    #endregion methods
  }
}

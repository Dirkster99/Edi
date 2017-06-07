namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ConnectCreate
{
  using System.Windows;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Document;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect;
  using MiniUML.View.Views;

  /// <summary>
  /// Class to handle mouseevents for drawing a line between two shapes.
  /// </summary>
  public abstract class AbstractCreateAssociationMouseHandler : ICanvasViewMouseHandler
  {
    #region fields
    // is true when drag operation is finished succesfully or mouse operation is cancelled
    private bool mIsDone = false;

    // determines if a new shape has already been added to the data model or not
    // (to remove the shape if operation was cancelled when it was already added in the model)
    private bool mHasBeenAdded = false;

    // Association viemodel being added with the mouse handler
    private UmlAssociationShapeViewModel mAssociation;

    // Document viewmodel that contains the shapes including the new association
    private AbstractDocumentViewModel mViewModel;
    #endregion fields

    #region constructor
    /// <summary>
    /// Parameterized class constructor
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="association"></param>
    public AbstractCreateAssociationMouseHandler(AbstractDocumentViewModel viewModel,
                                                 UmlAssociationShapeViewModel association)
    {
      this.mViewModel = viewModel;
      this.mAssociation = association;

      this.mAssociation.Add(new AnchorViewModel(viewModel.vm_CanvasViewModel)
                                {
                                  Left = 0,
                                  Top = 0
                                  //// ,UmlShapeKey = "Anchor"
                                });

      this.mAssociation.Add(new AnchorViewModel(viewModel.vm_CanvasViewModel)
                                {
                                  Left = 0,
                                  Top = 0
                                  //// ,UmlShapeKey = "Anchor"
                                });
    }
    #endregion constructor

    #region methods
    #region ICanvasViewMouseHandler interface methods
    /// <summary>
    /// Cancels usage of this mouse handler if user clicks on a shape.
    /// </summary>
    /// <param name="shape"></param>
    void ICanvasViewMouseHandler.OnShapeClick(ShapeViewModelBase shape)
    {
      if (this.mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      this.mViewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
    }

    /// <summary>
    /// Check whether first position is valid and add new association shape
    /// with first position into document root if it is valid (cancel otherwise).
    /// </summary>
    /// <param name="position"></param>
    /// <param name="shape"></param>
    void ICanvasViewMouseHandler.OnShapeDragBegin(Point position, ShapeViewModelBase shape)
    {
      if (this.mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      string idString = string.Empty;

      if (shape != null)
      {
        if (this.IsValidFrom(shape) == false)
        {
          this.mViewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
          return;
        }

        idString = shape.ID;            // Add a shape id if we begin drawing over a shape
        if (idString == string.Empty)
        {
          idString = this.mViewModel.dm_DocumentDataModel.GetUniqueId();
          shape.ID = idString;
        }

        this.mAssociation.From = idString;
      }

      ((ShapeViewModelBase)this.mAssociation.FirstNode).Position = position;
      ////Console.WriteLine("First position is: {0}", position);

      // Add the shape to the document root.
      this.mViewModel.vm_CanvasViewModel.AddShape((ShapeViewModelBase)this.mAssociation);
      this.mHasBeenAdded = true;

      // Set associations view element to hit test invisible to make it non-interactible with mouse
      ((CanvasView)this.mViewModel.v_CanvasView).PresenterFromElement(this.mAssociation).IsHitTestVisible = false;
    }

    /// <summary>
    /// Updates the end position of the assocation shape durring the drag operation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="delta"></param>
    void ICanvasViewMouseHandler.OnShapeDragUpdate(Point position, Vector delta)
    {
      if (this.mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      ((ShapeViewModelBase)this.mAssociation.LastNode).Position  = position;
    }

    /// <summary>
    /// Determines whether the end point of the drag operation is valid and updates the
    /// already added association shape in the document root or removes it if not.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="shape"></param>
    void ICanvasViewMouseHandler.OnShapeDragEnd(Point position, ShapeViewModelBase shape)
    {
      if (this.mIsDone) // return if command is already finished (successfully or cancelled)
        return;

      string idString = string.Empty;
      if (shape != null)
      {
        if (this.IsValidTo(shape) == false)
        {
          this.mViewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
          return;
        }

        idString = shape.ID;
        if (idString == string.Empty)
        {
          idString = this.mViewModel.dm_DocumentDataModel.GetUniqueId();
          shape.ID = idString;
        }
      }

      // HACK: Work around for odd not-quite-updated binding problem: Remove, then re-add.
      // to (re)-fire SnapToTarget event again when the element is being removed and re-added.
      this.mViewModel.dm_DocumentDataModel.RemoveShape(this.mAssociation);

      // (Re)-setting this property should in fact (re)-fire the snapToTarget event (but it does not?)
      this.mAssociation.To = idString;

      this.mViewModel.vm_CanvasViewModel.AddShape(this.mAssociation);
      ////Console.WriteLine("Last position is: {0}", position);

      // make newly added association view hit test visible (enables user interaction)
      ((CanvasView)this.mViewModel.v_CanvasView).PresenterFromElement(this.mAssociation).IsHitTestVisible = true;

      this.CleanUp();                                                    // indicate end of this mouse handler command
      this.mViewModel.vm_CanvasViewModel.FinishCanvasViewMouseHandler();
    }

    /// <summary>
    /// Perform mouse handler cancel operation:
    /// - Remove new association shape if it is already part of the model
    /// - Reset all internal states to indicate that this command is finished.
    /// </summary>
    void ICanvasViewMouseHandler.OnCancelMouseHandler()
    {
      if (this.mHasBeenAdded == true)
        this.mAssociation.Remove();

      this.CleanUp();
    }
    #endregion ICanvasViewMouseHandler

    /// <summary>
    /// Determine whether the association accepts the <paramref name="element"/>
    /// as input connection or not. Cancel association is performed if not.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    protected abstract bool IsValidFrom(ShapeViewModelBase element);

    /// <summary>
    /// Determine whether the association accepts the <paramref name="element"/>
    /// as output connection or not. Cancel association is performed if not.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    protected abstract bool IsValidTo(ShapeViewModelBase element);

    /// <summary>
    /// Reset all variables to indicate that this operation is finished
    /// (successfully or cancelled).
    /// </summary>
    private void CleanUp()
    {
      this.mViewModel.v_CanvasView.ForceCursor = true;
      this.mViewModel.v_CanvasView.Cursor = null;
      this.mIsDone = true;
    }
    #endregion methods
  }
}

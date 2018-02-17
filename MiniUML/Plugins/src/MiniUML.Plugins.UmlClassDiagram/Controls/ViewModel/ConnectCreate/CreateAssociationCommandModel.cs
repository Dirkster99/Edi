namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ConnectCreate
{
  using System.Windows;
  using System.Windows.Input;
  using Framework;
  using Framework.Command;
  using Framework.interfaces;
  using Model.ViewModels;
  using Model.ViewModels.Command;
  using Model.ViewModels.Document;
  using Model.ViewModels.Shapes;
  using Connect;
  using UmlElements;

  public class CreateAssociationCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    private PluginViewModel mViewModel;
    private RelayCommand<object> mCreateShape;

    private UmlTypes mUmlType;

      #endregion fields

    #region constructor
    /// <summary>
    /// Standard cosntructor
    /// </summary>
    public CreateAssociationCommandModel(PluginViewModel viewModel,
                                         string toolboxImageUrl,
                                         UmlTypes umlType,
                                         string toolBoxName,
                                         string toolBoxDescription)
    {
      mViewModel = viewModel;

      ToolBoxImageUrl = toolboxImageUrl;
      mUmlType = umlType;
      Title = toolBoxName;
      ToolTip = toolBoxDescription;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get property to command which can create a new shape.
    /// </summary>
    public override ICommand CreateShape
    {
      get
      { return mCreateShape ?? (mCreateShape = new RelayCommand<object>((p) => OnExecute(), (p) => OnQueryEnabled())); }
    }

    /// <summary>
    /// Get description string for this canvase item.
    /// </summary>
    public string ToolTip { get; }

      /// <summary>
    /// Get title string for this canvase item.
    /// </summary>
    public string Title { get; }

      #endregion properties

    #region methods
    /// <summary>
    /// Method is required by <seealso cref="IDragableCommandModel"/>. It is executed
    /// when the drag & drop operation on the canvas is infished with its last step
    /// (the creation of the viewmodel for the new item).
    /// </summary>
    /// <param name="dropPoint"></param>
    public void OnDragDropExecute(Point dropPoint)
    {
      var model = mViewModel.mWindowViewModel.vm_DocumentViewModel;

      model.v_CanvasView.ForceCursor = true;
      model.v_CanvasView.Cursor = Cursors.Pen;

      IShapeParent parent = model.vm_CanvasViewModel;

      // Tell the canvas view model that we are about to change the mode of 'object selection'
      // to one which allows us to connect things on the canvas.
      model.vm_CanvasViewModel.BeginCanvasViewMouseHandler(
          new CreateAssociationMouseHandler(model,
                                            UmlElementDataDef.CreateShape(mUmlType, dropPoint, parent) as UmlAssociationShapeViewModel));
    }

    #region CreateShape Command
    private bool OnQueryEnabled()
    {
      if (mViewModel == null)
        return false;

      return mViewModel.mWindowViewModel.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready;
    }

    private void OnExecute()
    {
      OnDragDropExecute(new Point(100, 10));
    }
    #endregion CreateShape Command
    #endregion methods

    #region private class
    private class CreateAssociationMouseHandler : AbstractCreateAssociationMouseHandler
    {
      public CreateAssociationMouseHandler(AbstractDocumentViewModel viewModel, UmlAssociationShapeViewModel association)
        : base(viewModel, association)
      {
      }

      protected override bool IsValidFrom(ShapeViewModelBase element)
      {
        return true;

        /***
          string[] validShapes = { "Uml.Interface", "Uml.Class", "Uml.AbstractClass" };

          _fromElement = element;

          foreach (string s in validShapes)
            if (element.Name == s) return true;

          return false;
         ***/
      }

      protected override bool IsValidTo(ShapeViewModelBase element)
      {
        return true;
        /***
          string[] validShapes;

          if (_fromElement.Name == "Uml.Interface")
            validShapes = new string[] { "Uml.Interface" };
          else
            validShapes = new string[] { "Uml.Interface", "Uml.Class", "Uml.AbstractClass" };

          foreach (string s in validShapes)
            if (element.Name == s) return true;

          return false;
        ***/
      }
    }
    #endregion private classe
  }
}

namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ConnectCreate
{
  using System.Windows;
  using System.Windows.Input;
  using MiniUML.Framework;
  using MiniUML.Framework.Command;
  using MiniUML.Framework.interfaces;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Command;
  using MiniUML.Model.ViewModels.Document;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements;

  public class CreateAssociationCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    private PluginViewModel mViewModel;
    private RelayCommand<object> mCreateShape = null;

    private UmlTypes mUmlType;
    private string mToolBoxName;
    private string mToolBoxDescription;
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
      this.mViewModel = viewModel;

      this.ToolBoxImageUrl = toolboxImageUrl;
      this.mUmlType = umlType;
      this.mToolBoxName = toolBoxName;
      this.mToolBoxDescription = toolBoxDescription;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get property to command which can create a new shape.
    /// </summary>
    public override ICommand CreateShape
    {
      get
      {
        if (this.mCreateShape == null)
          this.mCreateShape = new RelayCommand<object>((p) => this.OnExecute(), (p) => this.OnQueryEnabled());

        return this.mCreateShape;
      }
    }

    /// <summary>
    /// Get description string for this canvase item.
    /// </summary>
    public string ToolTip
    {
      get
      {
        return this.mToolBoxDescription;
      }
    }

    /// <summary>
    /// Get title string for this canvase item.
    /// </summary>
    public string Title
    {
      get
      {
        return this.mToolBoxName;
      }
    }
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
      var model = this.mViewModel.mWindowViewModel.vm_DocumentViewModel;

      model.v_CanvasView.ForceCursor = true;
      model.v_CanvasView.Cursor = Cursors.Pen;

      IShapeParent parent = model.vm_CanvasViewModel;

      // Tell the canvas view model that we are about to change the mode of 'object selection'
      // to one which allows us to connect things on the canvas.
      model.vm_CanvasViewModel.BeginCanvasViewMouseHandler(
          new CreateAssociationMouseHandler(model,
                                            UmlElementDataDef.CreateShape(this.mUmlType, dropPoint, parent) as UmlAssociationShapeViewModel));
    }

    #region CreateShape Command
    private bool OnQueryEnabled()
    {
      if (this.mViewModel == null)
        return false;

      return this.mViewModel.mWindowViewModel.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready;
    }

    private void OnExecute()
    {
      this.OnDragDropExecute(new Point(100, 10));
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

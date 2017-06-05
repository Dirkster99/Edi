namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ShapeCreate
{
  using System.Windows;
  using System.Windows.Input;
  using MiniUML.Framework.Command;
  using MiniUML.Framework.interfaces;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Command;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements;

  /// <summary>
  /// Creates a command model that has the ability to create an UML Class Shape.
  /// </summary>
  internal class CreateShapeCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    private readonly PluginViewModel mViewModel;
    private readonly UmlTypes mUmlType;
    private readonly string mDescription;
    private readonly string mDisplayName;

    private RelayCommand<object> mCreateShape = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="viewModel"></param>
    public CreateShapeCommandModel(PluginViewModel viewModel,
                                   UmlTypes umlType,
                                   string description,
                                   string displayName,
                                   string toolboxImageUrl)
    {
      this.mViewModel = viewModel;
      this.mUmlType = umlType;

      this.mDescription = description;
      this.mDisplayName = displayName;
      this.ToolBoxImageUrl = toolboxImageUrl;
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
    /// Get description string for this canvase item  (for display in toolbox).
    /// </summary>
    public string ToolTip
    {
      get
      {
        return this.mDescription;
      }
    }

    /// <summary>
    /// Get title string for this canvase item (for display in toolbox).
    /// </summary>
    public string Title
    {
      get
      {
        return this.mDisplayName;
      }
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Method is required by <seealso cref="IDragableCommandModel"/>. It is executed
    /// when the drag & drop operation on the canvas is infished with its last step
    /// (the creation of the viewmodel for the new item).
    /// </summary>
    /// <param name="dropPoint"></param>
    public void OnDragDropExecute(Point dropPoint)
    {
      IShapeParent parent = this.mViewModel.mWindowViewModel.vm_DocumentViewModel.vm_CanvasViewModel;

      this.AddShape(this.mViewModel.mWindowViewModel.vm_DocumentViewModel.vm_CanvasViewModel,
                    UmlElementDataDef.CreateShape(this.mUmlType, dropPoint, parent));
    }

    #region CreateShape Command
    private bool OnQueryEnabled()
    {
      if (this.mViewModel == null)
        return false;

      return this.mViewModel.QueryEnableEditCommands();
    }

    private void OnExecute()
    {
      this.OnDragDropExecute(new Point(100, 10));
    }
    #endregion CreateShape Command
    #endregion methods
  }
}

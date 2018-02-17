namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ShapeCreate
{
  using System.Windows;
  using System.Windows.Input;
  using Framework.Command;
  using Framework.interfaces;
  using Model.ViewModels;
  using Model.ViewModels.Command;
  using UmlElements;

  /// <summary>
  /// Creates a command model that has the ability to create an UML Class Shape.
  /// </summary>
  internal class CreateShapeCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    private readonly PluginViewModel mViewModel;
    private readonly UmlTypes mUmlType;

      private RelayCommand<object> mCreateShape;
    #endregion fields

    #region constructor

      /// <summary>
      /// Standard constructor
      /// </summary>
      /// <param name="viewModel"></param>
      /// <param name="umlType"></param>
      /// <param name="description"></param>
      /// <param name="displayName"></param>
      /// <param name="toolboxImageUrl"></param>
      public CreateShapeCommandModel(PluginViewModel viewModel,
                                   UmlTypes umlType,
                                   string description,
                                   string displayName,
                                   string toolboxImageUrl)
    {
      mViewModel = viewModel;
      mUmlType = umlType;

      ToolTip = description;
      Title = displayName;
      ToolBoxImageUrl = toolboxImageUrl;
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
          return mCreateShape ?? (mCreateShape =
                     new RelayCommand<object>((p) => OnExecute(), (p) => OnQueryEnabled()));
      }
    }

    /// <summary>
    /// Get description string for this canvase item  (for display in toolbox).
    /// </summary>
    public string ToolTip { get; }

      /// <summary>
    /// Get title string for this canvase item (for display in toolbox).
    /// </summary>
    public string Title { get; }

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
      IShapeParent parent = mViewModel.mWindowViewModel.vm_DocumentViewModel.vm_CanvasViewModel;

      AddShape(mViewModel.mWindowViewModel.vm_DocumentViewModel.vm_CanvasViewModel,
                    UmlElementDataDef.CreateShape(mUmlType, dropPoint, parent));
    }

    #region CreateShape Command
    private bool OnQueryEnabled()
    {
        return mViewModel != null && mViewModel.QueryEnableEditCommands();
    }

    private void OnExecute()
    {
      OnDragDropExecute(new Point(100, 10));
    }
    #endregion CreateShape Command
    #endregion methods
  }
}

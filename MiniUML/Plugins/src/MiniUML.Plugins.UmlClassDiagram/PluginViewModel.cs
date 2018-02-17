namespace MiniUML.Plugins.UmlClassDiagram
{
  using Framework;
  using Model.ViewModels.Document;
  using Controls.ViewModel.UmlElements;
  using ToolBox.ViewModel;

  public partial class PluginViewModel : BaseViewModel
  {
    #region fields
    private ToolBoxControlViewModel mClassShapeBox;
    private ToolBoxControlViewModel mDeploymentShapeBox;
    private ToolBoxControlViewModel mUseCaseShapeBoxViewModel;
    private ToolBoxControlViewModel mActivityShapeBoxViewModel;
    private ToolBoxControlViewModel mConnectBox;

    private object lockobj = new object();
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowViewModel"></param>
    public PluginViewModel(IMiniUMLDocument windowViewModel)
    {
      // Store a reference to the parent view model.
      mWindowViewModel = windowViewModel;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get a collection of commands which can be used to create Class shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel ClassShapeBox => mClassShapeBox ?? (mClassShapeBox = new ToolBoxControlViewModel(this, UmlDiagrams.Class));

      /// <summary>
    /// Get a collection of commands which can be used to create Deployment shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel DeploymentShapeBox => mDeploymentShapeBox ??
                                                         (mDeploymentShapeBox = new ToolBoxControlViewModel(this, UmlDiagrams.Deployment));

      /// <summary>
    /// Get a collection of commands which can be used to create Use Case diagram shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel UseCaseShapeBox => mUseCaseShapeBoxViewModel ??
                                                      (mUseCaseShapeBoxViewModel = new ToolBoxControlViewModel(this, UmlDiagrams.UseCase));

      /// <summary>
    /// Get a collection of commands which can be used to create Activity diagram shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel ActivityShapeBox => mActivityShapeBoxViewModel ??
                                                       (mActivityShapeBoxViewModel = new ToolBoxControlViewModel(this, UmlDiagrams.Activity));

      /// <summary>
    /// Get a collection of commands which can be used to create connections between shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel ConnectBox => mConnectBox ?? (mConnectBox = new ToolBoxControlViewModel(this, UmlDiagrams.Connector));

      /// <summary>
    /// Get the current <seealso cref="IMiniUMLDocument"/> which contains the
    /// document with shapes and other items.
    /// </summary>
    public IMiniUMLDocument mWindowViewModel { get; }
    #endregion properties

    #region methods
    /// <summary>
    /// Thread safe method to find out whether document editing is currently permitted or not.
    /// </summary>
    /// <returns></returns>
    internal bool QueryEnableEditCommands()
    {
      lock (lockobj)
      {
          return mWindowViewModel?.vm_DocumentViewModel?.dm_DocumentDataModel?.State == DataModel.ModelState.Ready;
      }
    }
    #endregion methods
  }
}

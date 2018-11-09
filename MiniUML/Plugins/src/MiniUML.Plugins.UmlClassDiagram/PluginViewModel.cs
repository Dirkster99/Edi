namespace MiniUML.Plugins.UmlClassDiagram
{
    using MiniUML.Framework;
    using MiniUML.Model.ViewModels.Document;
    using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements;
    using MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel;

    public partial class PluginViewModel : BaseViewModel
  {
    #region fields
    private ToolBoxControlViewModel mClassShapeBox = null;
    private ToolBoxControlViewModel mDeploymentShapeBox = null;
    private ToolBoxControlViewModel mUseCaseShapeBoxViewModel = null;
    private ToolBoxControlViewModel mActivityShapeBoxViewModel = null;
    private ToolBoxControlViewModel mConnectBox = null;

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
      this.mWindowViewModel = windowViewModel;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get a collection of commands which can be used to create Class shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel ClassShapeBox
    {
      get
      {
        if (this.mClassShapeBox == null)
          this.mClassShapeBox = new ToolBoxControlViewModel(this, UmlDiagrams.Class);

        return this.mClassShapeBox;
      }
    }

    /// <summary>
    /// Get a collection of commands which can be used to create Deployment shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel DeploymentShapeBox
    {
      get
      {
        if (this.mDeploymentShapeBox == null)
          this.mDeploymentShapeBox = new ToolBoxControlViewModel(this, UmlDiagrams.Deployment);

        return this.mDeploymentShapeBox;
      }
    }

    /// <summary>
    /// Get a collection of commands which can be used to create Use Case diagram shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel UseCaseShapeBox
    {
      get
      {
        if (this.mUseCaseShapeBoxViewModel == null)
          this.mUseCaseShapeBoxViewModel = new ToolBoxControlViewModel(this, UmlDiagrams.UseCase);

        return this.mUseCaseShapeBoxViewModel;
      }
    }

    /// <summary>
    /// Get a collection of commands which can be used to create Activity diagram shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel ActivityShapeBox
    {
      get
      {
        if (this.mActivityShapeBoxViewModel == null)
          this.mActivityShapeBoxViewModel = new ToolBoxControlViewModel(this, UmlDiagrams.Activity);

        return this.mActivityShapeBoxViewModel;
      }
    }

    /// <summary>
    /// Get a collection of commands which can be used to create connections between shapes on the canvas.
    /// </summary>
    public ToolBoxControlViewModel ConnectBox
    {
      get
      {
        if (this.mConnectBox == null)
          this.mConnectBox = new ToolBoxControlViewModel(this, UmlDiagrams.Connector);

        return this.mConnectBox;
      }
    }

    /// <summary>
    /// Get the current <seealso cref="IMiniUMLDocument"/> which contains the
    /// document with shapes and other items.
    /// </summary>
    public IMiniUMLDocument mWindowViewModel { get; private set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Thread safe method to find out whether document editing is currently permitted or not.
    /// </summary>
    /// <returns></returns>
    internal bool QueryEnableEditCommands()
    {
      lock (this.lockobj)
      {
        if (this.mWindowViewModel == null)
          return false;

        if (this.mWindowViewModel.vm_DocumentViewModel == null)
          return false;

        if (this.mWindowViewModel.vm_DocumentViewModel.dm_DocumentDataModel == null)
          return false;

        return this.mWindowViewModel.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready;
      }
    }
    #endregion methods
  }
}

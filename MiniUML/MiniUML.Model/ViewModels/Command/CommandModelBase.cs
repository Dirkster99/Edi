namespace MiniUML.Model.ViewModels.Command
{
  using System.Windows.Input;
  using Document;
  using Shapes;

  /// <summary>
  /// Model for a command
  /// </summary>
  public abstract class CommandModelBase
  {
    #region fields
    private string mToolBoxImageUrl = string.Empty;
    #endregion fields

    #region Constructors
    /// <summary>
    /// Standard constructor
    /// </summary>
    public CommandModelBase()
    {
    }
    #endregion

    #region Properties
    /// <summary>
    /// Get Url string of image representation of this command in GUI
    /// </summary>
    public virtual string ToolBoxImageUrl
    {
     get
     {
       return mToolBoxImageUrl;
     }

     protected set
     {
       if (mToolBoxImageUrl != value)
       {
          mToolBoxImageUrl = value;
       }
     }
   }

    /// <summary>
    /// Command to be executed via this model
    /// </summary>
    public abstract ICommand CreateShape { get; }
    #endregion Properties

    #region methods
    protected ShapeViewModelBase AddShape(CanvasViewModel canvasViewModel, ShapeViewModelBase element)
    {
      canvasViewModel.AddShape(element);
      ////viewModel.dm_DocumentDataModel.AddShape(element);

      return element;
    }
    #endregion methods
  }
}

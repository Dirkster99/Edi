namespace MiniUML.Framework
{
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media.Imaging;

  /// <summary>
  /// Model for a command
  /// </summary>
  public abstract class CommandModel
  {
    #region fields
    private readonly RoutedCommand mRoutedCommand;
    private string mName;
    private string mDescription;
    private BitmapImage mImage;
    #endregion fields

    #region Constructors

    public CommandModel()
    {
      this.mRoutedCommand = new RoutedCommand();
    }

    public CommandModel(RoutedUICommand command)
    {
      this.mRoutedCommand = command;
      this.mName = command.Text;
    }

    #endregion

    #region Properties

    /// <summary>
    /// RoutedCommand associated with the model.
    /// </summary>
    public RoutedCommand Command
    {
      get { return this.mRoutedCommand; }
    }

    /// <summary>
    /// Name of the command.
    /// </summary>
    public string Name
    {
      get { return this.mName; }
      set { this.mName = value; }
    }

    /// <summary>
    /// Description of the command.
    /// </summary>
    public string Description
    {
      get { return this.mDescription; }
      set { this.mDescription = value; }
    }

    /// <summary>
    /// Image representing the command.
    /// </summary>
    public BitmapImage Image
    {
      get { return this.mImage; }
      set { this.mImage = value; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Determines if the command is enabled. Override to provide custom behavior.
    /// Do not call the base version when overriding.
    /// </summary>
    public virtual void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
      e.Handled = true;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public abstract void OnExecute(object sender, ExecutedRoutedEventArgs e);

    #endregion
  }
}
namespace FileSystemModels.Models
{
  /// <summary>
  /// Wrapper class for <seealso cref="System.Environment.SpecialFolder"/> items.
  /// </summary>
  public class CustomFolderItemModel
  {
    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="specialFolder"></param>
    public CustomFolderItemModel(System.Environment.SpecialFolder specialFolder)
    {
      this.SpecialFolder = specialFolder;

      this.Path = PathModel.SpecialFolderHasPath(specialFolder);
    }

    /// <summary>
    /// Protected class constructor
    /// </summary>
    protected CustomFolderItemModel()
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets the file system path of this custom folder item.
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// Gets the <seealso cref="System.Environment.SpecialFolder"/> enumeration member
    /// associated with this class.
    /// </summary>
    public System.Environment.SpecialFolder SpecialFolder { get; private set; }
    #endregion properties
  }
}

namespace FileSystemModels.Models.FSItems
{
  using System.IO;
  using System.Security;

  public class FileModel : Base.FileSystemModel
  {
    #region fields
    FileInfo mFile;
    #endregion fields

    #region constructors
    /// <summary>
    /// Parameterized class  constructor
    /// </summary>
    /// <param name="model"></param>
    [SecuritySafeCritical]
    public FileModel(PathModel model)
      : base(model)
    {
      this.mFile = new FileInfo(model.Path);
    }
    #endregion constructors

    #region properties
    public DirectoryInfo Directory
    {
      get
      {
        return this.mFile.Directory;
      }
    }

    public string DirectoryName
    {
      get
      {
        return this.mFile.DirectoryName;
      }
    }

    public bool Exists
    {
      get
      {
        return this.mFile.Exists;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return this.mFile.IsReadOnly;
      }
    }

    public long Length
    {
      get
      {
        return this.mFile.Length;
      }
    }
    #endregion properties
  }
}

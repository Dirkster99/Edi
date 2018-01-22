using System;
namespace FileSystemModels.Models.FSItems
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Security;
  using System.Text;

  public class FolderModel : Base.FileSystemModel
  {
    #region fields
    DirectoryInfo mDir;
    #endregion fields

    #region constructors
    /// <summary>
    /// Parameterized class  constructor
    /// </summary>
    /// <param name="model"></param>
    [SecuritySafeCritical]
    public FolderModel(PathModel model)
      : base(model)
    {
      this.mDir = new DirectoryInfo(model.Path);
    }
    #endregion constructors

    #region properties
    /// <summary>
    /// Gets whether this folder really exists or not.
    /// </summary>
    public bool Exists
    {
      get
      {
        return this.mDir.Exists;
      }
    }

    /// <summary>
    /// Gets the parent directory of this folder.
    /// </summary>
    public DirectoryInfo Parent
    {
      get
      {
        return this.mDir.Parent;
      }
    }

    /// <summary>
    /// Gets the root entry of this folder.
    /// </summary>
    public DirectoryInfo Root
    {
      get
      {
        return this.mDir.Root;
      }
    }
    #endregion properties
  }
}
